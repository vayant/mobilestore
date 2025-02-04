using System.ComponentModel.DataAnnotations;

namespace mobilestore.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Введите текущий пароль")]
        [MinLength(6, ErrorMessage = "Пароль должен содержать минимум 6 символов")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите новый пароль")]
        [MinLength(6, ErrorMessage = "Пароль должен содержать минимум 6 символов")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Подтвердите новый пароль")]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
} 