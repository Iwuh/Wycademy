using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WycademyV2.Commands.Enums;

namespace WycademyV2.Commands.Entities
{
    public class WeaponShot
    {
        public GunShotType ShotType { get; private set; }

        /// <summary>
        /// The magazine size for the bullet.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Whether the bullet is enabled by default.
        /// </summary>
        public bool Enabled { get; private set; }

        public WeaponShot(GunShotType type, int count, bool enabled)
        {
            ShotType = type;
            Count = count;
            Enabled = enabled;
        }

        public override string ToString()
        {
            return $"{ShotType} ({Count})";
        }
    }
}
