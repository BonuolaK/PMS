using MassTransit;
using Microsoft.Extensions.Logging;
using PMS.Shared.PubSub;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Proj.Core
{
    public class TaskCompletedMessageConsumer : IConsumer<ITaskCompletedMessage>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TaskCompletedMessageConsumer> _logger;
        public TaskCompletedMessageConsumer(IServiceProvider serviceProvider, ILogger<TaskCompletedMessageConsumer> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<ITaskCompletedMessage> context)
        {
            try
            {
                var productService = _serviceProvider.GetService(typeof(IProjectService)) as IProjectService;

                var product =  productService.TryUpdateStatus(context.Message.ProjectId);

            }
            catch (Exception ex)
            {
                _logger.LogError("TaskChangedConsumerError", ex);
            }

        }
    }
}


