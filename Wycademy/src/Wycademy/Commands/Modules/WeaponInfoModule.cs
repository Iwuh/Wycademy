using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Wycademy.Commands.Entities;
using Wycademy.Commands.Preconditions;
using Wycademy.Commands.Services;

namespace Wycademy.Commands.Modules
{
    [RequireUnlocked]
    [Group("weaponinfo")]
    [Summary("MH Weapon Info")]
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
        [Priority(1)]
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
                var message = await ReplyAsync(string.Empty, embed: embed.First());
                _cache.Add(Context.Message.Id, message.Id);
            }
        }

        [Command("gen", RunMode = RunMode.Async)]
        [Priority(1)]
        [Summary("Gets information about a Gen weapon.")]
        public async Task GetGenWeaponData([Remainder, Summary("All or part of the weapon's name. Can optionally be followed by a pipe and number to specify a starting level.")] string weaponName)
        {
            // Extract the weapon name and optionally a starting level.
            Match match = Regex.Match(weaponName, @"([^|]+)\|?(\d+)?");
            // If the input string could not be matched, return an error message.
            if (!match.Success)
            {
                await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, "The input could not be parsed.");
                return;
            }

            // Set the query to the first capture group and trim leading and trailing whitespace.
            string query = match.Groups[1].Value.Trim();
            int startIndex = 0;
            // More than two groups means the user specified a starting level, as the first group is the entire match.
            if (match.Groups.Count > 2)
            {
                // If the starting level is a valid number and is greater than 0...
                if (int.TryParse(match.Groups[2].Value.Trim(), out startIndex) && startIndex > 0)
                {
                    // Subtract one to convert the level to an index. Ex: Level 1 = index 0.
                    startIndex--;
                }
                else
                {
                    // Otherwise the number was 0 or negative so the starting index should just be 0.
                    startIndex = 0;
                }
            }

            var results = _weaponInfo.SearchGen(query);
            if (results.Count() == 0)
            {
                await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, $"No weapons were found matching the string `{query}`.");
                return;
            }
            else if (results.Count() > 1)
            {
                await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, $"Several matches were found:\n{Format.Code(string.Join("\n", results.Select(r => $"{r.Name} / {r.FinalName}")))}");
                return;
            }
            else
            {
                var embeds = _weaponInfo.Build(results.First(), Context.User);
                var message = new WeaponInfoMessage(Context.User, embeds, startIndex);
                await _reactionMenu.SendReactionMenuMessageAsync(Context.Channel, message);
            }
        }

        [Command]
        [Priority(0)]
        [Summary("Fall-through for when a user doesn't specify a game.")]
        [Remarks("default")]
        public async Task Default([Remainder, Summary("This input is discarded as the command simply returns an error message.")] string input)
            => await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, "Please specify a game (`<weaponinfo 4u` or `<weaponinfo gen`).");
    }
}
