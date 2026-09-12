# syntax=docker/dockerfile:1

# ---------- Etapa de build ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/UserRegistration.Domain/UserRegistration.Domain.csproj src/UserRegistration.Domain/
COPY src/UserRegistration.Application/UserRegistration.Application.csproj src/UserRegistration.Application/
COPY src/UserRegistration.Infrastructure/UserRegistration.Infrastructure.csproj src/UserRegistration.Infrastructure/
COPY src/UserRegistration.Api/UserRegistration.Api.csproj src/UserRegistration.Api/
RUN dotnet restore src/UserRegistration.Api/UserRegistration.Api.csproj

COPY src/ src/
RUN dotnet publish src/UserRegistration.Api/UserRegistration.Api.csproj -c Release -o /app/publish --no-restore

# ---------- Etapa final ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "UserRegistration.Api.dll"]
