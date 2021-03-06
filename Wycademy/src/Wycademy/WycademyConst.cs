﻿using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wycademy
{
    public static class WycademyConst
    {
        public const string INVALID_MONSTER_NAME = " is not a valid monster name.";
        public const string INVALID_MV_WEAPON_NAME = " is not a recognized weapon name.";
        public const string INVALID_DC_SHARPNESS_NAME = " is not a recognized sharpness type. Check the help for the command.";
        public const string INVALID_DC_WEAPON_NAME = " is not a recognised weapon type. Check the help for the command.";
        public const string DC_WEAPON_TYPE_NOT_SUPPORTED = "Sorry, but that weapon type is not supported yet. It will be, sometime in the near future. Thank you for your understanding.";

#if DEBUG
        public const string DATA_LOCATION = @"D:\Documents\Visual Studio 2017\Projects\Git\Wycademy\Wycademy\src\Wycademy\Data";
#else
        public const string DATA_LOCATION = @"/home/matthew/release/Data";
#endif

        public const string HELP_REACTION = "🏛";

        public const string AVATAR_URL = "https://cdn.discordapp.com/avatars/207172354101608448/67bb079bde2e9ed142ad824e4a31d5af.png";
        public const string GITHUB = "https://github.com/Iwuh/Wycademy";
    }
}
