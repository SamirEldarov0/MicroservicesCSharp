# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["CommandService.csproj", "./"]
RUN dotnet restore "CommandService.csproj"

COPY . .
RUN dotnet publish "CommandService.csproj" -c Release -o /app/out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "CommandService.dll"]
