using MassTransit;
using Sample.OpenTelemetry.Infrastructure.Context;
using Sample.OpenTelemetry.Infrastructure.ViewModels;

namespace Sample.OpenTelemetry.Worker.Consumers;

public class ClientUpdateConsumer : IConsumer<ClientViewModel>
{
	private readonly ClientContext _context;
	private readonly ILogger<ClientUpdateConsumer> _logger;

	public ClientUpdateConsumer(ClientContext context, ILogger<ClientUpdateConsumer> logger)
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

public class ClientUpdateConsumerDefinition : ConsumerDefinition<ClientUpdateConsumer>
{
	protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<ClientUpdateConsumer> consumerConfigurator)
	{
		consumerConfigurator.UseMessageRetry(retry => retry.Interval(3, TimeSpan.FromSeconds(3)));
	}
}