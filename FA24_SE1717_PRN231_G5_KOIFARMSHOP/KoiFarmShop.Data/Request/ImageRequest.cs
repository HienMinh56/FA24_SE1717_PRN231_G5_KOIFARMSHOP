using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiFarmShop.Data.Request
{
    public class ImageRequest
    {
        public string? ImageId { get; set; }
        public string KoiId { get; set; } 
        public string ImageURL { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string DeletedBy { get; set; }

    }
}
