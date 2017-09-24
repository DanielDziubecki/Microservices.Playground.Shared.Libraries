using System;
using System.Threading.Tasks;
using MassTransit;

namespace Logging.Core.Handlers.RabbitMQ.Observers
{
    public class SendMessagesObserver : ISendObserver
    {
        private readonly ILoggingBusControl loggingBusControl;

        public SendMessagesObserver(ILoggingBusControl loggingBusControl)
        {
            this.loggingBusControl = loggingBusControl;
        }

        public Task PreSend<T>(SendContext<T> context) where T : class
        {
            return Task.CompletedTask;
        }

        public async Task PostSend<T>(SendContext<T> context) where T : class
        {

        }

        public async Task SendFault<T>(SendContext<T> context, Exception exception) where T : class
        {

        }
    }
}