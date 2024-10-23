using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using WebApp.BusinessLogic.Services.Interfaces;

namespace WebApp.BusinessLogic.Services;
public class RabbitMqPublisher : IRabbitMqPublisher
{
    private readonly IConfiguration configuration;
    private readonly string hostname;
    private readonly string queueName;
    private readonly string username;
    private readonly string password;

    public RabbitMqPublisher(IConfiguration configuration)
    {
        this.configuration = configuration;
        this.hostname = this.configuration["RabbitMQ:HostName"];
        this.queueName = this.configuration["RabbitMQ:QueueName"];
        this.username = this.configuration["RabbitMQ:UserName"];
        this.password = this.configuration["RabbitMQ:Password"];
    }

    public void PublishEmailMessage(string toEmail, string subject, string body)
    {
        var factory = new ConnectionFactory
        {
            HostName = this.hostname,
            UserName = this.username,
            Password = this.password,
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare(queue: this.queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

        var message = new { ToEmail = toEmail, Subject = subject, Body = body };
        var bodyBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        channel.BasicPublish(exchange: "qwe", routingKey: this.queueName, basicProperties: null, body: bodyBytes);
        Console.WriteLine($"Message published to RabbitMQ. Email: {toEmail}, Subject: {subject}, Body: {body}");
    }
}
