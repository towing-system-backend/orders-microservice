using Application.Core;

namespace orders_microservice.Src.Application.Errors
{
    public class MissingDriverTokenError() : ApplicationError("Trying to assign a driver that is not register in the app") {} 
    
}
