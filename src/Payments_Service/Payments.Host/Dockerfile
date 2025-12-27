FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Payments_Service/Payments.Host/Payments.Host.csproj src/Payments_Service/Payments.Host/
COPY src/Payments_Service/Payments.Presentation/Payments.Presentation.csproj src/Payments_Service/Payments.Presentation/
COPY src/Payments_Service/Payments.UseCases/Payments.UseCases.csproj src/Payments_Service/Payments.UseCases/
COPY src/Payments_Service/Payments.Infrastructure/Payments.Infrastructure.csproj src/Payments_Service/Payments.Infrastructure/
COPY src/Payments_Service/Payments.Entities/Payments.Entities.csproj src/Payments_Service/Payments.Entities/

RUN dotnet restore src/Payments_Service/Payments.Host/Payments.Host.csproj

COPY . .

RUN dotnet publish src/Payments_Service/Payments.Host/Payments.Host.csproj \
    -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Payments.Host.dll"]