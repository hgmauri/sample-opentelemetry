using MassTransit.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Sample.OpenTelemetry.WebApi.Core.Configurations;
using System.Diagnostics;

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
					.AddAspNetCoreInstrumentation(p =>
					{
						p.RecordException = true;
					})
					.AddHttpClientInstrumentation(p =>
					{
						p.RecordException = true;
					})
					.AddSqlClientInstrumentation(p =>
					{
						p.SetDbStatementForText = true;
						p.EnableConnectionLevelAttributes = true;
						p.RecordException = true;
					})
					.AddEntityFrameworkCoreInstrumentation(p =>
					{
						p.SetDbStatementForText = true;
					})
					.AddMassTransitInstrumentation()
					.SetSampler(new AlwaysOnSampler())
					.AddJaegerExporter(p =>
					{
						p.AgentHost = appSettings?.DistributedTracing?.Jaeger?.Host;
						p.AgentPort = appSettings?.DistributedTracing?.Jaeger?.Port ?? 0;
					});
			});

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

		builder.Services.Configure<AspNetCoreInstrumentationOptions>(options => options.RecordException = true);
		builder.Services.Configure<OpenTelemetryLoggerOptions>(opt =>
		{
			opt.IncludeScopes = true;
			opt.ParseStateValues = true;
			opt.IncludeFormattedMessage = true;
		});
		ActivitySource = new ActivitySource(appSettings?.DistributedTracing?.Jaeger?.ServiceName ?? string.Empty);
	}
}