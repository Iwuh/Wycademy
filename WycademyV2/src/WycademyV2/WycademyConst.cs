using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WycademyV2
{
    public static class WycademyConst
    {
        public const string INVALID_MONSTER_NAME = " is not a valid monster name. Try `<monsterlist` for a list of recognised names.";
        public const string INVALID_WEAPON_NAME = " is not a recognized weapon name. Try `<weaponlist` for a list of names.";

        public static readonly DateTime START_TIME;

        static WycademyConst()
        {
            START_TIME = DateTime.Now;
        }
    }
}
