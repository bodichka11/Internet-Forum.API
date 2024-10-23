namespace WebApp.BusinessLogic.Services.Interfaces;
public interface IRabbitMqPublisher
{
    void PublishEmailMessage(string toEmail, string subject, string body);
}
