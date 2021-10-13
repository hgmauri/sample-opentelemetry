FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Sample.OpenTelemetry.WebApi/Sample.OpenTelemetry.WebApi.csproj", "src/Sample.OpenTelemetry.WebApi/"]
COPY ["src/Sample.OpenTelemetry.WebApi.Core/Sample.OpenTelemetry.WebApi.Core.csproj", "src/Sample.OpenTelemetry.WebApi.Core/"]
RUN dotnet restore "src/Sample.OpenTelemetry.WebApi/Sample.OpenTelemetry.WebApi.csproj"
COPY . .
WORKDIR "/src/src/Sample.OpenTelemetry.WebApi"
RUN dotnet build "Sample.OpenTelemetry.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sample.OpenTelemetry.WebApi.csproj" -c Release -o /app/publish

FROM base AS final

ENV TZ=America/Sao_Paulo
ENV LANG pt-BR
ENV LANGUAGE pt-BR
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sample.OpenTelemetry.WebApi.dll"]