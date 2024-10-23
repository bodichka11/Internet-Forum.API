using EmailWorker;
using WebApp.BusinessLogic.Services;
using WebApp.BusinessLogic.Services.Interfaces;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<IEmailSender, EmailSenderService>();
builder.Services.AddHostedService<EmailProcessorService>();

var host = builder.Build();
host.Run();
