using MassTransit;

namespace Application.Core
{
    public class OrderStatusStates : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }
        public string? CurrentState { get; set; }
        public string? DriverThatAccept { get; set; }
        public string? DeviceToken { get; set; }
        public List<string> DriversThatRejected { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; }
        public DateTime? LastStateChange { get; set; }
        public int Version { get; set; }
    }
}