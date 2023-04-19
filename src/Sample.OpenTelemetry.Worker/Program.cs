using Microsoft.EntityFrameworkCore;
using Sample.OpenTelemetry.Infrastructure;
using Sample.OpenTelemetry.Infrastructure.Context;
using Sample.OpenTelemetry.WebApi.Core.Configurations;
using Sample.OpenTelemetry.WebApi.Core.Extensions;
using Sample.OpenTelemetry.Worker.Extensions;

var builder = WebApplication.CreateBuilder(args);
var appSettings = new AppSettings();
builder.Configuration.Bind(appSettings);

builder.AddOpenTelemetry(appSettings);

builder.Services.AddApiConfiguration();

builder.Services.AddDbContext<ClientContext>(opt =>
	opt.UseSqlServer(builder.Configuration.GetConnectionString("ClientContext")));

builder.Services.AddAutoMapper(typeof(MapperProfile));
builder.Services.AddMassTransitWorkerExtension(builder.Configuration);

builder.Services.AddHttpClient();

var app = builder.Build();

app.UseApiConfiguration();

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<ClientContext>();
dbContext.Database.Migrate();

await app.RunAsync();