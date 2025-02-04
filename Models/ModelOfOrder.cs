using System.ComponentModel.DataAnnotations;

namespace mobilestore.Models
{
    public class ModelOfOrder
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public UserClass User { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        
        [Required]
        [StringLength(500)]
        public string DeliveryAddress { get; set; } = string.Empty;
        
        [Required]
        public string DeliveryCity { get; set; } = string.Empty;
        
        public string? Comment { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Новый";
        
        public decimal TotalAmount { get; set; }
        
        public List<OrderItem> OrderItems { get; set; } = new();
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public ModelOfOrder Order { get; set; }
        public int ProductId { get; set; }
        public ProductClass Product { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        [Required]
        public decimal Price { get; set; }
    }
} 