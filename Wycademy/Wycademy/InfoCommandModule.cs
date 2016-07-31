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
                        try
                        {
                            MonsterInfo hitzoneinfo = await Task.Run(() => MonsterInfoBuilder.GetMonsterInfo(e.GetArg("Monster"), "hitzones"));
                        }
                        catch (FileNotFoundException)
                        {
                            await e.Channel.SendMessage($"'{e.GetArg("Monster")}' {WycademyConst.INVALID_MONSTER_NAME}");
                        }
                    });
                    info.CreateCommand("stagger")
                    .Alias("sever", "break", "extract")
                    .MinPermissions((int)PermissionLevels.User)
                    .Description("Provides stagger, sever, and extract colour information about a monster. Aliases are 'sever', 'break', and 'extract'.")
                    .Parameter("Monster", ParameterType.Required)
                    .Do(async e =>
                    {
                        try
                        {
                            MonsterInfo staggerinfo = await Task.Run(() => MonsterInfoBuilder.GetMonsterInfo(e.GetArg("Monster"), "hitzones"));
                        }
                        catch (FileNotFoundException)
                        {
                            await e.Channel.SendMessage($"'{e.GetArg("Monster")}' {WycademyConst.INVALID_MONSTER_NAME}");
                        }
                    });
                    info.CreateCommand("status")
                    .Alias("trap")
                    .MinPermissions((int)PermissionLevels.User)
                    .Description("Provides status, flash bomb, and trap information about a monster. Alias is 'trap'.")
                    .Parameter("Monster", ParameterType.Required)
                    .Do(async e =>
                    {
                        try
                        {
                            MonsterInfo statusinfo = await Task.Run(() => MonsterInfoBuilder.GetMonsterInfo(e.GetArg("Monster"), "hitzones"));
                        }
                        catch (FileNotFoundException)
                        {
                            await e.Channel.SendMessage($"'{e.GetArg("Monster")}' {WycademyConst.INVALID_MONSTER_NAME}");
                        }
                    });
                    info.CreateCommand("list")
                    .MinPermissions((int)PermissionLevels.User)
                    .Description("Provides a list of all monsters you can get info from.")
                    .Do(async e =>
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
                    });
                });
            });
        }
    }
}
