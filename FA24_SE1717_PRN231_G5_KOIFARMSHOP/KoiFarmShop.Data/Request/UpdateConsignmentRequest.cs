using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiFarmShop.Data.Request
{
    public class UpdateConsignmentRequest
    {
        public string ConsignmentId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string KoiId { get; set; }
        [Required]
        public int Type { get; set; }
        [Required]
        [Range(0, double.MaxValue)]
        public double? DealPrice { get; set; }
        [Required]
        public string Method { get; set; }
        [Required]
        public int Status { get; set; }
        [Required]
        public string Note { get; set; }
        [Required]
        public string CustomerContact { get; set; }
        [Required]
        public string CustomerAddress { get; set; }
        [Required]
        public decimal? TotalWeight { get; set; }
    }
}
