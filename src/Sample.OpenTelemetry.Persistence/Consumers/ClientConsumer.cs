using MassTransit;
using Microsoft.Extensions.Logging;
using Sample.OpenTelemetry.Infrastructure.Context;
using Sample.OpenTelemetry.Infrastructure.ViewModels;

namespace Sample.OpenTelemetry.Infrastructure.Consumers;

public class ClientConsumer : IConsumer<ClientViewModel>
{
	private readonly ClientContext _context;
	private readonly ILogger<ClientConsumer> _logger;

	public ClientConsumer(ClientContext context, ILogger<ClientConsumer> logger)
	{
		_context = context;
		_logger = logger;
	}

	public async Task Consume(ConsumeContext<ClientViewModel> context)
	{
		var client = _context.Clients.FirstOrDefault();

		if (client != null)
		{
			client.Email = "email2@email.com";
			_context.Clients.Update(client);
			await _context.SaveChangesAsync();
		}

		_logger.LogInformation($"Cliente {context.Message.Nome} indexado com sucesso!");
	}
}

public class IndexClientProductConsumerDefinition : ConsumerDefinition<ClientConsumer>
{
	protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<ClientConsumer> consumerConfigurator)
	{
		consumerConfigurator.UseMessageRetry(retry => retry.Interval(3, TimeSpan.FromSeconds(3)));
	}
}