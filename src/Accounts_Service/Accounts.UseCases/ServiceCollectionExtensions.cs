using Accounts.UseCases.Commands.AccountStatus;
using Accounts.UseCases.Commands.CreateAccount;
using Accounts.UseCases.Commands.UpdateProfile;
using Accounts.UseCases.Queries.GetAccount;
using Microsoft.Extensions.DependencyInjection;

namespace Accounts.UseCases;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAccountsUseCases(this IServiceCollection services)
    {
        services.AddScoped<CreateAccountHandler>();
        services.AddScoped<BlockAccountHandler>();
        services.AddScoped<ActivateAccountHandler>();
        services.AddScoped<UpdateProfileHandler>();

        services.AddScoped<GetAccountHandler>();

        return services;
    }
}