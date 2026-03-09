# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["PlatformService.csproj", "./"]
RUN dotnet restore "PlatformService.csproj"

COPY . .
RUN dotnet publish "PlatformService.csproj" -c Release -o /app/out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/out .

# 🔴 REQUIRED FOR KUBERNETES
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "PlatformService.dll"]




## Build stage
#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#WORKDIR /src
#
#COPY ["PlatformService.csproj", "./"]
#RUN dotnet restore "PlatformService.csproj"
#
#COPY . .
#RUN dotnet publish "PlatformService.csproj" -c Release -o /app/out
#
## Runtime stage
#FROM mcr.microsoft.com/dotnet/aspnet:8.0
#WORKDIR /app
#COPY --from=build /app/out .
#ENV ASPNETCORE_ENVIRONMENT=Production
#ENTRYPOINT ["dotnet", "PlatformService.dll"]