using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public WeaponInfoModule(WeaponInfoService weaponInfo, CommandCacheService cache)
        {
            _weaponInfo = weaponInfo;
            _cache = cache;
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
                var embed = _weaponInfo.Build(results.First());
                var message = await ReplyAsync("Weapon Info", embed: embed.First());
                _cache.Add(Context.Message.Id, message.Id);
            }
        }
    }
}
