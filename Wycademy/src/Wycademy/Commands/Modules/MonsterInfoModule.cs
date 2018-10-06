using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Wycademy.Commands.Entities;
using Wycademy.Commands.Enums;
using Wycademy.Commands.Models;
using Wycademy.Commands.Preconditions;
using Wycademy.Commands.Services;

namespace Wycademy.Commands.Modules
{
    [Name("monsterinfo")]
    [Summary("MH Monster Info")]
    [RequireBotPermission(ChannelPermission.AddReactions)]
    public class MonsterInfoModule : ModuleBase<SocketCommandContext>
    {
        private MonsterInfoService _monster;
        private CommandCacheService _cache;
        private ReactionMenuService _reaction;
        private MonsterContext _db;

        public MonsterInfoModule(MonsterInfoService monster, CommandCacheService cache, ReactionMenuService reaction, MonsterContext context)
        {
            _monster = monster;
            _cache = cache;
            _reaction = reaction;
            _db = context;
        }

        [Command("hitzone")]
        [Alias("hitzones")]
        [Summary("Gets the hitzone data for the specified monster.")]
        [RequireUnlocked]
        public async Task GetHitzones([Remainder, Summary("The monster to search for.")] string monster)
        {
            await GetInfo(MonsterDataCategory.Hitzone, monster, m => m.Hitzones);
        }

        [Command("status")]
        [Summary("Gets the status effect data for the specified monster.")]
        [RequireUnlocked]
        public async Task GetStatus([Remainder, Summary("The monster to search for.")] string monster)
        {
            await GetInfo(MonsterDataCategory.Status, monster, m => m.Status);
        }

        [Command("stagger")]
        [Alias("sever", "break", "extract")]
        [Summary("Gets the stagger/sever/extract colour data for the specified monster.")]
        [RequireUnlocked]
        public async Task GetStagger([Remainder, Summary("The monster to search for.")] string monster)
        {
            await GetInfo(MonsterDataCategory.Stagger, monster, m => m.Stagger);
        }

        [Command("itemeffects")]
        [Alias("item", "items", "trap", "traps")]
        [Summary("Gets the item effect data for the specified monster.")]
        [RequireUnlocked]
        public async Task GetItems([Remainder, Summary("The monster to search for.")] string monster)
        {
            await GetInfo(MonsterDataCategory.Item, monster, m => m.Items);
        }

        private async Task GetInfo(MonsterDataCategory category, string monstername, Expression<Func<Monster, IEnumerable<IMonsterData>>> getValues)
        {
            string lowerMonsterName = string.Join("-", monstername.ToLower().Split(' ', '_'));
            // Take care of any exceptions.
            switch (lowerMonsterName)
            {
                case "plum-daimyo-hermitaur":
                    lowerMonsterName = "plum-d.hermitaur";
                    break;
                case "dah'ren-mohran":
                    lowerMonsterName = "dahren-mohran";
                    break;
            }

            var noStaggerData = new string[] { "shah-dalamadur", "fatalis", "crimson-fatalis", "white-fatalis" };
            if (category == MonsterDataCategory.Stagger && noStaggerData.Contains(lowerMonsterName))
            {
                await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, "Stagger data is not available for this monster.");
                return;
            }

            try
            {
                Dictionary<string, (string, int?)> tables = await _monster.GetMonsterInfo(category, lowerMonsterName, _db, getValues);

                if (tables.Count == 1)
                {
                    var tuple = tables.Values.First();
                    if (tuple.Item2 == null)
                    {
                        await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, tuple.Item1, prependZWSP: true);
                    }
                    else
                    {
                        await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, tuple.Item1.Substring(0, tuple.Item2.Value), prependZWSP: true);
                        await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, tuple.Item1.Substring(tuple.Item2.Value), prependZWSP: true);
                    }
                }
                else
                {
                    var menu = new MonsterInfoMessage(Context.User, tables, (Context.Guild?.CurrentUser as IUser) ?? Context.Client.CurrentUser as IUser, _cache, Context.Message.Id);
                    var message = await _reaction.SendReactionMenuMessageAsync(Context.Channel, menu);
                    _cache.Add(Context.Message.Id, message.Id);
                }
            }
            catch (InvalidOperationException)
            {
                await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, $"`{monstername}`" + WycademyConst.INVALID_MONSTER_NAME, prependZWSP: true);
            }
        }

        protected override void AfterExecute(CommandInfo command)
        {
            _db.Dispose();
        }
    }
}
