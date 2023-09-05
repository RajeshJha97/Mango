using System.ComponentModel.DataAnnotations;

namespace Mango.Services.CouponAPI.Models.DTO
{
    public class CouponCreateDTO
    {
        
        [Required]
        public string CouponCode { get; set; }=string.Empty;
        [Required]
        public double DiscountAmount { get; set; }
        [Required]
        public int MinAmount { get; set; }
    }
}
