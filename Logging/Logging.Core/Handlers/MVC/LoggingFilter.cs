using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Logging.Model;
using Microsoft.AspNetCore.Mvc.Filters;
using NLog;

namespace Logging.Core.Handlers.MVC
{
    public class LoggingFilter : IAsyncActionFilter
    {
        private readonly ILogger logger;
        public LoggingFilter(ILogger logger)
        {
            this.logger = logger;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var request = context.HttpContext.Request;

            var operationId = Guid.Empty;
            try
            {
                if (request.Headers.ContainsKey(LogConstansts.Common.OperationId))
                {
                    var value = request.Headers[LogConstansts.Common.OperationId];
                    operationId = Guid.Parse(value);
                }
                else
                {
                    operationId = Guid.NewGuid();
                    request.Headers.Add(LogConstansts.Common.OperationId, operationId.ToString());
                }
            }
            catch (Exception e)
            {
                logger.Error(e);
            }

            var requestSize = 0;
            try
            {
                if (request.Body != null)
                {
                    using (var requesBody = new StreamReader(request.Body))
                    {
                        requestSize = Encoding.UTF8.GetByteCount(requesBody.ReadToEnd());
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e);
            }

            var requestLogEvent1 = new LogEventInfo(LogLevel.Info, logger.Name, "");
            requestLogEvent1.Properties["request-size"] = requestSize.ToString(); ;
            requestLogEvent1.Properties["protocol"] = LogConstansts.Protocols.Http;
            requestLogEvent1.Properties["operationid"] = operationId.ToString(); ;
            logger.Log(requestLogEvent1);


            var stopWatch = new Stopwatch();
            var responseSize = 0;
            var responseStatusCode = "";
            try
            {
                stopWatch.Start();
                var result = await next();
                stopWatch.Stop();
                var e =  result.Result;
                var response = result.HttpContext.Response;
                responseStatusCode = response.StatusCode.ToString();

                if (response.Body != null && response.Body.CanRead)
                {
                    using (var responseBody = new StreamReader(response.Body))
                    {
                        responseSize = Encoding.UTF8.GetByteCount(responseBody.ReadToEnd());
                    }
                   
                }
                if(result.Exception != null)
                    logger.Error(result.Exception);
            }
            catch (Exception e)
            {
               logger.Error(e);
            }
            finally
            {
                var resposneLogEvent1 = new LogEventInfo(LogLevel.Info, logger.Name, "");
                resposneLogEvent1.Properties["response-size"] = responseSize.ToString();
                resposneLogEvent1.Properties["protocol"] = LogConstansts.Protocols.Http;
                resposneLogEvent1.Properties["operationid"] = operationId.ToString();
                resposneLogEvent1.Properties["httpcode"] = responseStatusCode;
                logger.Log(resposneLogEvent1);
                stopWatch.Stop();
            }
        }
    }
}