using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KiranicoScraper.Scrapers.Lists
{
    /// <summary>
    /// Represents lists common to all games.
    /// </summary>
    abstract class ScraperListBase
    {
        /// <summary>
        /// The list of all monsters in a game.
        /// </summary>
        [JsonProperty("monsters")]
        public List<string> Monsters { get; set; }
        /// <summary>
        /// The list of all weapon types in a game.
        /// </summary>
        [JsonProperty("weapons")]
        public List<string> Weapons { get; set; }
    }
}
