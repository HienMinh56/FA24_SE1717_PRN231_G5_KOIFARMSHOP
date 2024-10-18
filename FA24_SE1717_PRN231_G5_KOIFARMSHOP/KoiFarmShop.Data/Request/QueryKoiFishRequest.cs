using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiFarmShop.Data.Request
{
    public class QueryKoiFishRequest
    {
        public string? KoiName { get; set; }

        public string? Origin { get; set; }

        public string? Gender { get; set; }

        public int Age { get; set; }

        public double Size { get; set; }

        public string? Breed { get; set; }

        public string? Type { get; set; }

        public double Price { get; set; }

        public int Quantity { get; set; }
    }
}
