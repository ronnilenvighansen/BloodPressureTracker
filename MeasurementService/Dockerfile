# Base image for .NET applications
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Build image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy project files
COPY ["PatientService/PatientService.csproj", "PatientService/"]
RUN dotnet restore "PatientService/PatientService.csproj"

# Copy the rest of the files and build
COPY PatientService/. ./PatientService/
WORKDIR "/src/PatientService"
RUN dotnet build -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PatientService.dll"]
