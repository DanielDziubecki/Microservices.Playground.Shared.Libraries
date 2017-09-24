using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Logging.Model;
using Microsoft.AspNetCore.Http;
using NLog;

namespace Logging.Core.Handlers.WebApi
{
    public class HttpLoggingMiddleware : IHttpLoggingMiddleware
    {
        private readonly RequestDelegate requestDelegate;
        private readonly ILogger logger;

        public HttpLoggingMiddleware(RequestDelegate requestDelegate,
            ILogger logger)
        {
            this.requestDelegate = requestDelegate;
            this.logger = logger;
        }

        public async Task<HttpResponse> Invoke(HttpContext context)
        {
            var request = context.Request;

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
                    var responseBody = new StreamReader(request.Body).ReadToEnd();
                    requestSize = Encoding.UTF8.GetByteCount(responseBody);
                }
            }
            catch (Exception e)
            {
                logger.Error(e);
            }


            var filelog = LogManager.GetLogger("rabbit");
            var requestLogEvent1 = new LogEventInfo(LogLevel.Info, "rabbit", "");
            requestLogEvent1.Properties["request-size"] = requestSize.ToString(); ;
            requestLogEvent1.Properties["protocol"] = LogConstansts.Protocols.Http;
            requestLogEvent1.Properties["operationid"] = operationId.ToString(); ;

            filelog.Log(requestLogEvent1);

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            if (!context.Response.HasStarted)
                await requestDelegate.Invoke(context);

            stopWatch.Stop();

            var response = context.Response;
            var responseSize = 0;
            try
            {
                if (response.Body != null)
                {
                    var responseBody = new StreamReader(response.Body).ReadToEnd();
                    responseSize = Encoding.UTF8.GetByteCount(responseBody);
                }
            }
            catch (Exception e)
            {
                logger.Error(e);
            }


            var resposneLogEvent1 = new LogEventInfo(LogLevel.Info, "rabbit", "");
            resposneLogEvent1.Properties["response-size"] = responseSize.ToString();
            resposneLogEvent1.Properties["protocol"] = LogConstansts.Protocols.Http;
            resposneLogEvent1.Properties["operationid"] = operationId.ToString();
            resposneLogEvent1.Properties["httpcode"] = response.StatusCode.ToString();
            filelog.Log(resposneLogEvent1);

            return response;
        }
    }
}