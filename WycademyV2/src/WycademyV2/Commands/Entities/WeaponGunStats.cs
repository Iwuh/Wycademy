using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WycademyV2.Commands.Enums;

namespace WycademyV2.Commands.Entities
{
    public class WeaponGunStats
    {
        /// <summary>
        /// The reload speed of the level.
        /// </summary>
        public GunReloadSpeed ReloadSpeed { get; private set; }

        /// <summary>
        /// The recoil level of the weapon level.
        /// </summary>
        public GunRecoilLevel Recoil { get; private set; }

        /// <summary>
        /// The deviation of the level.
        /// </summary>
        public GunDeviation Deviation { get; private set; }

        public WeaponGunStats(int reload, int recoil, int deviation)
        {
            ReloadSpeed = (GunReloadSpeed)reload;
            Recoil = (GunRecoilLevel)recoil;
            Deviation = (GunDeviation)deviation;
        }
    }
}
