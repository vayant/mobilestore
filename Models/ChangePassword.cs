using System.ComponentModel.DataAnnotations;

namespace mobilestore.Models
{
    public class ChangePassword
    {
        [Required(ErrorMessage = "Введите текущий пароль")]
        [Display(Name = "Текущий пароль")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Введите новый пароль")]
        [Display(Name = "Новый пароль")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^[a-zA-Z0-9!@#$%^&*()_+\-=\[\]{};':""\\|,.<>/?]*$",
            ErrorMessage = "Пароль может содержать только латинские буквы, цифры и специальные символы")]
        [StringLength(100, MinimumLength = 6, 
            ErrorMessage = "Пароль должен содержать минимум 6 символов")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Подтвердите новый пароль")]
        [Display(Name = "Подтверждение пароля")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

        public bool IsSameAsCurrentPassword(string currentPassword)
        {
            return NewPassword == currentPassword;
        }
    }
} 