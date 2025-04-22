# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["GreenRoam/GreenRoam.csproj", "GreenRoam/"]
COPY ["Service/Service.csproj", "Service/"]
COPY ["Repository/Repository.csproj", "Repository/"]
COPY ["BusinessObject/BusinessObject.csproj", "BusinessObject/"]
COPY ["DataAccessObject/DataAccessObject.csproj", "DataAccessObject/"]
RUN dotnet restore "GreenRoam/GreenRoam.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR "/src/GreenRoam"
RUN dotnet build "GreenRoam.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "GreenRoam.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose port 80
EXPOSE 80

ENTRYPOINT ["dotnet", "GreenRoam.dll"]