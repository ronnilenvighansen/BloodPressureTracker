FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

COPY PatientService/PatientService.csproj ./PatientService/

RUN dotnet restore PatientService/PatientService.csproj

COPY PatientService/. ./PatientService/

RUN dotnet publish PatientService/PatientService.csproj -c Release -o /app/out/PatientService

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app

COPY --from=build /app/out/PatientService ./PatientService

EXPOSE 80

ENTRYPOINT ["dotnet", "PatientService/PatientService.dll"]