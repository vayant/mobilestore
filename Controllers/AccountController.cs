using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mobilestore.Data;
using mobilestore.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using System.Linq;

namespace mobilestore.Controllers
{
    public class AccountController : BaseController
    {
        private readonly ILogger<AccountController> _logger;

        public AccountController(ApplicationDbContext context, ILogger<AccountController> logger)
            : base(context)
        {
            _logger = logger;
        }
        

        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPatch]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassword model)
        {
            try 
            {
                _logger.LogInformation("Получен запрос на смену пароля");
                
                if (!ModelState.IsValid)
                {
                    var errors = string.Join("; ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    _logger.LogWarning($"Ошибка валидации модели: {errors}");
                    return BadRequest(new { success = false, message = errors });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    _logger.LogWarning("Пользователь не найден в токене");
                    return BadRequest(new { success = false, message = "Пользователь не найден" });
                }

                var user = await _context.Users.FindAsync(int.Parse(userId));
                if (user == null)
                {
                    _logger.LogWarning($"Пользователь с ID {userId} не найден в базе");
                    return BadRequest(new { success = false, message = "Пользователь не найден" });
                }

                if (user.Password != model.CurrentPassword)
                {
                    _logger.LogWarning("Введен неверный текущий пароль");
                    return BadRequest(new { success = false, message = "Неверный текущий пароль" });
                }

                if (model.NewPassword == model.CurrentPassword)
                {
                    return BadRequest(new { success = false, message = "Новый пароль должен отличаться от текущего" });
                }

                user.Password = model.NewPassword;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Пароль успешно изменен для пользователя {user.Username}");
                
                return Ok(new { 
                    success = true, 
                    message = "Пароль успешно изменен" 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при смене пароля");
                return StatusCode(500, new { success = false, message = "Произошла ошибка при смене пароля" });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Пользователь с таким email уже существует" });
            }

            var user = new UserClass
            {
                Username = model.Username,
                Email = model.Email,
                Password = model.Password 
            };

            try 
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Пользователь создан успешно.");
                return StatusCode(201, new { success = true, message = "Регистрация успешна" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при регистрации пользователя");
                return StatusCode(500, new { message = "Ошибка при регистрации пользователя" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Json(new { success = true });
        }
    }
} 