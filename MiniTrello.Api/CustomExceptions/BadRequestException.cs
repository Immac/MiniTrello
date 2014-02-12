using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MiniTrello.Api.CustomExceptions
{
    public class BadRequestException : HttpResponseException
    {
        public BadRequestException(HttpStatusCode statusCode) : base(statusCode)
        {
        }

        public BadRequestException(HttpResponseMessage response) : base(response)
        {
        }

        public BadRequestException(string errorMessage) : base(HttpStatusCode.BadRequest)
        {
            
            Response.ReasonPhrase = errorMessage;
        }
    }
}