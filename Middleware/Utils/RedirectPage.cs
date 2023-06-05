using DeviceDetectorNET;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System.Threading.Tasks;

namespace Middleware.Utils
{
    public class RedirectPage
    {
        private readonly RequestDelegate _next;

        public RedirectPage(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string userAgent = context.Request.Headers["User-Agent"].ToString();
            var userAgentParser = new DeviceDetector(userAgent);
            userAgentParser.Parse();

            string browserName = userAgentParser.GetClient().Match.Name;
            if (IsSupportedBrowser(browserName))
            {
                string redirectUrl = GetRedirectUrl(browserName);
                if (!string.IsNullOrEmpty(redirectUrl))
                {
                    context.Response.Redirect(redirectUrl);
                    return;
                }
            }

            await _next(context);
        }

        private bool IsSupportedBrowser(string browserName)
        {
            string[] supportedBrowsers = { "Microsoft Edge", "EdgeChromium", "IE"};
            return supportedBrowsers.Contains(browserName);
        }

        private string GetRedirectUrl(string browserName)
        {
            if (browserName == "Microsoft Edge" || browserName == "EdgeChromium" || browserName == "IE")
            {
                return "https://www.mozilla.org/pl/firefox/new/";
            }
            return string.Empty;
        }
    }
}
