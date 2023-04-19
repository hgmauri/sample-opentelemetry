namespace Sample.OpenTelemetry.Infrastructure.ViewModels;

public class CidadeViewModel
{
	public int Id { get; set; }
	public string Nome { get; set; }
	public MicrorregiaoViewModel Microrregiao { get; set; }
}