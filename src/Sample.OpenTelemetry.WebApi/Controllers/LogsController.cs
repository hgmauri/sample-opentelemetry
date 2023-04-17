using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Sample.OpenTelemetry.WebApi.Core.ViewModels;

namespace Sample.OpenTelemetry.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class LogsController : ControllerBase
{
	private readonly ILogger<LogsController> _logger;
	private readonly IHttpClientFactory _httpClientFactory;

	public LogsController(ILogger<LogsController> logger, IHttpClientFactory httpClientFactory)
	{
		_logger = logger;
		_httpClientFactory = httpClientFactory;
	}

	[HttpGet("cidades")]
	public async Task<IEnumerable<CidadeViewModel>?> GetCitiesAsync()
	{
		IList<CidadeViewModel>? cidades = null;
		var client = _httpClientFactory.CreateClient("google");
		var response = await client.GetAsync("https://servicodados.ibge.gov.br/api/v1/localidades/estados/SP/municipios");

		if (response.IsSuccessStatusCode)
		{
			var result = await response.Content.ReadAsStringAsync();
			cidades = JsonSerializer.Deserialize<IList<CidadeViewModel>>(result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
		}
		return cidades;
	}

	[HttpGet("thread")]
	public IActionResult GetThreadAsync()
	{
		Task.Delay(3000);
		return Ok();
	}

	[HttpGet("exception")]
	public IActionResult GetException()
	{
		throw new ArgumentException("Erro");
	}
}