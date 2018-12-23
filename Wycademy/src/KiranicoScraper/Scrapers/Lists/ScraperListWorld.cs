using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KiranicoScraper.Scrapers.Lists
{
    class ScraperListWorld : ScraperListBase
    {
        /// <summary>
        /// Maps the HTML classes used for Kinsect extract colours to their English names.
        /// </summary>
        [JsonProperty("bug_extracts")]
        public Dictionary<string, string> BugExtracts { get; set; }
    }
}
