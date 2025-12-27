FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/SharedKernel/SharedKernel.csproj src/SharedKernel/

COPY src/Accounts_Service/Accounts.Entities/Accounts.Entities.csproj src/Accounts_Service/Accounts.Entities/
COPY src/Accounts_Service/Accounts.UseCases/Accounts.UseCases.csproj src/Accounts_Service/Accounts.UseCases/
COPY src/Accounts_Service/Accounts.Infrastructure/Accounts.Infrastructure.csproj src/Accounts_Service/Accounts.Infrastructure/
COPY src/Accounts_Service/Accounts.Presentation/Accounts.Presentation.csproj src/Accounts_Service/Accounts.Presentation/
COPY src/Accounts_Service/Accounts.Host/Accounts.Host.csproj src/Accounts_Service/Accounts.Host/

RUN dotnet restore src/Accounts_Service/Accounts.Host/Accounts.Host.csproj

COPY src/ ./src/

RUN dotnet publish src/Accounts_Service/Accounts.Host/Accounts.Host.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .
EXPOSE 8080

ENTRYPOINT ["dotnet", "Accounts.Host.dll"]