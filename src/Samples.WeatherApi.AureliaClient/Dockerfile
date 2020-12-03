#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster AS build
WORKDIR /src
COPY ["src/Samples.WeatherApi.AureliaClient/Samples.WeatherApi.AureliaClient.csproj", "src/Samples.WeatherApi.AureliaClient/"]
RUN dotnet restore "src/Samples.WeatherApi.AureliaClient/Samples.WeatherApi.AureliaClient.csproj"
COPY . .
WORKDIR "/src/src/Samples.WeatherApi.AureliaClient"
RUN dotnet build "Samples.WeatherApi.AureliaClient.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Samples.WeatherApi.AureliaClient.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Samples.WeatherApi.AureliaClient.dll"]
