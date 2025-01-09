using FirebaseAdmin.Messaging;
using orders_microservice.Utils.Core.Src.Application.NotificationService;


namespace orders_microservice.Utils.Core.Src.Infrastructure.FireBaseNotificationsService
{
    public class FirebaseNotificationsService : INotificationService
    {
        
        public async Task SendNotification(string deviceToken, string? messageTitle, string? messageBody)
        {
            try
            {
                var message = new Message
                {
                    Token = deviceToken,
                    Notification = new Notification { Title = messageTitle, Body = messageBody },
                    Android = new AndroidConfig
                    {
                        Priority = Priority.High,
                        Notification = new AndroidNotification
                        {
                            Title = messageTitle,
                            Body = messageBody,
                            Priority = NotificationPriority.HIGH,
                            Sound = "default",
                            DefaultSound = false,
                            ChannelId = "default_channel"
                        }
                    }
                };

                var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                Console.WriteLine($"Notification sent successfully. Response: {response}");
            }
            catch (FirebaseMessagingException fcmEx)
            {
                Console.WriteLine($"Firebase Messaging Error: {fcmEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending notification: {ex.Message}");
                throw;
            }
        }
    }
}
