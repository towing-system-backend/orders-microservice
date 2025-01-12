using Application.Core;

namespace Order.Application
{
    public class MissingDriverTokenError() : ApplicationError("Trying to assign a driver that is not register in the app") {} 
    
}
