using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiFarmShop.Data.Request
{
    public class UpdateKoiFishRequest
    {
        public string KoiId { get; set; } = null!;

        public string? KoiName { get; set; }

        public string? Origin { get; set; }

        public string? Gender { get; set; }

        public int Age { get; set; }

        public double Size { get; set; }

        public string? Breed { get; set; }

        public string? Type { get; set; }

        public double Price { get; set; }

        public int Quantity { get; set; }

        public int OwnerType { get; set; }

        public string? Description { get; set; }

        public List<string> RemovedImage { get; set; } = [];

        public IFormFileCollection? AddedImage { get; set; } = null;
    }
}
