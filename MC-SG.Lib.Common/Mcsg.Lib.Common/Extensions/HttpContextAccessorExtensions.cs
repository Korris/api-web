using Microsoft.AspNetCore.Http;
using System.Net;

namespace Mcsg.Lib.Common.Extensions
{
    public static class HttpContextAccessorExtensions
    {
        public static string GetIpAddress(this IHttpContextAccessor accessor)
        {
            var ipString = "";
            if (!string.IsNullOrEmpty(accessor.HttpContext.Request.Headers["X-Forwarded-Client-Ip"]))
                ipString = accessor.HttpContext.Request.Headers["X-Forwarded-Client-Ip"];
            else if (!string.IsNullOrEmpty(accessor.HttpContext.Request.Headers["X-FORWARDED-CLIENT-IP"]))
                ipString = accessor.HttpContext.Request.Headers["X-FORWARDED-CLIENT-IP"];
            else if (!string.IsNullOrEmpty(accessor.HttpContext.Request.Headers["CF-CONNECTING-IP"]))
                ipString = accessor.HttpContext.Request.Headers["CF-CONNECTING-IP"];


            if (string.IsNullOrEmpty(ipString))
            {
                ipString = accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            }
            if (IPEndPoint.TryParse(ipString, out IPEndPoint endpoint))
            {
                return endpoint.Address.ToString();
            }
            else
            {
                var splitList = ipString.Split(':');
                if (splitList.Length > 2)
                {
                    ipString = IPAddress.Parse(ipString).ToString();
                }
                else if (splitList.Length == 2)
                {
                    ipString = splitList[0];
                }
                else
                {
                    //throw new ParseException("No port separator found", 0);
                }

                return ipString;
            }
        }
    }
}
