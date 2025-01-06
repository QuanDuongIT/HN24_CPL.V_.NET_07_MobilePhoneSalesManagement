using Microsoft.AspNetCore.Mvc;
using ServerApp.BLL.Services.InterfaceServices;
using ServerApp.DAL.Models;

namespace ServerApp.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // Lấy tất cả người dùng
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var users = await _userService.GetAllAsync();
            if (users == null || !users.Any())
            {
                return NotFound("No users found.");
            }
            return Ok(users);
        }

        // Lấy người dùng theo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }
            return Ok(user);
        }

        // Thêm người dùng mới
        [HttpPost]
        public async Task<ActionResult<User>> AddUser([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("User data is null.");
            }

            var result = await _userService.AddAsync(user);
            if (result > 0)
            {
                return CreatedAtAction(nameof(GetUserById), new { id = user.UserId }, user);
            }
            return BadRequest("Error while creating user.");
        }

        // Cập nhật thông tin người dùng
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(int id, [FromBody] User user)
        {
            if (user == null || id != user.UserId)
            {
                return BadRequest("User data is invalid.");
            }

            var updated = await _userService.UpdateAsync(user);
            if (updated == 0)
            {
                return NotFound($"User with ID {id} not found.");
            }

            return NoContent();  // Trả về 204 khi cập nhật thành công
        }

        // Xóa người dùng theo ID
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var deleted = await _userService.DeleteAsync(id);
            if (deleted == 0)
            {
                return NotFound($"User with ID {id} not found.");
            }

            return NoContent();  // Trả về 204 khi xóa thành công
        }
    }

}
