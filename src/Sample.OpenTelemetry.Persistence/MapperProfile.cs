using AutoMapper;
using Sample.OpenTelemetry.Infrastructure.Context;
using Sample.OpenTelemetry.Infrastructure.ViewModels;

namespace Sample.OpenTelemetry.Infrastructure;
public class MapperProfile : Profile
{
	public MapperProfile()
	{
		CreateMap<Client, ClientViewModel>().ReverseMap();
	}
}
