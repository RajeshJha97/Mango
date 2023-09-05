using System.ComponentModel.DataAnnotations;

namespace Mango.Services.CouponAPI.Models.DTO
{
    public class CouponUpdateDTO
    {
        [Required]
        public int CouponId { get; set; }
        [Required]
        public string CouponCode { get; set; }=string.Empty;
        [Required]
        public double DiscountAmount { get; set; }
        [Required]
        public int MinAmount { get; set; }
    }
}
