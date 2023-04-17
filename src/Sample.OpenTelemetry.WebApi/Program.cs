using Sample.OpenTelemetry.WebApi.Core.Configurations;
using Sample.OpenTelemetry.WebApi.Core.Extensions;
using Serilog;

try
{
	var builder = WebApplication.CreateBuilder(args);
	builder.AddSerilog("Sample Jaeger");

	var appSettings = new AppSettings();
	builder.Configuration.Bind(appSettings);

	builder.Services.AddRouting(options => options.LowercaseUrls = true);
	builder.Services.AddControllers();
	builder.Services.AddEndpointsApiExplorer();
	builder.Services.AddSwaggerGen();

	builder.Services.AddOpenTelemetry(appSettings);
	builder.Services.AddHttpClient("google");

	var app = builder.Build();

	if (app.Environment.IsDevelopment())
	{
		app.UseSwagger();
		app.UseSwaggerUI();
	}

	app.UseAuthorization();
	app.MapControllers();

	await app.RunAsync();
}
catch (Exception ex)
{
	Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
	Log.Information("Server Shutting down...");
	Log.CloseAndFlush();
}