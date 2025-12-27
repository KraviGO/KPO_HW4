FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/GateWay/Gateway.Host/Gateway.Host.csproj src/GateWay/Gateway.Host/

RUN dotnet restore src/GateWay/Gateway.Host/Gateway.Host.csproj

COPY . .

RUN dotnet publish src/GateWay/Gateway.Host/Gateway.Host.csproj \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Gateway.Host.dll"]