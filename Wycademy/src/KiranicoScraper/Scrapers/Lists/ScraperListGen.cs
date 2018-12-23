using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KiranicoScraper.Scrapers.Lists
{
    class ScraperListGen : ScraperListBase
    {
        /// <summary>
        /// Maps extract colours used in Kiranico monster pages to their English names.
        /// </summary>
        [JsonProperty("extract_colours")]
        public Dictionary<string, string> ExtractColours { get; set; }

        /// <summary>
        /// Maps gunlance shell type IDs used in the Kiranico json to their English names.
        /// </summary>
        [JsonProperty("gunlance_shell_types")]
        public Dictionary<int, string> GunlanceShellTypes { get; set; }

        /// <summary>
        /// Maps SA/CB phial type IDs used in the Kiranico json to their English names.
        /// </summary>
        [JsonProperty("phial_types")]
        public Dictionary<int, string> PhialTypes { get; set; }
        
        /// <summary>
        /// The sharpness keys used in the Kiranico json. Doesn't include purple, because purple isn't in Gen.
        /// </summary>
        [JsonProperty("sharpness_keys")]
        public List<string> SharpnessKeys { get; set; }

        /// <summary>
        /// Maps bow arc shot type IDs used in the Kiranico json to their English names.
        /// </summary>
        [JsonProperty("arc_shot_types")]
        public Dictionary<int, string> ArcShotTypes { get; set; }

        /// <summary>
        /// Maps bow charge shot type IDs used in the Kiranico json to their English names.
        /// </summary>
        [JsonProperty("charge_shot_types")]
        public Dictionary<int, string> ChargeShotTypes { get; set; }

        /// <summary>
        /// Maps bow coating numbers used in the Kiranico json to their English names.
        /// </summary>
        [JsonProperty("coatings")]
        public Dictionary<int, string> Coatings { get; set; }

        /// <summary>
        /// Maps gun reload speed IDs used in the Kiranico json to their English names.
        /// </summary>
        [JsonProperty("reload_speeds")]
        public Dictionary <int, string> ReloadSpeeds { get; set; }

        /// <summary>
        /// Maps gun recoil level IDs used in the Kiranico json to their English names.
        /// </summary>
        [JsonProperty("recoil_levels")]
        public Dictionary<int, string> RecoilLevels { get; set; }

        /// <summary>
        /// Maps gun deviation level IDs used in the Kiranico json to their English names.
        /// </summary>
        [JsonProperty("deviation_levels")]
        public Dictionary<int, string> DeviationLevels { get; set; }

        /// <summary>
        /// Maps gun shot numbers used in the Kiranico json to their English names.
        /// </summary>
        [JsonProperty("gun_shots")]
        public Dictionary<int, string> GunShots { get; set; }

        /// <summary>
        /// Maps rapid fire wait time IDs used in the Kiranico json to their English names.
        /// </summary>
        [JsonProperty("rapid_fire_wait_times")]
        public Dictionary<int, string> RapidFireWaitTimes { get; set; }
    }
}
