using Newtonsoft.Json.Linq;
using System.IO;

namespace KiranicoScraper.Scrapers.Lists
{
    static class ScraperListCollection
    {
        static ScraperListCollection()
        {
            var lists = JObject.Parse(File.ReadAllText(Path.Combine(".", "Data", "ScraperLists.json")));

            FourUltimate = lists["4u"].ToObject<ScraperList4U>();
            Generations = lists["gen"].ToObject<ScraperListGen>();
            World = lists["world"].ToObject<ScraperListWorld>();
        }

        /// <summary>
        /// Lists relating to the scraping of 4U data.
        /// </summary>
        public static ScraperList4U FourUltimate { get; private set; }

        /// <summary>
        /// Lists relating to the scraping of Generations data.
        /// </summary>
        public static ScraperListGen Generations { get; private set; }

        /// <summary>
        /// Lists relating to the scraping of World data.
        /// </summary>
        public static ScraperListWorld World { get; private set; }
    }
}
