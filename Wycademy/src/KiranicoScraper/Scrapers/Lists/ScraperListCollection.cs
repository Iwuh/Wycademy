using KiranicoScraper.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KiranicoScraper.Scrapers.Lists
{
    class ScraperListCollection
    {
        /// <summary>
        /// Lists relating to the scraping of 4U data.
        /// </summary>
        [JsonProperty("4u")]
        public ScraperList4U FourUltimate { get; set; }

        /// <summary>
        /// Lists relating to the scraping of Generations data.
        /// </summary>
        [JsonProperty("gen")]
        public ScraperListGen Generations { get; set; }

        /// <summary>
        /// Lists relating to the scraping of World data.
        /// </summary>
        [JsonProperty("world")]
        public ScraperListWorld World { get; set; }
    }
}
