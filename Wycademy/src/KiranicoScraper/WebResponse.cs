using HtmlAgilityPack;
using KiranicoScraper.Database;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using Wycademy.Core.Models;

namespace KiranicoScraper
{
    class WebResponse : IDisposable
    {
        private readonly IServiceScope _scope;
        private bool _disposed = false;

        /// <summary>
        /// The contents of the retrieved page.
        /// </summary>
        public string Page { get; }

        public WebResponse(string page, IServiceScope scope)
        {
            Page = page;
            _scope = scope;
        }

        /// <summary>
        /// Returns a slice of the web page, parsed to json.
        /// </summary>
        /// <param name="start">A substring indicating the start of the slice. The start index is the first character of the substring.</param>
        /// <param name="end">A substring indicating the end of the slice.</param>
        /// <param name="shouldIncludeEndLength">Whether the returned slice should end at the first character of <paramref name="end"/>, or the last.</param>
        public JToken GetPageAsJson(string start, string end, bool shouldIncludeEndLength = true)
        {
            // Find the start index according to the provided substring.
            var startIndex = Page.IndexOf(start);
            // Find the end index according to the provided substring, adding the length of the substring because IndexOf returns the index of the substring's first character, unless we're told not to add the length.
            var endIndex = Page.IndexOf(end, startIndex);
            if (shouldIncludeEndLength)
            {
                endIndex += end.Length;
            }

            // Get the proper substring, parse it to JSON, and return it.
            return JToken.Parse(Page.Substring(startIndex, endIndex - startIndex));
        }

        /// <summary>
        /// Returns the entire web page, parsed into a HAP document.
        /// </summary>
        public HtmlDocument GetPageAsHtml()
        {
            // Parse the page into a HAP document and return it.
            var document = new HtmlDocument();
            document.LoadHtml(Page);
            return document;
        }

        /// <summary>
        /// Creates a database builder for <see cref="Monster"/> and all dependent types.
        /// </summary>
        public DbMonsterBuilder CreateMonsterBuilder()
        {
            if (_disposed) throw new ObjectDisposedException("Cannot create DB object builder if response has been disposed.");
            return new DbMonsterBuilder(_scope.ServiceProvider.GetRequiredService<WycademyContext>());
        }

        /// <summary>
        /// Creates a database builder for <see cref="Weapon"/> and all dependent types.
        /// </summary>
        public DbWeaponBuilder CreateWeaponBuilder()
        {
            if (_disposed) throw new ObjectDisposedException("Cannot create DB object builder if response has been disposed.");
            return new DbWeaponBuilder(_scope.ServiceProvider.GetRequiredService<WycademyContext>());
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _scope.Dispose();
                _disposed = true;
            }
            else
            {
                throw new ObjectDisposedException("Response has already been disposed.");
            }
        }
    }
}
