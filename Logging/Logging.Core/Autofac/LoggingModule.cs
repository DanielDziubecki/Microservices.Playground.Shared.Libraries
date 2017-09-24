using Autofac;
using Logging.Core.Handlers.RabbitMQ.Observers;
using Logging.Core.Handlers.WebApi;
using MassTransit;
using NLog;

namespace Logging.Core.Autofac
{
    public class LoggingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LoggingBusControl>().As<ILoggingBusControl>().SingleInstance();
            builder.RegisterType<HttpLoggingMiddleware>().As<IHttpLoggingMiddleware>();
            builder.RegisterType<RecievedMessagesObserver>().As<IReceiveObserver>();
            builder.RegisterType<SendMessagesObserver>().As<ISendObserver>();
            builder.RegisterType<PublishEventsObserver>().As<IPublishObserver>();
            builder.RegisterInstance(LogManager.GetLogger("rabbitmq")).As<ILogger>();
         
            base.Load(builder);
        }
    }
}