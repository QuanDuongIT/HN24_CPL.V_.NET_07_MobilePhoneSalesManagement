using Microsoft.AspNetCore.Mvc;
using ServerApp.BLL.Services;
using ServerApp.BLL.Services.ViewModels;
using ServerApp.DAL.Models;
using System.Security.Claims;

namespace ServerApp.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserDetailsService _userDetailsService;

        public UsersController(IUserService userService, IUserDetailsService userDetailsService)
        {
            _userService = userService;
            _userDetailsService = userDetailsService;
        }

        // Lấy tất cả người dùng
        [HttpGet]
        public async Task<ActionResult<PagedResult<UserVm>>> GetAllUsers(int? pageNumber, int? pageSize)
        {
            // Lấy danh sách người dùng từ dịch vụ User
            var users = await _userService.GetAllUserAsync(pageNumber, pageSize);
            if (users == null || !users.Items.Any())
            {
                return NotFound("No users found.");
            }
            // Trả về danh sách PagedResult UserVm
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
            try
            {
                if (user == null)
                {
                    return BadRequest(new { success = false, message = "Dữ liệu người dùng trống." });
                }

                // Kiểm tra tính hợp lệ của model
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = ModelState });
                }

                // Kiểm tra xem email đã tồn tại trong DB chưa
                var existingUser = await _userService.GetUserByEmailAsync(user.Email);
                if (existingUser != null)
                {
                    return BadRequest(new { success = false, message = "Email đã tồn tại." });
                }

                // Thêm người dùng mới
                var newUserId = await _userService.AddUserAsync(user);
                if (newUserId > 0)
                {
                    return Ok(new { success = true, message = "Thêm thành công." });
                }
                return BadRequest(new { success = false, message = "Có lỗi trong quá trình thêm người dùng." });
            }

            catch (ExceptionBusinessLogic ex)
            {
                // Nếu có lỗi business logic, trả về BadRequest với thông điệp lỗi chi tiết
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                // Nếu có lỗi không xác định, trả về InternalServerError
                return StatusCode(500, new { success = false, message = "An unexpected error occurred.", errorDetails = ex.Message });
            }
        }

        // Cập nhật thông tin người dùng
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserVm userVm)
        {
            try
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
                    var result = await _userService.UpdateUserAsync(id, userVm);

                    if (result)
                    {
                        return Ok("Update successful");
                    }
                    return BadRequest("Some error");
                }
            }
            catch (ExceptionBusinessLogic ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                // Nếu có lỗi không xác định, trả về InternalServerError
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        // Xóa người dùng theo ID
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var deletedUser = await _userService.DeleteUserByIdAsync(id);
            if (!deletedUser)
            {
                return NotFound($"User with ID {id} not found.");
            }

            return Ok("Delete success");
        }
        [HttpDelete("delete-users-by-id-list")]
        public async Task<IActionResult> DeleteUsersByIdList([FromBody] List<int> userIds)
        {
            if (userIds == null || userIds.Count == 0)
            {
                return BadRequest("Danh sách UserId không hợp lệ.");
            }

            try
            {
                // Gọi phương thức DeleteUsersByIdAsync từ service
                var result = await _userService.DeleteUsersByIdAsync(userIds);

                if (result)
                {
                    return Ok("Xóa người dùng thành công.");
                }
                else
                {
                    return BadRequest("Không tìm thấy người dùng để xóa.");
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return StatusCode(500, new { message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordVm model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // lấy userId từ Claims của người dùng đang đăng nhập
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                // Nếu không có claim NameIdentifier, trả về lỗi yêu cầu người dùng đăng nhập
                if (userIdClaim == null)
                {
                    return Unauthorized("User is not logged in.");
                }

                var userId = int.Parse(userIdClaim.Value);

                var result = await _userService.ChangePasswordAsync(userId, model);

                if (result.Succeeded)
                {
                    return Ok("Password changed successfully.");
                }

                return BadRequest("Failed to change password.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("filter-by-last-active/{days}")]
        public async Task<ActionResult<List<User>>> FilterUsersByLastActive(int days)
        {
            if (days < 0)
            {
                return BadRequest("Days must be a non-negative integer.");
            }

            var filteredUsers = await _userService.FilterUsersAsync(null, days, null, null);

            if (filteredUsers == null || !filteredUsers.Items.Any())
            {
                return NotFound($"No users found who were last active {days} or more days ago.");
            }

            return Ok(filteredUsers);
        }

        [HttpGet("filter-search/{query}")]
        public async Task<ActionResult<List<User>>> FilterUsersByKeySearch(string query)
        {
            query = query.ToLower().Trim();
            if (query == "")
            {
                return BadRequest("Key search cann't empty.");
            }

            var filteredUsers = await _userService.FilterUsersAsync(query, null, null, null);

            if (filteredUsers == null || !filteredUsers.Items.Any())
            {
                return NotFound($"No users found");
            }

            return Ok(filteredUsers);
        }

        // block, unblock người dùng
        [HttpPost("toggle-block/{id}")]
        public async Task<IActionResult> ToggleBlockUser(int id)
        {
            var result = await _userService.ToggleBlockUserAsync(id);

            if (result)
            {
                return Ok(new { success = true, message = "Cập nhật trạng thái người dùng thành công!" });
            }
            return BadRequest("Không tìm thấy người dùng nào với các UserId đã cung cấp.");
        }
        [HttpPost("toggle-block-users")]
        public async Task<IActionResult> ToggleBlockUsersAsync([FromBody] List<int> userIds)
        {
            var result = await _userService.ToggleBlockUsersAsync(userIds);

            if (result)
            {
                return Ok(new { success = true, message = "Cập nhật trạng thái các người dùng thành công!" });
            }
            return BadRequest("Không tìm thấy người dùng nào với các UserId đã cung cấp.");
        }
    }


}
