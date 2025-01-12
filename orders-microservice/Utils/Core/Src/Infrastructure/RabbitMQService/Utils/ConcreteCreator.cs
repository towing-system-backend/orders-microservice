using Order.Infrastructure;

namespace RabbitMQ.Contracts
{
    public class DriverResponseDtoCreator : DtoCreator<DriverResponse, TowDriverResponseDto>
    {
        public override TowDriverResponseDto CreateDto(DriverResponse message)
        {
            return new TowDriverResponseDto(
                message.PublisherId,
                message.Status,
                string.Empty
            );
        }
    }
}
