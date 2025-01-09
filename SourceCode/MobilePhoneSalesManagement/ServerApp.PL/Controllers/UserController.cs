using Microsoft.AspNetCore.Mvc;
using ServerApp.BLL.Services;
using ServerApp.BLL.Services.ViewModels;
using ServerApp.DAL.Models;

namespace ServerApp.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserDetailsService _userDetailsService;

        public UserController(IUserService userService, IUserDetailsService userDetailsService)
        {
            _userService = userService;
            _userDetailsService = userDetailsService;
        }

        // Lấy tất cả người dùng
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserVm>>> GetAllUsers()
        {
            // Lấy danh sách người dùng từ dịch vụ User
            var users = await _userService.GetAllUserAsync();
            if (users == null || !users.Any())
            {
                return NotFound("No users found.");
            }
            // Trả về danh sách UserVm
            return Ok(users);
        }

        // Lấy người dùng theo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<UserVm>> GetUserById(int id)
        {
            // Lấy thông tin user từ dịch vụ User
            var user = await _userService.GetByUserIdAsync(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            // Trả về UserVm
            return Ok(user);
        }


        // Thêm người dùng mới
        [HttpPost]
        public async Task<ActionResult<User>> AddUser([FromBody] UserVm user)
        {
            if (user == null)
            {
                return BadRequest("User data is null.");
            }

            // Kiểm tra tính hợp lệ của model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra xem email đã tồn tại trong DB chưa
            var existingUser = await _userService.GetUserByEmailAsync(user.Email);
            if (existingUser != null)
            {
                return BadRequest("Email already exists.");
            }

            // Thêm người dùng mới
            var newUserId = await _userService.AddUserAsync(user);
            var newUserDetailsId = await _userDetailsService.AddUserDetailsAsync(newUserId, user);
            if (newUserId > 0 && newUserDetailsId > 0)
            {
                return CreatedAtAction(nameof(GetUserById), new { id = newUserId }, user);
            }

            return BadRequest("Error while creating user.");
        }

        // Cập nhật thông tin người dùng
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserVm userVm)
        {
            if (id == null)
            {
                return BadRequest("Invalid User ID.");
            }

            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            else
            {
                await _userService.UpdateUserAsync(id, userVm);
                await _userDetailsService.UpdateUserDetailsAsync(user.UserId, userVm);
                return Ok("Update success");
            }
        }
        // Xóa người dùng theo ID
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var deletedDetails = await _userDetailsService.DeleteUserDetailsByUserIdAsync(id);
            var deletedUser = await _userService.DeleteByIdAsync(id);
            if (!deletedUser)
            {
                return NotFound($"User with ID {id} not found.");
            }
            if (!deletedDetails)
            {
                return NotFound($"UserDetails with UserId {id} not found.");
            }

            return Ok("Delete success");
        }

        [HttpGet("filter-by-last-active/{days}")]
        public async Task<ActionResult<List<User>>> FilterUsersByLastActive(int days)
        {
            if (days < 0)
            {
                return BadRequest("Days must be a non-negative integer.");
            }

            var filteredUsers = await _userService.FilterUsersByLastActiveAsync(days);

            if (filteredUsers == null || !filteredUsers.Any())
            {
                return NotFound($"No users found who were last active {days} or more days ago.");
            }

            return Ok(filteredUsers);
        }
    }

}
