using Microsoft.Extensions.DependencyInjection;
using Payments.UseCases.Commands.CreatePaymentAccount;
using Payments.UseCases.Commands.TopUpAccount;
using Payments.UseCases.Commands.ProcessPayment;
using Payments.UseCases.Queries.GetBalance;

namespace Payments.UseCases;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPaymentsUseCases(this IServiceCollection services)
    {
        services.AddScoped<CreatePaymentAccountHandler>();
        services.AddScoped<TopUpAccountHandler>();
        services.AddScoped<ProcessPaymentHandler>();
        services.AddScoped<GetBalanceHandler>();

        return services;
    }
}
