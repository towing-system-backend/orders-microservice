using Application.Core;

namespace Order.Application
{
    public class NoAvailableDriversError(): ApplicationError("No avaiable drivers"){}
}
