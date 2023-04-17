namespace Sample.OpenTelemetry.WebApi.Core.ViewModels;

public class CidadeViewModel
{
	public int Id { get; set; }
	public string Nome { get; set; }
	public MicrorregiaoViewModel Microrregiao { get; set; }
}