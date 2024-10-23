using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using WebApp.BusinessLogic.Services.Interfaces;

namespace EmailWorker;
public class EmailProcessorService : BackgroundService
{
    private readonly ILogger<EmailProcessorService> logger;
    private readonly IConfiguration configuration;
    private readonly IEmailSender emailSender;
    private IModel channel;
    private IConnection connection;
    private EventingBasicConsumer consumer;

    public EmailProcessorService(IConfiguration configuration, IEmailSender emailSender, ILogger<EmailProcessorService> logger)
    {
        this.configuration = configuration;
        this.emailSender = emailSender;
        this.logger = logger;
        this.InitializeRabbitMQ();
    }

    private void InitializeRabbitMQ()
    {
        var factory = new ConnectionFactory
        {
            HostName = this.configuration["RabbitMQ:HostName"],
            UserName = this.configuration["RabbitMQ:UserName"],
            Password = this.configuration["RabbitMQ:Password"]
        };

        this.connection = factory.CreateConnection();
        this.channel = this.connection.CreateModel();
        _ = this.channel.QueueDeclare(queue: this.configuration["RabbitMQ:QueueName"], durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    private async void ProcessEmailQueue(object sender, BasicDeliverEventArgs eventArgs)
    {
        var body = eventArgs.Body.ToArray();
        var message = JsonSerializer.Deserialize<EmailMessage>(Encoding.UTF8.GetString(body));
        this.logger.LogInformation("Received message to send email to {ToEmail}", message.ToEmail);

        await this.emailSender.SendEmailAsync(message.ToEmail, message.Subject, message.Body);
        this.logger.LogInformation("Email sent to {ToEmail}", message.ToEmail);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this.consumer = new EventingBasicConsumer(this.channel);
        this.consumer.Received += this.ProcessEmailQueue;
        _ = this.channel.BasicConsume(queue: this.configuration["RabbitMQ:QueueName"], autoAck: true, consumer: this.consumer);

        this.logger.LogInformation("EmailProcessorService is running.");

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        this.channel.Close();
        this.connection.Close();
        base.Dispose();
    }
}
