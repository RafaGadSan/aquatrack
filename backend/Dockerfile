FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY AquaTrack.sln ./
COPY src/AquaTrack.Domain/AquaTrack.Domain.csproj src/AquaTrack.Domain/
COPY src/AquaTrack.Application/AquaTrack.Application.csproj src/AquaTrack.Application/
COPY src/AquaTrack.Infrastructure/AquaTrack.Infrastructure.csproj src/AquaTrack.Infrastructure/
COPY src/AquaTrack.Api/AquaTrack.Api.csproj src/AquaTrack.Api/
COPY tests/AquaTrack.Domain.Tests/AquaTrack.Domain.Tests.csproj tests/AquaTrack.Domain.Tests/
COPY tests/AquaTrack.Application.Tests/AquaTrack.Application.Tests.csproj tests/AquaTrack.Application.Tests/
COPY tests/AquaTrack.Api.IntegrationTests/AquaTrack.Api.IntegrationTests.csproj tests/AquaTrack.Api.IntegrationTests/
RUN dotnet restore AquaTrack.sln

COPY . .
RUN dotnet publish src/AquaTrack.Api/AquaTrack.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "AquaTrack.Api.dll"]
