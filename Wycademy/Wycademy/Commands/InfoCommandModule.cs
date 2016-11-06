using Discord;
using Discord.Commands;
using Discord.Commands.Permissions.Levels;
using Discord.Commands.Permissions.Userlist;
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
                    .UseGlobalBlacklist()
                    .Do(async e =>
                    {
                        if (!Program.locked)
                        {
                            try
                            {
                                MonsterInfo hitzoneInfo = await Task.Run(() => MonsterInfoBuilder.GetMonsterInfo(e.GetArg("Monster"), "Hitzones"));

                                Message m = await e.Channel.SendMessageZWSP(GenerateMessage(hitzoneInfo, e.GetArg("Monster")));
                                await Task.Delay(1000);
                                Program.MessageCache.Add(e.Message.Id, m.Id);
                            }
                            catch (ArgumentException)
                            {
                                Message m = await e.Channel.SendMessageZWSP($"'{e.GetArg("Monster")}' {WycademySettings.INVALID_MONSTER_NAME}");
                                await Task.Delay(1000);
                                Program.MessageCache.Add(e.Message.Id, m.Id);
                            }
                        }
                    });
                    info.CreateCommand("stagger")
                    .Alias("sever", "break", "extract")
                    .MinPermissions((int)PermissionLevels.User)
                    .Description("Provides stagger, sever, and extract colour information about a monster. Aliases are 'sever', 'break', and 'extract'.")
                    .Parameter("Monster", ParameterType.Required)
                    .UseGlobalBlacklist()
                    .Do(async e =>
                    {
                        if (!Program.locked)
                        {
                            try
                            {
                                MonsterInfo staggerInfo = await Task.Run(() => MonsterInfoBuilder.GetMonsterInfo(e.GetArg("Monster"), "Stagger/Sever"));

                                Message m = await e.Channel.SendMessageZWSP(GenerateMessage(staggerInfo, e.GetArg("Monster")));
                                await Task.Delay(1000);
                                Program.MessageCache.Add(e.Message.Id, m.Id);
                            }
                            catch (ArgumentException)
                            {
                                Message m = await e.Channel.SendMessageZWSP($"'{e.GetArg("Monster")}' {WycademySettings.INVALID_MONSTER_NAME}");
                                await Task.Delay(1000);
                                Program.MessageCache.Add(e.Message.Id, m.Id);
                            }
                        }
                    });
                    info.CreateCommand("status")
                    .MinPermissions((int)PermissionLevels.User)
                    .Description("Provides status information about a monster.")
                    .Parameter("Monster", ParameterType.Required)
                    .UseGlobalBlacklist()
                    .Do(async e =>
                    {
                        if (!Program.locked)
                        {
                            try
                            {
                                MonsterInfo statusInfo = await Task.Run(() => MonsterInfoBuilder.GetMonsterInfo(e.GetArg("Monster"), "Status"));

                                Message m = await e.Channel.SendMessageZWSP(GenerateMessage(statusInfo, e.GetArg("Monster")));
                                await Task.Delay(1000);
                                Program.MessageCache.Add(e.Message.Id, m.Id);
                            }
                            catch (ArgumentException)
                            {
                                Message m = await e.Channel.SendMessageZWSP($"'{e.GetArg("Monster")}' {WycademySettings.INVALID_MONSTER_NAME}");
                                await Task.Delay(1000);
                                Program.MessageCache.Add(e.Message.Id, m.Id);
                            }
                        }
                    });
                    info.CreateCommand("trap")
                    .Alias("item")
                    .MinPermissions((int)PermissionLevels.User)
                    .Description("Provides flash bomb and trap information about a monster. Alias is 'item'.")
                    .Parameter("Monster", ParameterType.Required)
                    .UseGlobalBlacklist()
                    .Do(async e =>
                    {
                        if (!Program.locked)
                        {
                            try
                            {
                                MonsterInfo itemInfo = await Task.Run(() => MonsterInfoBuilder.GetMonsterInfo(e.GetArg("Monster"), "Item Effects"));

                                Message m = await e.Channel.SendMessageZWSP(GenerateMessage(itemInfo, e.GetArg("Monster")));
                                await Task.Delay(1000);
                                Program.MessageCache.Add(e.Message.Id, m.Id);
                            }
                            catch (ArgumentException)
                            {
                                Message m = await e.Channel.SendMessageZWSP($"'{e.GetArg("Monster")}' {WycademySettings.INVALID_MONSTER_NAME}");
                                await Task.Delay(1000);
                                Program.MessageCache.Add(e.Message.Id, m.Id);
                            }
                        }
                    });
                    info.CreateCommand("monsterlist")
                    .MinPermissions((int)PermissionLevels.User)
                    .Description("Provides a list of all monsters you can get info from.")
                    .UseGlobalBlacklist()
                    .Do(async e =>
                    {
                        if (!Program.locked)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine("Here are all the valid monster names for the info commands.");
                            // 3 backticks indicate a code block in Discord's markdown. Here we use it to wrap the monster names in a block.
                            sb.AppendLine("```");
                            foreach (var monster in WycademySettings.MONSTER_LIST)
                            {
                                sb.AppendLine(monster);
                            }
                            sb.AppendLine("```");
                            Message m = await e.User.SendMessageZWSP(sb.ToString());
                            await Task.Delay(1000);
                            Program.MessageCache.Add(e.Message.Id, m.Id);
                        }
                    });

                    info.CreateCommand("motionvalues")
                    .MinPermissions((int)PermissionLevels.User)
                    .Description("Sends a text file containing all the motion values for a specific weapon type.")
                    .Alias("mv")
                    .UseGlobalBlacklist()
                    .Do(async e =>
                    {
                        if (!Program.locked)
                        {

                        }
                    });

                    info.CreateCommand("help")
                    .MinPermissions((int)PermissionLevels.User)
                    .Description("Points users to the correct help function.")
                    .UseGlobalBlacklist()
                    .Do(async e =>
                    {
                        Message m = await e.Channel.SendMessageZWSP("Try <help [command] instead.");
                        await Task.Delay(1000);
                        Program.MessageCache.Add(e.Message.Id, m.Id);
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

        public string GenerateMessage(MonsterInfo info, string monsterName)
        {
            int columnTitleWidth;
            int rowTitleWidth;
            string[] columnNames;
            switch (info.Category)
            {
                case "Hitzones":
                    // Get the length of the longest category
                    columnTitleWidth = (WycademySettings.HITZONE_COLUMN_NAMES.Max(x => x.Length));
                    // Get the length of the longest hitzone
                    rowTitleWidth = info.Data.Keys.Max(x => x.Length);
                    columnNames = WycademySettings.HITZONE_COLUMN_NAMES;
                    break;
                case "Stagger/Sever":
                    // Get the length of the longest category
                    columnTitleWidth = (WycademySettings.STAGGER_COLUMN_NAMES.Max(x => x.Length));
                    // Get the length of the longest hitzone
                    rowTitleWidth = info.Data.Keys.Max(x => x.Length);
                    columnNames = WycademySettings.STAGGER_COLUMN_NAMES;
                    break;
                case "Status":
                    // Get the length of the longest category
                    columnTitleWidth = (WycademySettings.STATUS_COLUMN_NAMES.Max(x => x.Length));
                    // Get the length of the longest hitzone
                    rowTitleWidth = info.Data.Keys.Max(x => x.Length);
                    columnNames = WycademySettings.STATUS_COLUMN_NAMES;
                    break;
                case "Item Effects":
                    // Get the length of the longest category
                    columnTitleWidth = (WycademySettings.ITEMEFFECTS_COLUMN_NAMES.Max(x => x.Length));
                    // Get the length of the longest hitzone
                    rowTitleWidth = info.Data.Keys.Max(x => x.Length);
                    columnNames = WycademySettings.ITEMEFFECTS_COLUMN_NAMES;
                    break;
                default:
                    // Go with hitzone info if none of the above match
                    // Get the length of the longest category
                    columnTitleWidth = (WycademySettings.HITZONE_COLUMN_NAMES.Max(x => x.Length));
                    // Get the length of the longest hitzone
                    rowTitleWidth = info.Data.Keys.Max(x => x.Length);
                    columnNames = WycademySettings.HITZONE_COLUMN_NAMES;
                    break;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{info.Category} info about {monsterName}:");
            sb.AppendLine("```XL");
            // Add blank spaces in the upper-left corner.
            sb.Append(' ', rowTitleWidth);
            // Add column titles followed by a newline
            foreach (var title in columnNames)
            {
                sb.Append($"| {PadCenter(title, columnTitleWidth)}");
            }
            sb.AppendLine();
            // Now add a new line for each hitzone and it's values
            foreach (var title in info.Data.Keys)
            {
                sb.Append(title + new string(' ', rowTitleWidth - title.Length));

                if (info.Category == "Hitzones")
                {
                    // I used to just use a foreach but I'm no longer using the last 2 elements in the array to save space
                    // and I am _not_ going through 71 sheets and editing the numbers. I'll just use a regular for loop.
                    for (int i = 0; i < 8; i++)
                    {
                        sb.Append($"| {PadCenter(info.Data[title][i], columnTitleWidth)}");
                    }
                    sb.AppendLine();
                }
                else
                {
                    foreach (var value in info.Data[title])
                    {
                        sb.Append($"| {PadCenter(value, columnTitleWidth)}");
                    }
                    sb.AppendLine();
                }
            }
            sb.AppendLine("```");

            return sb.ToString();
        }
    }
}
