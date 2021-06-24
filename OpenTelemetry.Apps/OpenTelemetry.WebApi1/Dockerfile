#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["OpenTelemetry.WebApi1/OpenTelemetry.WebApi1.csproj", "OpenTelemetry.WebApi1/"]
RUN dotnet restore "OpenTelemetry.WebApi1/OpenTelemetry.WebApi1.csproj"
COPY . .
WORKDIR "/src/OpenTelemetry.WebApi1"
RUN dotnet build "OpenTelemetry.WebApi1.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OpenTelemetry.WebApi1.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpenTelemetry.WebApi1.dll"]