using System.ComponentModel.DataAnnotations;
using mobilestore.Models;

namespace mobilestore.Models
{
    public class ItemInCart
    {
        public int Id { get; set; }
        public string? CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public ProductClass Product { get; set; } = null!;
    }
} 