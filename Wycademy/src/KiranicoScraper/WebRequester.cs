using HtmlAgilityPack;
using KiranicoScraper.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading;
using Wycademy.Core.Models;

namespace KiranicoScraper
{
    class WebRequester : IDisposable
    {
        private readonly HttpClient _client;
        private DateTime _lastRequest;

        private readonly IServiceProvider _provider;
        private readonly ILogger<WebRequester> _logger;

        public WebRequester(IServiceProvider provider)
        {
            _client = new HttpClient();
            _lastRequest = new DateTime(0);

            _provider = provider;
            _logger = _provider.GetRequiredService<ILogger<WebRequester>>();
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        public WebResponse GetPage(string url)
        {
            SleepIfNecessary();

            _logger.LogDebug($"GET {url}");
            // Get the requested page synchronously.
            var page = _client.GetStringAsync(url).Result;
            // Update the time of the last request.
            _lastRequest = DateTime.Now;

            // Create a new scope for this request. Normally scopes are disposed through a using block, but in this case the scope will be disposed when the response is disposed.
            IServiceScope scope = _provider.CreateScope();
            return new WebResponse(page, scope);
        }

        private void SleepIfNecessary()
        {
            // Calculate the number of milliseconds since the last request.
            double difference = (DateTime.Now - _lastRequest).TotalMilliseconds;
            // If less than 2 seconds, sleep until 2 seconds have passed.
            if (difference < 2000)
            {
                var timeToSleep = (int)(2000 - difference);
                _logger.LogTrace($"Sleeping for {timeToSleep}ms");
                Thread.Sleep(timeToSleep);
            }
        }
    }
}
