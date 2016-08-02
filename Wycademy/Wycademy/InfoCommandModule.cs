using Discord;
using Discord.Commands;
using Discord.Commands.Permissions.Levels;
using Discord.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wycademy
{
    class InfoCommandModule : IModule
    {
        private DiscordClient _client;
        private ModuleManager _manager;

        void IModule.Install(ModuleManager manager)
        {
            _manager = manager;
            _client = manager.Client;

            manager.CreateCommands("", cgb =>
            {
                cgb.CreateGroup("info", info =>
                {
                    info.CreateCommand("hitzone")
                    .Alias("hitzones")
                    .MinPermissions((int)PermissionLevels.User)
                    .Description("Provides hitzone information about a monster.")
                    .Parameter("Monster", ParameterType.Required)
                    .Do(async e =>
                    {
                        if (!Program.locked)
                        {
                            try
                            {
                                MonsterInfo hitzoneInfo = await Task.Run(() => MonsterInfoBuilder.GetMonsterInfo(e.GetArg("Monster"), "Hitzones"));

                                // Get the length of the longest category
                                int columnTitleWidth = (WycademyConst.HITZONE_COLUMN_NAMES.Max(x => x.Length));
                                // Get the length of the longest hitzone
                                int rowTitleWidth = hitzoneInfo.Data.Keys.Max(x => x.Length);

                                StringBuilder sb = new StringBuilder();
                                sb.AppendLine($"{hitzoneInfo.Category} info about {e.GetArg("Monster")}:");
                                sb.AppendLine("```");
                                // Add blank spaces in the upper-left corner.
                                sb.Append(' ', rowTitleWidth);
                                // Add column titles followed by a newline
                                foreach (var title in WycademyConst.HITZONE_COLUMN_NAMES)
                                {
                                    sb.Append($"| {PadCenter(title, columnTitleWidth)}");
                                }
                                sb.AppendLine();
                                // Now add a new line for each hitzone and it's values
                                foreach (var hitzone in hitzoneInfo.Data.Keys)
                                {
                                    sb.Append(hitzone + new string(' ', rowTitleWidth - hitzone.Length));
                                    // I used to just use a foreach but I'm no longer using the last 2 elements in the array to save space
                                    // and I am _not_ going through 71 sheets and editing the numbers. I'll just use a regular for loop.
                                    for (int i = 0; i < 8; i++)
                                    {
                                        sb.Append($"| {PadCenter(hitzoneInfo.Data[hitzone][i], columnTitleWidth)}");
                                    }
                                    sb.AppendLine();
                                }
                                sb.AppendLine("```");

                                await e.Channel.SendMessage(sb.ToString());
                            }
                            catch (FileNotFoundException)
                            {
                                await e.Channel.SendMessage($"'{e.GetArg("Monster")}' {WycademyConst.INVALID_MONSTER_NAME}");
                            }
                        }
                    });
                    info.CreateCommand("stagger")
                    .Alias("sever", "break", "extract")
                    .MinPermissions((int)PermissionLevels.User)
                    .Description("Provides stagger, sever, and extract colour information about a monster. Aliases are 'sever', 'break', and 'extract'.")
                    .Parameter("Monster", ParameterType.Required)
                    .Do(async e =>
                    {
                        if (!Program.locked)
                        {
                            try
                            {
                                MonsterInfo staggerInfo = await Task.Run(() => MonsterInfoBuilder.GetMonsterInfo(e.GetArg("Monster"), "Stagger/Sever"));

                                // Get the length of the longest category
                                int columnTitleWidth = (WycademyConst.STAGGER_COLUMN_NAMES.Max(x => x.Length));
                                // Get the length of the longest body part name
                                int rowTitleWidth = staggerInfo.Data.Keys.Max(x => x.Length);

                                StringBuilder sb = new StringBuilder();
                                sb.AppendLine($"{staggerInfo.Category} info about {e.GetArg("Monster")}:");
                                sb.AppendLine("```");
                                // Add blank spaces in the upper-left corner.
                                sb.Append(' ', rowTitleWidth);
                                // Add column titles followed by a newline
                                foreach (var title in WycademyConst.STAGGER_COLUMN_NAMES)
                                {
                                    sb.Append($"| {PadCenter(title, columnTitleWidth)}");
                                }
                                sb.AppendLine();
                                // Now add a new line for each hitzone and it's values
                                foreach (var bodypart in staggerInfo.Data.Keys)
                                {
                                    sb.Append(bodypart + new string(' ', rowTitleWidth - bodypart.Length));
                                    foreach (var value in staggerInfo.Data[bodypart])
                                    {
                                        sb.Append($"| {PadCenter(value, columnTitleWidth)}");
                                    }
                                    sb.AppendLine();
                                }
                                sb.AppendLine("```");

                                await e.Channel.SendMessage(sb.ToString());
                            }
                            catch (FileNotFoundException)
                            {
                                await e.Channel.SendMessage($"'{e.GetArg("Monster")}' {WycademyConst.INVALID_MONSTER_NAME}");
                            }
                        }
                    });
                    info.CreateCommand("status")
                    .MinPermissions((int)PermissionLevels.User)
                    .Description("Provides status information about a monster.")
                    .Parameter("Monster", ParameterType.Required)
                    .Do(async e =>
                    {
                        if (!Program.locked)
                        {
                            try
                            {
                                MonsterInfo statusInfo = await Task.Run(() => MonsterInfoBuilder.GetMonsterInfo(e.GetArg("Monster"), "Status"));

                                // Get the length of the longest category
                                int columnTitleWidth = (WycademyConst.STATUS_COLUMN_NAMES.Max(x => x.Length));
                                // Get the length of the longest status
                                int rowTitleWidth = statusInfo.Data.Keys.Max(x => x.Length);

                                StringBuilder sb = new StringBuilder();
                                sb.AppendLine($"{statusInfo.Category} info about {e.GetArg("Monster")}:");
                                sb.AppendLine("```");
                                // Add blank spaces in the upper-left corner.
                                sb.Append(' ', rowTitleWidth);
                                // Add column titles followed by a newline
                                foreach (var title in WycademyConst.STATUS_COLUMN_NAMES)
                                {
                                    sb.Append($"| {PadCenter(title, columnTitleWidth)}");
                                }
                                sb.AppendLine();
                                // Now add a new line for each hitzone and it's values
                                foreach (var bodypart in statusInfo.Data.Keys)
                                {
                                    sb.Append(bodypart + new string(' ', rowTitleWidth - bodypart.Length));
                                    foreach (var value in statusInfo.Data[bodypart])
                                    {
                                        sb.Append($"| {PadCenter(value, columnTitleWidth)}");
                                    }
                                    sb.AppendLine();
                                }
                                sb.AppendLine("```");

                                await e.Channel.SendMessage(sb.ToString());
                            }
                            catch (FileNotFoundException)
                            {
                                await e.Channel.SendMessage($"'{e.GetArg("Monster")}' {WycademyConst.INVALID_MONSTER_NAME}");
                            }
                        }
                    });
                    info.CreateCommand("trap")
                    .Alias("item")
                    .MinPermissions((int)PermissionLevels.User)
                    .Description("Provides flash bomb and trap information about a monster. Alias is 'item'.")
                    .Parameter("Monster", ParameterType.Required)
                    .Do(async e =>
                    {
                        if (!Program.locked)
                        {
                            try
                            {
                                MonsterInfo itemInfo = await Task.Run(() => MonsterInfoBuilder.GetMonsterInfo(e.GetArg("Monster"), "Item Effects"));

                                // Get the length of the longest category
                                int columnTitleWidth = (WycademyConst.ITEMEFFECTS_COLUMN_NAMES.Max(x => x.Length));
                                // Get the length of the longest status
                                int rowTitleWidth = itemInfo.Data.Keys.Max(x => x.Length);

                                StringBuilder sb = new StringBuilder();
                                sb.AppendLine($"{itemInfo.Category} info about {e.GetArg("Monster")}:");
                                sb.AppendLine("```");
                                // Add blank spaces in the upper-left corner.
                                sb.Append(' ', rowTitleWidth);
                                // Add column titles followed by a newline
                                foreach (var title in WycademyConst.ITEMEFFECTS_COLUMN_NAMES)
                                {
                                    sb.Append($"| {PadCenter(title, columnTitleWidth)}");
                                }
                                sb.AppendLine();
                                // Now add a new line for each hitzone and it's values
                                foreach (var bodypart in itemInfo.Data.Keys)
                                {
                                    sb.Append(bodypart + new string(' ', rowTitleWidth - bodypart.Length));
                                    foreach (var value in itemInfo.Data[bodypart])
                                    {
                                        sb.Append($"| {PadCenter(value, columnTitleWidth)}");
                                    }
                                    sb.AppendLine();
                                }
                                sb.AppendLine("```");

                                await e.Channel.SendMessage(sb.ToString());
                            }
                            catch (FileNotFoundException)
                            {
                                await e.Channel.SendMessage($"'{e.GetArg("Monster")}' {WycademyConst.INVALID_MONSTER_NAME}");
                            }
                        }
                    });
                    info.CreateCommand("list")
                    .MinPermissions((int)PermissionLevels.User)
                    .Description("Provides a list of all monsters you can get info from.")
                    .Do(async e =>
                    {
                        if (!Program.locked)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine("Here are all the valid monster names for the info commands.");
                            // 3 backticks indicate a code block in Discord's markdown. Here we use it to wrap the monster names in a block.
                            sb.AppendLine("```");
                            foreach (var monster in WycademyConst.MONSTER_LIST)
                            {
                                sb.AppendLine(monster);
                            }
                            sb.AppendLine("```");
                            await e.User.SendMessage(sb.ToString());
                        }
                    });
                });
            });
        }

        // Centers a string inside a given amount of whitespace.
        public string PadCenter(string original, int totalLength)
        {
            int delta = totalLength - original.Length;
            if (delta == 0)
            {
                // If the string takes up 100% of the whitespace then just return the original string
                return original;
            }
            return original.PadLeft(delta / 2 + original.Length).PadRight(totalLength);
        }
    }
}
