using MassTransit;
using Sample.OpenTelemetry.Infrastructure.Consumers;
using Sample.OpenTelemetry.Worker.Consumers;

namespace Sample.OpenTelemetry.Worker.Extensions;
public static class MasstransitExtension
{
	public static void AddMassTransitWorkerExtension(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddMassTransit(x =>
		{
			x.AddDelayedMessageScheduler();
			x.SetKebabCaseEndpointNameFormatter();

			x.AddConsumer<ClientUpdateConsumer>(typeof(ClientUpdateConsumerDefinition));

			x.UsingRabbitMq((ctx, cfg) =>
			{
				cfg.Host(configuration.GetConnectionString("RabbitMq"));

				cfg.UseDelayedMessageScheduler();
				cfg.ConfigureEndpoints(ctx, new KebabCaseEndpointNameFormatter("dev", false));
				cfg.UseMessageRetry(retry => { retry.Interval(3, TimeSpan.FromSeconds(5)); });
			});
		});
	}
}
