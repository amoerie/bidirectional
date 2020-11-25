using System.Globalization;
using System.Net;

namespace SignalR.Demo.Common.Extensions
{
    public static class ExtensionsForString
    {
        public static IPAddress? ToIpAddress(this string? @this)
        {
            if (@this == null)
                return null;

            switch (@this.ToUpper(CultureInfo.InvariantCulture))
            {
                case "ANY":
                case "*":
                    return IPAddress.Any;
                case "LOOPBACK":
                case "LOCALHOST":
                    return IPAddress.Loopback;
                default:
                    return IPAddress.TryParse(@this, out var parsed) ? parsed : null;
            }
        }

    }
}