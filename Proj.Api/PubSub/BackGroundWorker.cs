using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PMS.Shared.DataAccess.EF;
using Proj.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Proj.Api.PubSub
{
        public class BackGroundWorker : BackgroundService
        {
            private readonly ILogger<BackGroundWorker> _logger;
            private readonly IBusControl _busControl;
            private readonly IServiceProvider _serviceProvider;
            private readonly QueueSettings _queueSettings;
            public BackGroundWorker(IServiceProvider serviceProvider, ILogger<BackGroundWorker> logger, 
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
                    var dbContextService = _serviceProvider.GetService(typeof(BaseContext)) as BaseContext;

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

