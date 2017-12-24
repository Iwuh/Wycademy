using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WycademyV2.Commands.Entities
{
    public class GunStats
    {
        /// <summary>
        /// The reload speed of the gun.
        /// </summary>
        [JsonProperty]
        public string ReloadSpeed { get; set; }

        /// <summary>
        /// The recoil level of the gun.
        /// </summary>
        [JsonProperty]
        public string Recoil { get; set; }

        /// <summary>
        /// The deviation of the gun and its severity.
        /// </summary>
        [JsonProperty]
        public string Deviation { get; set; }

        /// <summary>
        /// The shots that can be loaded by the gun.
        /// </summary>
        [JsonProperty("shots")]
        public List<GunShot> UsableShots { get; set; }

        /// <summary>
        /// The gun's internal shots, or null for 4U guns.
        /// </summary>
        [JsonProperty]
        public List<GunInternalShot> InternalShots { get; set; }

        /// <summary>
        /// The gun's rapid fire shots, or null if the weapon is not a LBG.
        /// </summary>
        [JsonProperty]
        public List<GunRapidFireShot> RapidFireShots { get; set; }

        /// <summary>
        /// The gun's crouching fire shots, or null if the weapon is not a HBG.
        /// </summary>
        [JsonProperty]
        public List<GunCrouchingFireShot> CrouchingFireShots { get; set; }
    }

    public class GunShot
    {
        /// <summary>
        /// The name of the shot.
        /// </summary>
        [JsonProperty]
        public string Name { get; set; }

        /// <summary>
        /// The clip size of the shot.
        /// </summary>
        [JsonProperty]
        public string Capacity { get; set; }
    }

    public class GunInternalShot
    {
        /// <summary>
        /// The name of the internal shot.
        /// </summary>
        [JsonProperty]
        public string Name { get; set; }

        /// <summary>
        /// The total carried amount of the internal shot.
        /// </summary>
        [JsonProperty]
        public string Total { get; set; }

        /// <summary>
        /// The clip size of the internal shot.
        /// </summary>
        [JsonProperty("clip_size")]
        public string ClipSize { get; set; }
    }

    public class GunRapidFireShot
    {
        /// <summary>
        /// The shot name.
        /// </summary>
        [JsonProperty]
        public string Name { get; set; }
        
        /// <summary>
        /// The number of shots fired in a single volley.
        /// </summary>
        [JsonProperty]
        public string Count { get; set; }

        /// <summary>
        /// The damage multiplier applied to each shot in a volley, or null for 4U weapons.
        /// </summary>
        [JsonProperty]
        public string Multiplier { get; set; }

        /// <summary>
        /// The length of the delay between volleys, or null for 4U weapons.
        /// </summary>
        [JsonProperty("wait_time")]
        public string WaitTime { get; set; }
    }

    public class GunCrouchingFireShot
    {
        /// <summary>
        /// The name of the shot.
        /// </summary>
        [JsonProperty]
        public string Name { get; set; }

        /// <summary>
        /// The number of shots that can be fired in one crouching fire.
        /// </summary>
        [JsonProperty]
        public string Count { get; set; }
    }
}
