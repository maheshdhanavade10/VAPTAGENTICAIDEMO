# Multi-stage Dockerfile for building and running the VaptTestingDemo.API (.NET 8)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers for better caching
COPY ["VaptTestingDemo.API.csproj", "./"]
RUN dotnet restore "./VaptTestingDemo.API.csproj"

# Copy everything else and publish
COPY . .
RUN dotnet publish "VaptTestingDemo.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

# Listen on port 80 by default
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "VaptTestingDemo.API.dll"]
