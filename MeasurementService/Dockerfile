FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

COPY *.sln ./
COPY PatientService/PatientService.csproj ./PatientService/
COPY MeasurementService/MeasurementService.csproj ./MeasurementService/

RUN dotnet restore MeasurementService/MeasurementService.csproj

COPY PatientService/. ./PatientService/
COPY MeasurementService/. ./MeasurementService/

COPY MeasurementService/appsettings.MeasurementService.json /app/

RUN dotnet publish MeasurementService/MeasurementService.csproj -c Release -o /app/out/MeasurementService

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app

COPY --from=build /app/out/MeasurementService ./MeasurementService
COPY --from=build /app/appsettings.MeasurementService.json ./

EXPOSE 80

ENTRYPOINT ["dotnet", "MeasurementService/MeasurementService.dll"]
