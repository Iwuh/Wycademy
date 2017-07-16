﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WycademyV2.Commands.Entities;
using WycademyV2.Commands.Enums;
using WycademyV2.Commands.Models;
using WycademyV2.Commands.Preconditions;
using WycademyV2.Commands.Services;

namespace WycademyV2.Commands.Modules
{
    public class MonsterInfoModule : ModuleBase<SocketCommandContext>
    {
        private MonsterInfoService _monster;
        private CommandCacheService _cache;
        private ReactionMenuService _reaction;
        private MonsterContext _db;
        private DiscordSocketClient _client;

        public MonsterInfoModule(MonsterInfoService monster, CommandCacheService cache, ReactionMenuService reaction, MonsterContext context, DiscordSocketClient client)
        {
            _monster = monster;
            _cache = cache;
            _reaction = reaction;
            _db = context;
            _client = client;
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

        [Command("monsterlist")]
        [Summary("Provides a list of all monster names that are recognised by the bot.")]
        [RequireUnlocked]
        public async Task GetMonsterList()
        {
            await Context.User.SendMessageAsync(_monster.GetMonsterNames());
        }

        private async Task GetInfo(MonsterDataCategory category, string monstername, Expression<Func<Monster, IEnumerable<IMonsterData>>> getValues)
        {
            string lowerMonsterName = string.Join("-", monstername.ToLower().Split(' ', '_'));

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
                    var menu = new MonsterInfoMessage(Context.User, tables, _client);
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
