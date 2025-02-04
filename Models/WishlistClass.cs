using System;
using System.ComponentModel.DataAnnotations;

namespace mobilestore.Models
{
    public class WishlistClass
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public ProductClass Product { get; set; } = null!;
        public DateTime DateAdded { get; set; }
    }
} 