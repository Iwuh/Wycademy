using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading;

namespace KiranicoScraper
{
    class WebRequester : IDisposable
    {
        private HttpClient _client;
        private DateTime _lastRequest;
        private ILogger _logger;

        public WebRequester(ILogger logger)
        {
            _client = new HttpClient();
            _lastRequest = new DateTime(0);
            _logger = logger;
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        public JToken GetJson(string url, string start, string end, bool shouldIncludeEndLength = true)
        {
            // Get the full page.
            var page = GetPage(url);

            // Find the start index according to the provided substring.
            var startIndex = page.IndexOf(start);
            // Find the end index according to the provided substring, adding the length of the substring because IndexOf returns the index of the substring's first character, unless we're told not to add the length.
            var endIndex = page.IndexOf(end, startIndex);
            if (shouldIncludeEndLength)
            {
                endIndex += end.Length;
            }

            // Get the proper substring, parse it to JSON, and return it.
            return JToken.Parse(page.Substring(startIndex, endIndex - startIndex));
        }

        public HtmlDocument GetHtml(string url)
        {
            // Get the page as a string.
            var page = GetPage(url);

            // Parse it into a HAP document and return it.
            var document = new HtmlDocument();
            document.LoadHtml(page);
            return document;
        }

        private string GetPage(string url)
        {
            SleepIfNecessary();

            _logger.LogDebug($"GET {url}");
            // Get the requested page synchronously.
            var page = _client.GetStringAsync(url).Result;
            // Update the time of the last request.
            _lastRequest = DateTime.Now;
            return page;
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
