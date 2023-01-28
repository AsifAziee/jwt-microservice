using Edstem.Services.OrderAPI.Messaging;

namespace Edstem.Services.OrderAPI.Extension; 

public static class ApplicationBuilderExtensions
{
    private static IAzureServiceBusConsumer? ServiceBusConsumer { get; set; }
    public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
    {
        ServiceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();

        var hostApplicationLife = app.ApplicationServices.GetService<IHostApplicationLifetime>()!;

        hostApplicationLife.ApplicationStarted.Register(OnStart);
        hostApplicationLife.ApplicationStopped.Register(Onstop);
        return app;
    }
    private static void OnStart()
    {
        ServiceBusConsumer!.Start();
    }
    private static void Onstop()
    {
        ServiceBusConsumer?.Stop();
    }
}