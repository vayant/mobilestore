using System.ComponentModel.DataAnnotations;

public class ModelOfRegistration
{
    [Required(ErrorMessage = "Email обязателен")]
    [EmailAddress(ErrorMessage = "Некорректный формат email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Имя пользователя обязательно")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Пароль обязателен")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль должен содержать минимум 6 символов")]
    [RegularExpression(@"^[a-zA-Z0-9!@#$%^&*()_+\-=\[\]{};':""\\|,.<>/?]*$",
        ErrorMessage = "Пароль может содержать только латинские буквы, цифры и специальные символы")]
    public string Password { get; set; }
} 