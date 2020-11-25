using System;
using System.Runtime.Serialization;

namespace SignalR.Demo.Common.Web
{
    public class WebServerSettingsException : Exception
    {
        public WebServerSettingsException() { }
        protected WebServerSettingsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public WebServerSettingsException(string? message) : base(message) { }
        public WebServerSettingsException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}