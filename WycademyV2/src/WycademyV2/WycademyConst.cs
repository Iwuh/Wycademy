using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WycademyV2
{
    public static class WycademyConst
    {
        public const string INVALID_MONSTER_NAME = " is not a valid monster name. Try `<monsterlist` for a list of recognised names.";
        public const string INVALID_MV_WEAPON_NAME = " is not a recognized weapon name. Try `<weaponlist` for a list of names.";
        public const string INVALID_DC_SHARPNESS_NAME = " is not a recognized sharpness type. Check the help for the command.";
        public const string INVALID_DC_WEAPON_NAME = " is not a recognised weapon type. Check the help for the command.";
        public const string DC_WEAPON_TYPE_NOT_SUPPORTED = "Sorry, but that weapon type is not supported yet. It will be, sometime in the near future. Thank you for your understanding.";

        public static DateTime START_TIME { get; private set; }

        // Static constructors only run when a class member is accessed for the first time so instead I'm just going to call this method manually.
        public static void InitializeValues()
        {
            START_TIME = DateTime.Now;
        }
    }
}
