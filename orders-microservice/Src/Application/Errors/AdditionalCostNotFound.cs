using Application.Core;

namespace orders_microservice.Src.Application.Errors
{
    public class AdditionalCostNotFoundError : ApplicationError
    {
        public AdditionalCostNotFoundError() : base("Additional cost not found.") { }

    }   
}
