using Accounts.Infrastructure.Persistence;
using Accounts.Infrastructure.Repositories;
using Accounts.UseCases.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Accounts.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAccountsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AccountsDbContext>(options =>
        {
            options.UseSqlite(
                configuration.GetConnectionString("AccountsDb"));
        });

        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}