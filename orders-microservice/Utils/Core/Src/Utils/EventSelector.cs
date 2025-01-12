using RabbitMQ.Contracts;

namespace Application.Core;
public class EventSelector
{
    private readonly IPublishEndPointService _publishEndpoint;
    private readonly Dictionary<string, Func<string, Task>> _statusHandlers;
    public EventSelector(IPublishEndPointService publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
        _statusHandlers = new Dictionary<string, Func<string, Task>>
        {
            ["Accepted"] = async orderId => await _publishEndpoint.Publish(new EventOrderAccepted(Guid.Parse(orderId), DateTime.Now)),
            ["Located"] = async orderId => await _publishEndpoint.Publish(new EventOrderLocated(Guid.Parse(orderId), DateTime.Now)),
            ["InProgress"] = async orderId => await _publishEndpoint.Publish(new EventOrderInProgress(Guid.Parse(orderId), DateTime.Now)),
            ["Completed"] = async orderId => await _publishEndpoint.Publish(new EventOrderCompleted(Guid.Parse(orderId), DateTime.Now)),
            ["Paid"] = async orderId => await _publishEndpoint.Publish(new EventOrderPaid(Guid.Parse(orderId), DateTime.Now)),
            ["Cancelled"] = async orderId => await _publishEndpoint.Publish(new EventOrderCancelled(Guid.Parse(orderId), DateTime.Now))
        };
    }
    public async Task ExecuteEvent(string status, string orderId)
    {
        if (!_statusHandlers.ContainsKey(status))
        {
            throw new InvalidOperationException($"No handler defined for status: {status}");
        }

        await _statusHandlers[status](orderId);
    }
}