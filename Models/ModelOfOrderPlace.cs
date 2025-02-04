using System.ComponentModel.DataAnnotations;

namespace mobilestore.Models
{
    public class ModelOfOrderPlace
    {
        [Required(ErrorMessage = "Адрес доставки обязателен")]
        public string DeliveryAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Город доставки обязателен")]
        public string DeliveryCity { get; set; } = string.Empty;

        public string? Comment { get; set; }
    }
} 