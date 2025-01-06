using FirebaseAdmin.Messaging;

namespace orders_microservice.Utils.Core.Src.Application.NotificationService
{
    public interface INotificationService
    {
        Task SendNotification(string deviceToken, string? messageTitle, string? messageBody);
    }
}
