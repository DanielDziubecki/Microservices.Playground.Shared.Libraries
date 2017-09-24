using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Logging.Model
{
    public class ServiceLog : ILog
    {
        public ServiceLog(Guid operationId, string protocol, string service, DateTime timeStamp, TimeSpan duration,
            Exception exception, int responseSize, int requestSize, string requestUri, string responseUri,
            string username,
            string userId, string level, bool request, bool response, bool published, bool subscribed,
            dynamic wholeMessage,
            string httpCode, string additionalInfo = "")
        {
            OperationId = operationId;
            Protocol = protocol;
            Service = service;
            TimeStamp = timeStamp;
            Duration = duration;
            Exception = exception;
            RequestUri = requestUri;
            ResponseUri = responseUri;
            Username = username;
            UserId = userId;
            Level = level;
            ResponseSize = responseSize;
            RequestSize = requestSize;
            Request = request;
            Response = response;
            Published = published;
            Subscribed = subscribed;
            HttpCode = httpCode;
            AdditionalInfo = additionalInfo;
            try
            {
                var serializedWholeMessage = JsonConvert.SerializeObject(wholeMessage, Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore,
                        ContractResolver = new IgnoreErrorPropertiesResolver()
                    });
                WholeMessage = serializedWholeMessage;
            }
            catch (Exception e)
            {
                WholeMessage = "Unable to serialize wholeMessage " + e.Message;
            }
        }

        public Guid OperationId { get; set; }
        public string Protocol { get; set; }
        public string Service { get; set; }
        public DateTime TimeStamp { get; set; }
        public TimeSpan Duration { get; set; }
        public Exception Exception { get; set; }
        public int ResponseSize { get; set; }
        public int RequestSize { get; set; }
        public string WholeMessage { get; set; }
        public string RequestUri { get; set; }
        public string ResponseUri { get; set; }
        public string Username { get; set; }
        public string UserId { get; set; }
        public string Level { get; set; }
        public bool Request { get; set; }
        public bool Response { get; set; }
        public bool Published { get; set; }
        public bool Subscribed { get; set; }
        public string HttpCode { get; set; }
        public string AdditionalInfo { get; set; }

        internal class IgnoreErrorPropertiesResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var property = base.CreateProperty(member, memberSerialization);
                var props = new List<string>
                {
                    "InputStream",
                    "Filter",
                    "Length",
                    "Position",
                    "ReadTimeout",
                    "WriteTimeout",
                    "LastActivityDate",
                    "LastUpdatedDate",
                    "Session"
                };

                if (props.Contains(property.PropertyName))
                {
                    property.Ignored = true;
                }
                return property;
            }
        }
    }
}