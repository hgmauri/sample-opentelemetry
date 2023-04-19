using MassTransit.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Sample.OpenTelemetry.WebApi.Core.Configurations;
using System.Diagnostics;
using OpenTelemetry;

namespace Sample.OpenTelemetry.WebApi.Core.Extensions;

public static class OpenTelemetryExtension
{
	public static ActivitySource ActivitySource;
	public static void AddOpenTelemetry(this WebApplicationBuilder builder, AppSettings appSettings)
	{
		void ConfigureResource(ResourceBuilder r) => r
			.AddService(serviceName: appSettings?.DistributedTracing?.Jaeger?.ServiceName ?? string.Empty,
			serviceVersion: typeof(OpenTelemetryExtension).Assembly.GetName().Version?.ToString() ?? "unknown",
			serviceInstanceId: Environment.MachineName)
			.AddTelemetrySdk()
			.AddEnvironmentVariableDetector()
			.AddAttributes(new Dictionary<string, object>
			{
				["environment.name"] = "development",
				["team.name"] = "backend"
			});

		builder.Services.AddOpenTelemetry()
			.ConfigureResource(ConfigureResource)
			.WithTracing(p =>
			{
				p.AddSource(DiagnosticHeaders.DefaultListenerName, appSettings.DistributedTracing.Jaeger.ServiceName, "MassTransit")
				.AddAspNetCoreInstrumentation(asp =>
				{
					asp.RecordException = true;
				})
				.AddHttpClientInstrumentation(http =>
				{
					http.RecordException = true;
				})
				.AddSqlClientInstrumentation(opt =>
				{
					opt.SetDbStatementForText = true;
					opt.EnableConnectionLevelAttributes = true;
					opt.RecordException = true;
				})
				.AddEntityFrameworkCoreInstrumentation(options =>
				{
					options.SetDbStatementForText = true;
				})
				.AddMassTransitInstrumentation()
				.SetSampler(new AlwaysOnSampler())
				.AddJaegerExporter(jaegerOptions =>
				{
					jaegerOptions.AgentHost = appSettings?.DistributedTracing?.Jaeger?.Host;
					jaegerOptions.AgentPort = appSettings?.DistributedTracing?.Jaeger?.Port ?? 0;
				});
			})
			.WithMetrics(p =>
			{
				p.AddMeter("MassTransit");
				p.ConfigureResource(resource =>
				{
					resource.AddService(appSettings?.DistributedTracing?.Jaeger?.ServiceName ?? string.Empty);
				})
				.AddAspNetCoreInstrumentation()
				.AddHttpClientInstrumentation()
				.AddRuntimeInstrumentation();
			});

		
		builder.Services.Configure<AspNetCoreInstrumentationOptions>(options => options.RecordException = true);

		builder.Services.AddLogging(build =>
		{
			build.SetMinimumLevel(LogLevel.Debug);
			build.AddOpenTelemetry(options =>
			{
				options.AddConsoleExporter().SetResourceBuilder(ResourceBuilder.CreateDefault()
					.AddService(appSettings?.DistributedTracing?.Jaeger?.ServiceName ?? string.Empty))
					.AddProcessor(new ActivityEventExtensions()).IncludeScopes = true;
			});
		});

		ActivitySource = new ActivitySource(appSettings?.DistributedTracing?.Jaeger?.ServiceName ?? string.Empty);
	}
}