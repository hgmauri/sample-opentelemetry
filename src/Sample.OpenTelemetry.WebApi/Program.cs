using Sample.OpenTelemetry.WebApi.Core.Configuration;
using Sample.OpenTelemetry.WebApi.Core.Extensions;

var builder = WebApplication.CreateBuilder(args);

var appSettings = new AppSettings();
builder.Configuration.Bind(appSettings);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = appSettings.DistributedTracing.Jaeger.ServiceName, Version = "v1" });
});

builder.Services.AddOpenTelemetry(appSettings);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{appSettings.DistributedTracing.Jaeger.ServiceName} v1"));
}

app.UseAuthorization();

app.MapControllers();

app.Run();