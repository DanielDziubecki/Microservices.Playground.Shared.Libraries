using System;
using System.Threading.Tasks;
using Logging.Model;
using MassTransit;

namespace Logging.Core.Handlers.RabbitMQ.Observers
{
    //todo: method to retrieve header and some log factory to avoid code duplication
    public class PublishEventsObserver : IPublishObserver
    {
        private readonly ILoggingBusControl loggingBusControl;

        public PublishEventsObserver(ILoggingBusControl loggingBusControl)
        {
            this.loggingBusControl = loggingBusControl;
        }

        public async Task PrePublish<T>(PublishContext<T> context) where T : class
        {
            Guid operationId;
            Exception exception = null;
            Guid.TryParse(context.Headers.Get(LogConstansts.Common.OperationId, ""), out operationId);
            var publisher = context.Headers.Get(LogConstansts.QueueMessageHeaderNames.Publisher, "");

            if (operationId == Guid.Empty)
                exception = new Exception("Operation Id was not set in header of" + typeof(T).Name + " message");

            if (string.IsNullOrEmpty(publisher))
                exception = new Exception("Publisher was not set in header of" + typeof(T).Name + " message");

            var log = new ServiceLog(operationId, LogConstansts.Protocols.Ampq, publisher, DateTime.UtcNow,
                TimeSpan.Zero, exception, 0, 0,
                context.DestinationAddress.AbsoluteUri,
                context.SourceAddress.AbsoluteUri, "From JWT in headers", "From JwT in headers",
                LogConstansts.Levels.Debug, false, false, true, false, context, "");

            await loggingBusControl.Log(log);
        }

        public Task PostPublish<T>(PublishContext<T> context) where T : class
        {
            return Task.CompletedTask;
        }

        public async Task PublishFault<T>(PublishContext<T> context, Exception exception) where T : class
        {
            Guid operationId;
            Guid.TryParse(context.Headers.Get(LogConstansts.Common.OperationId, ""), out operationId);
            var publisher = context.Headers.Get(LogConstansts.QueueMessageHeaderNames.Publisher, "");

            var additionalInfo = "";
            if (operationId == Guid.Empty)
                additionalInfo = "Operation Id was not set in header of" + typeof(T).Name + " message";

            if (string.IsNullOrEmpty(publisher))
                additionalInfo = "Publisher was not set in header of" + typeof(T).Name + " message";

            var log = new ServiceLog(operationId, LogConstansts.Protocols.Ampq, publisher, DateTime.UtcNow,
                TimeSpan.Zero, exception, 0, 0,
                context.DestinationAddress.AbsoluteUri,
                context.SourceAddress.AbsoluteUri, "From JWT in headers", "From JwT in headers",
                LogConstansts.Levels.Error, false, false, true, false, context, "", additionalInfo);

            await loggingBusControl.Log(log);
        }
    }
}