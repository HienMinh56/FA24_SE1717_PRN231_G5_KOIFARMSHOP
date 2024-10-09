using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace KoiFarmShop.Data.Response
{
    public enum ErrorType
    {
        NotFound,
        BadRequest,
        Unauthorized,
        InternalServerError,
        ForbiddenMethod
    }
    public class ErrorResponse
    {
        public int StatusCode { get; set; } = (int)HttpStatusCode.InternalServerError;

        [EnumDataType(typeof(ErrorType))]
        public required string ErrorType { get; set; }

        public required string ErrorMessage { get; set; }

        public string? StackTrace { get; set; }
    }
}