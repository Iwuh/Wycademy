using Discord.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WycademyV2.Commands.Preconditions;
using WycademyV2.Commands.Services;

namespace WycademyV2.Commands.Modules
{
    public class MonHunModule : ModuleBase
    {
        private MonsterInfoService _monhun;
        private LockerService _locker;

        public MonHunModule(MonsterInfoService mis, LockerService ls)
        {
            _monhun = mis;
            _locker = ls;
        }

        [Command("hitzone")]
        [Alias("hitzones")]
        [Summary("Gets the hitzone data for the specified monster.")]
        [RequireUnlocked]
        public async Task GetHitzoneData([Remainder, Summary("The monster to search for.")] string monster)
        {
            try
            {
                string table = await _monhun.GetMonsterInfo("Hitzone", monster);
                await ReplyAsync(table);
            }
            catch (FileNotFoundException)
            {
                await ReplyAsync(monster + WycademyConst.INVALID_MONSTER_NAME);
            }
        }

        [Command("status")]
        [Summary("Gets the status effect data for the specified monster.")]
        [RequireUnlocked]
        public async Task GetStatusData([Remainder, Summary("The monster to search for.")] string monster)
        {
            try
            {
                string table = await _monhun.GetMonsterInfo("Status", monster);
                await ReplyAsync(table);
            }
            catch (FileNotFoundException)
            {
                await ReplyAsync(monster + WycademyConst.INVALID_MONSTER_NAME);
            }
        }

        [Command("staggersever")]
        [Alias("stagger", "sever", "break", "extract")]
        [Summary("Gets the stagger/sever/extract colour data for the specified monster.")]
        [RequireUnlocked]
        public async Task GetStaggerSeverData([Remainder, Summary("The monster to search for.")] string monster)
        {
            try
            {
                string table = await _monhun.GetMonsterInfo("Stagger/Sever", monster);
                await ReplyAsync(table);
            }
            catch (FileNotFoundException)
            {
                await ReplyAsync(monster + WycademyConst.INVALID_MONSTER_NAME);
            }
        }

        [Command("itemeffects")]
        [Alias("item", "items", "trap", "traps")]
        [Summary("Gets the item effect data for the specified monster.")]
        [RequireUnlocked]
        public async Task GetItemData([Remainder, Summary("The monster to search for.")] string monster)
        {
            try
            {
                string table = await _monhun.GetMonsterInfo("Item Effects", monster);
                await ReplyAsync(table);
            }
            catch (FileNotFoundException)
            {
                await ReplyAsync(monster + WycademyConst.INVALID_MONSTER_NAME);
            }
        }

        [Command("monsterlist")]
        [Summary("Provides a list of all monster names that are recognised by the bot.")]
        [RequireUnlocked]
        public async Task GetMonsterList()
        {
            var dm = await Context.User.GetDMChannelAsync();
            if (dm == null)
            {
                dm = await Context.User.CreateDMChannelAsync();
            }
            await Task.Delay(1000);
            await dm.SendMessageAsync("```\n" + string.Join("\n", WycademyConst.MONSTER_LIST) + "```");
        }
    }
}
