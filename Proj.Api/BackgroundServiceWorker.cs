using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PMS.Shared.NetCore;
using Proj.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Proj.Api
{
    public class BackgroundServiceWorker : BackgroundService
    {
        private readonly ILogger<BackgroundServiceWorker> _logger;
        private readonly IBusControl _busControl;
        private readonly IServiceProvider _serviceProvider;
        private readonly QueueSettings _queueSettings;
        public BackgroundServiceWorker(IServiceProvider serviceProvider, ILogger<BackgroundServiceWorker> logger, 
            IBusControl busControl, QueueSettings queueSettings)
        {
            _logger = logger;
            _busControl = busControl;
            _serviceProvider = serviceProvider;
            _queueSettings = queueSettings;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var dbContextService = _serviceProvider.GetService(typeof(DbContext)) as DbContext;
                
                //dbContextService.EnsureDbCreated();

                _logger.LogInformation("DataProcessor started!");


                var productChangeHandler = _busControl.ConnectReceiveEndpoint(_queueSettings.QueueName, x =>
                {
                    x.Consumer<TaskCompletedMessageConsumer>(_serviceProvider);
                });

                await productChangeHandler.Ready;
            }
            catch (Exception ex)
            {
                _logger.LogError("DataProcessor cannot be started.", ex);
            }
        }
    }
}
