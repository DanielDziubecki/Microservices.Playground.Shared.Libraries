

using System;
using System.Threading.Tasks;
using Logging.Model;
using MassTransit;
using Newtonsoft.Json;

namespace Logging.Core
{
    public class LoggingBusControl : ILoggingBusControl
    {
        private readonly IBusControl busControl;
        public LoggingBusControl()
        {
            busControl = GetBusControl();
            busControl.Start();
        }

        private static IBusControl GetBusControl()
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                var host = x.Host(new Uri("rabbitmq://localhost/#/queues/%2F/logging_queue"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
                x.ConfigureJsonSerializer(settings =>
                {
                    settings.NullValueHandling = NullValueHandling.Ignore;
                    return settings;
                });
            });
            return busControl;
        }


        public async Task Log<T>(T log) where T : ILog
        {
            await busControl.Publish<ILog>(log);
        }
    }
}