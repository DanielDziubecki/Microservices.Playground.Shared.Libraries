using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Logging.Model;
using MassTransit;

namespace Logging.Core.Handlers.RabbitMQ.Observers
{
    //todo: method to retrieve header and some log factory to avoid code duplication
    public class RecievedMessagesObserver : IReceiveObserver
    {
        private readonly ILoggingBusControl loggingBusControl;

        public RecievedMessagesObserver(ILoggingBusControl loggingBusControl)
        {
            this.loggingBusControl = loggingBusControl;
        }

        public Task PreReceive(ReceiveContext context)
        {
            return Task.CompletedTask;
        }

        public Task PostReceive(ReceiveContext context)
        {
            return Task.CompletedTask;
        }

        public async Task PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
            where T : class
        {
            Guid operationId;
            Exception exception = null;
            Guid.TryParse(context.Headers.Get(LogConstansts.Common.OperationId, ""), out operationId);
            var publisher = context.Headers.Get(LogConstansts.QueueMessageHeaderNames.Publisher, "");

            if (operationId == Guid.Empty)
                exception = new Exception("Operation Id was not set in header of" + typeof(T).Name + " message");

            if (string.IsNullOrEmpty(publisher))
                exception = new Exception("Publisher was not set in header of" + typeof(T).Name + " message");

            var log = new ServiceLog(operationId, LogConstansts.Protocols.Ampq, $"Publisher: {publisher} \r\n Consumer:{consumerType}", DateTime.UtcNow,
                duration, exception, 0,0, context.DestinationAddress.AbsoluteUri,
                context.SourceAddress.AbsoluteUri, "From JWT in headers", "From JwT in headers",
                LogConstansts.Levels.Debug, false, false, false, true, context, "");

            await loggingBusControl.Log(log);
        }

        public async Task ConsumeFault<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType,
            Exception exception) where T : class
        {
            Guid operationId;
            Guid.TryParse(context.Headers.Get(LogConstansts.Common.OperationId, ""), out operationId);
            var publisher = context.Headers.Get(LogConstansts.QueueMessageHeaderNames.Publisher, "");
            var additionalInfo = "";
            if (operationId == Guid.Empty)
                additionalInfo = "Operation Id was not set in header of" + typeof(T).Name + " message";

            if (string.IsNullOrEmpty(publisher))
                additionalInfo = "Publisher was not set in header of" + typeof(T).Name + " message";

            var log = new ServiceLog(operationId, LogConstansts.Protocols.Ampq, $"Publisher: {publisher} \r\n Consumer:{consumerType}", DateTime.UtcNow,
                duration, exception, 0,0, context.DestinationAddress.AbsoluteUri,
                context.SourceAddress.AbsoluteUri, "From JWT in headers", "From JwT in headers",
                LogConstansts.Levels.Debug, false, false, false, true, context, "", additionalInfo);

            await loggingBusControl.Log(log);
        }

        public async Task ReceiveFault(ReceiveContext context, Exception exception)
        {
            string message;
            using (var reader = new StreamReader(context.GetBody()))
            {
                message = reader.ReadToEnd();
            }

            var log = new ServiceLog(Guid.Empty, LogConstansts.Protocols.Ampq, "Cannot recognize", DateTime.UtcNow,
                context.ElapsedTime, exception,0 ,Encoding.UTF8.GetByteCount(message), "Not found",
                "Not found", "From JWT in headers", "From JwT in headers",
                LogConstansts.Levels.Debug, false, false, false, true, context, "");

            await loggingBusControl.Log(log);
        }
    }
}