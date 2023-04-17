using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Sample.OpenTelemetry.WebApi.Core.Configurations;

namespace Sample.OpenTelemetry.WebApi.Core.Extensions;

public static class OpenTelemetryExtension
{
	public static void AddOpenTelemetry(this IServiceCollection services, AppSettings appSettings)
	{

		services.AddOpenTelemetry().WithTracing(telemetry =>
		{
			var resourceBuilder = ResourceBuilder
				.CreateDefault()
				.AddService(appSettings?.DistributedTracing?.Jaeger?.ServiceName ?? string.Empty);

			telemetry
				.AddSource("MassTransit")
				.SetResourceBuilder(resourceBuilder)
				.AddAspNetCoreInstrumentation(asp =>
				{
					asp.RecordException = true;
				})
				.AddHttpClientInstrumentation(http =>
				{
					http.RecordException = true;
				})
				.AddSqlClientInstrumentation()
				.SetSampler(new AlwaysOnSampler())
				.AddJaegerExporter(jaegerOptions =>
				{
					jaegerOptions.AgentHost = appSettings?.DistributedTracing?.Jaeger?.Host;
					jaegerOptions.AgentPort = appSettings?.DistributedTracing?.Jaeger?.Port ?? 0;
				});
		});
	}
}