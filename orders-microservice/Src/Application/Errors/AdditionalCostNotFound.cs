using Application.Core;

namespace Order.Application
{
    public class AdditionalCostNotFoundError : ApplicationError
    {
        public AdditionalCostNotFoundError() : base("Additional cost not found.") { }
    }   
}
