using System.Text.Json;
using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sample.OpenTelemetry.Infrastructure.Context;
using Sample.OpenTelemetry.Infrastructure.ViewModels;
using Sample.OpenTelemetry.WebApi.Core.Extensions;

namespace Sample.OpenTelemetry.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class LogsController : ControllerBase
{
	private readonly IMapper _mapper;
	private readonly IPublishEndpoint _publishEndpoint;
	private readonly ILogger<LogsController> _logger;
	private readonly ClientContext _context;
	private readonly HttpClient _httpClient;

	public LogsController(HttpClient httpClient, ClientContext context, IMapper mapper, IPublishEndpoint publishEndpoint, ILogger<LogsController> logger)
	{
		_httpClient = httpClient;
		_context = context;
		_mapper = mapper;
		_publishEndpoint = publishEndpoint;
		_logger = logger;
	}

	[HttpGet("cidades")]
	public async Task<IEnumerable<CidadeViewModel>?> GetCitiesAsync()
	{
		using var activity = OpenTelemetryExtension.ActivitySource.StartActivity("HttpClient");
		IList<CidadeViewModel>? cidades = null;
		var response = await _httpClient.GetAsync("https://servicodados.ibge.gov.br/api/v1/localidades/estados/SP/municipios");

		if (response.IsSuccessStatusCode)
		{
			var result = await response.Content.ReadAsStringAsync();
			cidades = JsonSerializer.Deserialize<IList<CidadeViewModel>>(result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
			_logger.LogInformation($"Total de cidades: {cidades.Count}");
		}
	
		return cidades;
	}

	[HttpGet("thread")]
	public async Task<IActionResult> GetThreadAsync()
	{
		await Task.Delay(3000);
		return Ok();
	}

	[HttpGet("exception")]
	public IActionResult GetException()
	{
		throw new ArgumentException("Erro");
	}

	[HttpPost("client")]
	public async Task<IActionResult> PostClient([FromBody] ClientViewModel model)
	{
		var entity = _mapper.Map<Client>(model);

		await _context.Clients.AddAsync(entity);
		await _context.SaveChangesAsync();

		await _publishEndpoint.Publish(model);
		_logger.LogInformation($"Cliente salvo com sucesso!");

		return Ok();
	}

	[HttpGet("client")]
	public async Task<IActionResult> GetClient()
	{
		var clients = await _context.Clients.ToListAsync();

		var result = _mapper.Map<List<ClientViewModel>>(clients);

		return Ok(result);
	}
}