using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KiranicoScraper.Scrapers.Lists
{
    class ScraperList4U : ScraperListBase
    {
        /// <summary>
        /// The subtitles for monsters' alternate hitzone names. (ex. broken, enraged, etc...)
        /// </summary>
        [JsonProperty("alt_hitzone_names")]
        public Dictionary<string, string> AltHitzones { get; set; }

        /// <summary>
        /// Alternate hitzone subtitles for monsters that have more than one.
        /// </summary>
        [JsonProperty("special_alt_hitzone_names")]
        public Dictionary<string, List<string>> DoubleAltHitzones { get; set; }

        /// <summary>
        /// The list of hitzone keys used in the Kiranico json.
        /// </summary>
        [JsonProperty("hitzone_keys")]
        public List<string> HitzoneKeys { get; set; }

        /// <summary>
        /// The list of status keys used in the Kiranico json.
        /// </summary>
        [JsonProperty("status_keys")]
        public List<string> StatusKeys { get; set; }

        /// <summary>
        /// The list of item effect keys used in the Kiranico json.
        /// </summary>
        [JsonProperty("item_effect_keys")]
        public List<string> ItemEffectKeys { get; set; }

        /// <summary>
        /// Relates the Kiranico json key for a shot to its properly formatted name.
        /// </summary>
        [JsonProperty("gun_shots")]
        public Dictionary<string, string> GunShots { get; set; }

        /// <summary>
        /// The list of sharpness keys used in the Kiranico json.
        /// </summary>
        [JsonProperty("sharpness_keys")]
        public List<string> SharpnessKeys { get; set; }

        /// <summary>
        /// The list of gun stat (recoil, reload, deviation) keys used in the Kiranico json.
        /// </summary>
        [JsonProperty("gun_stat_keys")]
        public List<string> GunStatKeys { get; set; }
    }
}
