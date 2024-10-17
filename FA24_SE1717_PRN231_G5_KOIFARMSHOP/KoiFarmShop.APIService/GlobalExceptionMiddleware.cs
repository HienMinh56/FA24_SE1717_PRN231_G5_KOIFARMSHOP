using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using KoiFarmShop.Common.Exceptions;
using KoiFarmShop.Data.Response;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;

namespace KoiFarmShop.APIService
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, $"Something went wrong while processing {context.Request.Path}");
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var errorResponse = new ErrorResponse
            {
                ErrorType = ErrorType.InternalServerError.ToString(),
                ErrorMessage = exception.Message,
                StackTrace = exception.StackTrace
            };

            switch (exception)
            {
                case NotFoundException _:
                    errorResponse.ErrorType = ErrorType.NotFound.ToString();
                    errorResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                case BadRequestException _:
                    errorResponse.ErrorType = ErrorType.BadRequest.ToString();
                    errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case UnauthorizedException _:
                    errorResponse.ErrorType = ErrorType.Unauthorized.ToString();
                    errorResponse.StatusCode = (int)HttpStatusCode.Unauthorized;
                    break;
                case ForbiddenMethodException _:
                    errorResponse.ErrorType = ErrorType.ForbiddenMethod.ToString();
                    errorResponse.StatusCode = (int)HttpStatusCode.Forbidden;
                    break;
                default:
                    errorResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            var response = JsonConvert.SerializeObject(errorResponse);
            context.Response.StatusCode = errorResponse.StatusCode;

            return context.Response.WriteAsync(response);
        }
    }

}
