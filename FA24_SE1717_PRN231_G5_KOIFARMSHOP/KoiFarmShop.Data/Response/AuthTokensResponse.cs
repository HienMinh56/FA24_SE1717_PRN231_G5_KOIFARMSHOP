using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KoiFarmShop.Data.Response
{
    public class AuthTokensResponse
    {
        public required string AccessToken { get; set; }

        public required string RefreshToken { get; set; }

    }
}