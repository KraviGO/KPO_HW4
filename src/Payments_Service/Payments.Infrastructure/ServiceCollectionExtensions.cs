using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payments.Infrastructure.Messaging;
using Payments.Infrastructure.Persistence;
using Payments.Infrastructure.Repositories;
using Payments.UseCases.Abstractions;

namespace Payments.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPaymentsInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var cs = config.GetConnectionString("PaymentsDb") ?? "Data Source=payments.db";
        services.AddDbContext<PaymentsDbContext>(o => o.UseSqlite(cs));

        var opt = new RabbitMqOptions();
        config.GetSection("RabbitMq").Bind(opt);
        services.AddSingleton(opt);

        services.AddSingleton<RabbitMqConnectionFactory>();
        services.AddSingleton<RabbitMqPublisher>();

        services.AddScoped<IPaymentAccountRepository, PaymentAccountRepository>();
        services.AddScoped<IPaymentOperationRepository, PaymentOperationRepository>();
        services.AddScoped<IInboxMessageRepository, InboxMessageRepository>();
        services.AddScoped<IOutboxMessageRepository, OutboxMessageRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddHostedService<PaymentRequestConsumer>();
        services.AddHostedService<OutboxDispatcher>();

        return services;
    }
}
