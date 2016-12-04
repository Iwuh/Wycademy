using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WycademyV2.Commands.Preconditions;
using WycademyV2.Commands.Services;

namespace WycademyV2.Commands.Modules
{
    public class StatsModule : ModuleBase
    {
        private LockerService _locker;
        private MonsterInfoService _moninfo;
        private MotionValueService _mv;
        private CommandCacheService _cache;
        private RandomNumberService _rand;

        public StatsModule(LockerService ls, MonsterInfoService mis, MotionValueService mvs, CommandCacheService ccs, RandomNumberService rns)
        {
            _locker = ls;
            _moninfo = mis;
            _mv = mvs;
            _cache = ccs;
            _rand = rns;
        }

        [Command("stats")]
        [Summary("Gets statistics about the bot.")]
        [RequireUnlocked]
        public async Task GetStatistics()
        {
            var appInfo = await Context.Client.GetApplicationInfoAsync();

            using (Process p = Process.GetCurrentProcess())
            {
                var statsEmbed = new EmbedBuilder()
                .WithAuthor(new EmbedAuthorBuilder() { Name = "Wycademy", IconUrl = Context.Client.CurrentUser.AvatarUrl, Url = @"https://i.ytimg.com/vi/IRMFU3ZSvTc/hqdefault.jpg" })
                .WithColor(new Color((byte)_rand.GetRandomNumber(0, 256), (byte)_rand.GetRandomNumber(0, 256), (byte)_rand.GetRandomNumber(0, 256)))
                .WithTitle("Statistics about the Wycademy:")
                .AddField(x => x.WithName("Uptime:").WithValue(GetUptime(p)))
                .AddField(x => x.WithName("Queries:").WithValue($"{_moninfo.Queries + _mv.Queries}"))
                .AddField(x => x.WithName("Cached Messages:").WithValue($"{_cache.Count} / {_cache.MaxCapacity}"))
                .AddField(async x => x.WithName("Connected Servers").WithValue((await Context.Client.GetGuildsAsync()).Count().ToString()))
                .AddField(x => x.WithName("Memory Use:").WithValue(p.PrivateMemorySize64.ToString()));
            }
        }

        private string GetUptime(Process p)
        {
            TimeSpan uptime = DateTime.Now - p.StartTime;

            return uptime.ToString(@"dd\.hh\:mm\:fff");
        }
    }
}
