using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WycademyV2.Commands.Preconditions;
using WycademyV2.Commands.Services;

namespace WycademyV2.Commands.Modules
{
    public class MonsterInfoModule : ModuleBase
    {
        private MonsterInfoService _monster;
        private CommandCacheService _cache;

        public MonsterInfoModule(MonsterInfoService monster, CommandCacheService cache)
        {
            _monster = monster;
            _cache = cache;
        }

        [Command("hitzone")]
        [Alias("hitzones")]
        [Summary("Gets the hitzone data for the specified monster.")]
        [RequireUnlocked]
        public async Task GetHitzones([Remainder, Summary("The monster to search for.")] string monster)
        {
            await GetInfo("Hitzone", monster);
        }

        [Command("status")]
        [Summary("Gets the status effect data for the specified monster.")]
        [RequireUnlocked]
        public async Task GetStatus([Remainder, Summary("The monster to search for.")] string monster)
        {
            await GetInfo("Status", monster);
        }

        [Command("stagger")]
        [Alias("sever", "break", "extract")]
        [Summary("Gets the stagger/sever/extract colour data for the specified monster.")]
        [RequireUnlocked]
        public async Task GetStagger([Remainder, Summary("The monster to search for.")] string monster)
        {
            await GetInfo("Stagger", monster);
        }

        [Command("itemeffects")]
        [Alias("item", "items", "trap", "traps")]
        [Summary("Gets the item effect data for the specified monster.")]
        [RequireUnlocked]
        public async Task GetItems([Remainder, Summary("The monster to search for.")] string monster)
        {
            await GetInfo("Item Effect", monster);
        }

        [Command("monsterlist")]
        [Summary("Provides a list of all monster names that are recognised by the bot.")]
        [RequireUnlocked]
        public async Task GetMonsterList()
        {
            await Context.User.SendMessageAsync(_monster.GetMonsterNames());
        }

        private async Task GetInfo(string category, string monstername)
        {
            string lowerMonsterName = string.Join("-", monstername.ToLower().Split(' ', '_'));

            string message;
            try
            {
                message = _monster.GetMonsterInfo(category, lowerMonsterName);
            }
            catch (FileNotFoundException)
            {
                message = $"`{monstername}`" + WycademyConst.INVALID_MONSTER_NAME;
            }

            await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, message, prependZWSP: true);
        }
    }
}
