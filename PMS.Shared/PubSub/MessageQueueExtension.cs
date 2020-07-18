using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PMS.Shared.NetCore;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.Shared.PubSub
{
    public static class MessageQueueExtension
    {
        public static IServiceCollection RegisterQueueServices(this IServiceCollection services, IConfiguration section)
        {
            var appSettingsSection = section.GetSection("AppSettings");
            var appSettings = appSettingsSection.Get<AppSettings>();

            services.AddSingleton(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(appSettings.QueueSettings.HostName, appSettings.QueueSettings.VirtualHost, (x =>
                {
                    x.Username(appSettings.QueueSettings.UserName);
                    x.Password(appSettings.QueueSettings.Password);
                }));
                cfg.ExchangeType = ExchangeType.Direct;
            }
            )



            );

            services.AddSingleton<IPublishEndpoint>(provider => provider.GetRequiredService<IBusControl>());
            services.AddSingleton<ISendEndpointProvider>(provider => provider.GetRequiredService<IBusControl>());
            services.AddSingleton<IBus>(provider => provider.GetRequiredService<IBusControl>());

            return services;
        }
    }
}
