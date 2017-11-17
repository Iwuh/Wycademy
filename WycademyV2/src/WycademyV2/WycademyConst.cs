using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WycademyV2
{
    public static class WycademyConst
    {
        public const string INVALID_MONSTER_NAME = " is not a valid monster name.";
        public const string INVALID_MV_WEAPON_NAME = " is not a recognized weapon name.";
        public const string INVALID_DC_SHARPNESS_NAME = " is not a recognized sharpness type. Check the help for the command.";
        public const string INVALID_DC_WEAPON_NAME = " is not a recognised weapon type. Check the help for the command.";
        public const string DC_WEAPON_TYPE_NOT_SUPPORTED = "Sorry, but that weapon type is not supported yet. It will be, sometime in the near future. Thank you for your understanding.";

#if DEBUG
        public const string DATA_LOCATION = @"D:\Documents\Visual Studio 2017\Projects\Git\Wycademy\WycademyV2\src\WycademyV2\Data";
#else
        public const string DATA_LOCATION = @"/home/ubuntu/release/Data";
#endif

        public const string HELP_REACTION = "🏛";

        /// <summary>
        /// Gets the console color for a log severity.
        /// </summary>
        /// <param name="severity">The log message's severity.</param>
        /// <returns>A <see cref="ConsoleColor"/>.</returns>
        public static ConsoleColor GetConsoleColor(LogSeverity severity)
        {
            switch (severity)
            {
                case LogSeverity.Critical:
                    return ConsoleColor.DarkRed;
                case LogSeverity.Error:
                    return ConsoleColor.Red;
                case LogSeverity.Warning:
                    return ConsoleColor.Yellow;
                case LogSeverity.Info:
                    return ConsoleColor.White;
                case LogSeverity.Verbose:
                    return ConsoleColor.Gray;
                case LogSeverity.Debug:
                    return ConsoleColor.DarkGray;
                default:
                    return ConsoleColor.White;
            }
        }
    }
}
