using Microsoft.Extensions.DependencyInjection;

namespace Accounts.Presentation;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAccountsPresentation(this IServiceCollection services)
    {
        services.AddControllers();
        return services;
    }
}