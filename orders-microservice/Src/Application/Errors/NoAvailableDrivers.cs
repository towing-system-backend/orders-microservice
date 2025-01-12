using Application.Core;

namespace orders_microservice.Src.Application.Errors
{
    public class NoAvailableDriversError(): ApplicationError("No avaiable drivers"){}
}
