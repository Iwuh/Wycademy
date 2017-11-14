using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WycademyV2.Commands.Entities;
using WycademyV2.Commands.Preconditions;
using WycademyV2.Commands.Services;

namespace WycademyV2.Commands.Modules
{
    [RequireUnlocked]
    [Group("weaponinfo")]
    [RequireBotPermission(ChannelPermission.EmbedLinks)]
    public class WeaponInfoModule : ModuleBase<SocketCommandContext>
    {
        private WeaponInfoService _weaponInfo;
        private CommandCacheService _cache;
        private ReactionMenuService _reactionMenu;

        public WeaponInfoModule(WeaponInfoService weaponInfo, CommandCacheService cache, ReactionMenuService reactionMenu)
        {
            _weaponInfo = weaponInfo;
            _cache = cache;
            _reactionMenu = reactionMenu;
        }

        [Command("4u")]
        [Summary("Gets information about a 4U weapon.")]
        public async Task GetFourWeaponData([Remainder, Summary("All or part of the weapon's name.")] string weaponName)
        {
            var results = _weaponInfo.SearchFour(weaponName);
            if (results.Count() == 0)
            {
                await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, $"No weapons were found matching the string `{weaponName}`.");
                return;
            }
            else if (results.Count() > 1)
            {
                await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, $"Several matches were found:\n{Format.Code(string.Join("\n", results.Select(r => r.Name)))}");
                return;
            }
            else
            {
                var embed = _weaponInfo.Build(results.First(), Context.User);
                var message = await ReplyAsync("Weapon Info", embed: embed.First());
                _cache.Add(Context.Message.Id, message.Id);
            }
        }

        [Command("gen", RunMode = RunMode.Async)]
        public async Task GetGenWeaponData([Remainder, Summary("All or part of the weapon's name. Can optionally be followed by a pipe and number to specify a starting level.")] string weaponName)
        {
            // Allows the user to specify a starting level.
            var split = weaponName.Split('|');
            int startLevel = 0;
            if (split.Length > 1)
            {
                int.TryParse(split[1], out startLevel);
            }

            var results = _weaponInfo.SearchGen(split[0]);
            if (results.Count() == 0)
            {
                await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, $"No weapons were found matching the string `{split[0]}`.");
                return;
            }
            else if (results.Count() > 1)
            {
                await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, $"Several matches were found:\n{Format.Code(string.Join("\n", results.Select(r => r.Name)))}");
                return;
            }
            else
            {
                var embeds = _weaponInfo.Build(results.First(), Context.User);
                var message = new WeaponInfoMessage(Context.User, embeds);
                await _reactionMenu.SendReactionMenuMessageAsync(Context.Channel, message);
            }
        }

    }
}
