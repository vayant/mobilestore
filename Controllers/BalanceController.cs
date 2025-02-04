using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using mobilestore.Data;
using Microsoft.EntityFrameworkCore;
using mobilestore.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace mobilestore.Controllers
{
    [Authorize]
    [Route("[controller]")]  
    public class BalanceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BalanceController> _logger;

        public BalanceController(ApplicationDbContext context, ILogger<BalanceController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet] 
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("GetBalance")] 
        public async Task<IActionResult> GetBalance()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Json(new { balance = 0 });
                }

                var user = await _context.Users.FindAsync(int.Parse(userId));
                return Json(new { balance = user?.Balance ?? 0 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении баланса");
                return Json(new { balance = 0 });
            }
        }

        [HttpPost("UpdateBalance")]
        public async Task<IActionResult> UpdateBalance([FromBody] JsonElement data)
        {
            try
            {
                _logger.LogInformation($"Получены данные: {data.GetRawText()}");

                var paymentType = data.GetProperty("paymentType").GetString();
                
                switch (paymentType)
                {
                    case "CARD":
                        var cardModel = JsonSerializer.Deserialize<CardPaymentModel>(data.GetRawText(), new JsonSerializerOptions 
                        { 
                            PropertyNameCaseInsensitive = true 
                        });
                        if (cardModel == null)
                        {
                            return BadRequest(new { success = false, message = "Ошибка обработки данных карты" });
                        }
                        return await ProcessCardPayment(cardModel);
                        
                    case "SBP":
                        var sbpModel = JsonSerializer.Deserialize<SBPPaymentModel>(data.GetRawText(), new JsonSerializerOptions 
                        { 
                            PropertyNameCaseInsensitive = true 
                        });
                        if (sbpModel == null)
                        {
                            return BadRequest(new { success = false, message = "Ошибка обработки данных СБП" });
                        }
                        _logger.LogInformation($"СБП модель: Сумма={sbpModel.Amount}, Телефон={sbpModel.PhoneNumber}");
                        return await ProcessSBPPayment(sbpModel);
                        
                    default:
                        return BadRequest(new { success = false, message = "Неизвестный тип платежа" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при пополнении баланса");
                return StatusCode(500, new { success = false, message = $"Произошла ошибка при пополнении баланса: {ex.Message}" });
            }
        }

        [HttpPost("ProcessCardPayment")]
        public async Task<IActionResult> ProcessCardPayment([FromBody] CardPaymentModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Некорректные данные" });
                }

                var username = User.Identity?.Name;
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

                if (user == null)
                {
                    return NotFound(new { success = false, message = "Пользователь не найден" });
                }

                var expiryParts = model.ExpiryDate.Split('/');
                if (expiryParts.Length != 2 || 
                    !int.TryParse(expiryParts[0], out int month) || 
                    !int.TryParse(expiryParts[1], out int year))
                {
                    return BadRequest(new { success = false, message = "Неверный формат даты" });
                }

                if (year < 100) year += 2000;
                var expiryDate = new DateTime(year, month, 1).AddMonths(1).AddDays(-1);
                if (expiryDate < DateTime.Now)
                {
                    return BadRequest(new { success = false, message = "Срок действия карты истек" });
                }

                if (user.Balance + model.Amount > 999999999)
                {
                    decimal maxPossibleTopUp = 999999999 - user.Balance;
                    if (maxPossibleTopUp <= 0)
                    {
                        return BadRequest(new { 
                            success = false, 
                            message = "Достигнут максимальный баланс" 
                        });
                    }
                    return BadRequest(new { 
                        success = false, 
                        message = $"Превышен максимальный баланс. Максимально возможная сумма пополнения: {maxPossibleTopUp:N0} ₽" 
                    });
                }

                user.Balance += model.Amount;
                await _context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status201Created, new { 
                    success = true, 
                    balance = user.Balance,
                    message = $"Баланс успешно пополнен картой на {model.Amount:N0} ₽" 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке оплаты картой");
                return StatusCode(500, new { success = false, message = "Произошла ошибка при обработке оплаты" });
            }
        }

        [HttpPost("ProcessSBPPayment")]
        public async Task<IActionResult> ProcessSBPPayment([FromBody] SBPPaymentModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Некорректные данные" });
                }

                var username = User.Identity?.Name;
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

                if (user == null)
                {
                    return NotFound(new { success = false, message = "Пользователь не найден" });
                }

                if (user.Balance + model.Amount > 999999999)
                {
                    decimal maxPossibleTopUp = 999999999 - user.Balance;
                    if (maxPossibleTopUp <= 0)
                    {
                        return BadRequest(new { 
                            success = false, 
                            message = "Достигнут максимальный баланс" 
                        });
                    }
                    return BadRequest(new { 
                        success = false, 
                        message = $"Превышен максимальный баланс. Максимально возможная сумма пополнения: {maxPossibleTopUp:N0} ₽" 
                    });
                }

                user.Balance += model.Amount;
                await _context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status201Created, new { 
                    success = true, 
                    balance = user.Balance,
                    message = $"Баланс успешно пополнен через СБП на {model.Amount:N0} ₽" 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке оплаты через СБП");
                return StatusCode(500, new { success = false, message = "Произошла ошибка при обработке оплаты" });
            }
        }
    }
} 