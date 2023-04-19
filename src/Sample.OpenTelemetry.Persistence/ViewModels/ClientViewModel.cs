using System.Text.Json.Serialization;

namespace Sample.OpenTelemetry.Infrastructure.ViewModels;
public class ClientViewModel
{
	[JsonIgnore]
	public Guid Id { get; set; } = Guid.NewGuid();
	public string Nome { get; set; }
	public string Email { get; set; }
	public DateTime DataNascimento { get; set; }
}
