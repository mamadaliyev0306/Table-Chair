# Base image with runtime only
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
ENV ASPNETCORE_URLS=http://+:8080

# SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Table-Chair/Table-Chair.csproj", "Table-Chair/"]
COPY ["Table-Chair-Application/Table-Chair-Application.csproj", "Table-Chair-Application/"]
COPY ["Table-Chair-Entity/Table-Chair-Entity.csproj", "Table-Chair-Entity/"]
RUN dotnet restore "./Table-Chair/Table-Chair.csproj"
COPY . .
WORKDIR "/src/Table-Chair"
RUN dotnet build "./Table-Chair.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish step
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Table-Chair.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Table-Chair.dll"]
