using System.ComponentModel.DataAnnotations;

namespace mobilestore.Models
{
    public abstract class PaymentBaseModel
    {
        [Required(ErrorMessage = "Сумма пополнения обязательна")]
        [Range(1, 2000000, ErrorMessage = "Сумма должна быть от 1 до 2 000 000 рублей")]
        public virtual decimal Amount { get; set; }

        [Required]
        public string PaymentType { get; set; } = string.Empty;
    }

    public class CardPaymentModel : PaymentBaseModel
    {
        [Required(ErrorMessage = "Номер карты обязателен")]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Неверный формат номера карты")]
        public string CardNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Срок действия карты обязателен")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/([0-9]{2})$", ErrorMessage = "Неверный формат срока действия")]
        public string ExpiryDate { get; set; } = string.Empty;

        [Required(ErrorMessage = "CVV код обязателен")]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "Неверный формат CVV")]
        public string CVV { get; set; } = string.Empty;
    }

    public class SBPPaymentModel : PaymentBaseModel
    {
        [Required(ErrorMessage = "Номер телефона обязателен")]
        [RegularExpression(@"^7\d{10}$", ErrorMessage = "Неверный формат номера телефона")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
} 