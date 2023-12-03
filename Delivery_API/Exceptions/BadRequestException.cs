using Delivery_Models.Models;

namespace Delivery_API.Exceptions
{
    public class BadRequestException:Exception
    {
        public Response ErrorResponse { get; }
        public BadRequestException(Response errorResponse) : base(errorResponse.Message) {
            ErrorResponse = errorResponse;
        }
    }
}
