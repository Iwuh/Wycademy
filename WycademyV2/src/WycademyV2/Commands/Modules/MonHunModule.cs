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
        private MonsterInfoService _minfo;
        private LockerService _locker;
        private MotionValueService _mv;

        public MonHunModule(MonsterInfoService mis, LockerService ls, MotionValueService mv)
        {
            _minfo = mis;
            _locker = ls;
            _mv = mv;
        }

        [Command("hitzone")]
        [Alias("hitzones")]
        [Summary("Gets the hitzone data for the specified monster.")]
        [RequireUnlocked]
        public async Task GetHitzoneData([Remainder, Summary("The monster to search for.")] string monster)
        {
            try
            {
                string table = await _minfo.GetMonsterInfo("Hitzone", monster);
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
                string table = await _minfo.GetMonsterInfo("Status", monster);
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
                string table = await _minfo.GetMonsterInfo("Stagger/Sever", monster);
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
                string table = await _minfo.GetMonsterInfo("Item Effects", monster);
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
            var dm = await Context.User.GetDMChannelAsync() ?? await Context.User.CreateDMChannelAsync();
            await dm.SendMessageAsync(_minfo.GetMonsterNames());
        }

        [Command("motionvalue")]
        [Alias("motionvalues", "movementvalue", "movementvalues", "mv")]
        [Summary("Gets the motion values for a specific weapon.")]
        [RequireUnlocked]
        public async Task GetMV([Remainder, Summary("The weapon to find motion values for.")] string weapon)
        {
            var mvStream = _mv.GetMotionValueStream(weapon);
            try
            {
                await Context.Channel.SendFileAsync(mvStream, mvStream.Name);
            }
            catch (ArgumentException)
            {
                await ReplyAsync("Weapon name not recognised. Try <weaponlist for a list of options.");
            }
        }

        [Command("weaponlist")]
        [Summary("Gets a list of weapon names recognised by `<motionvalue`.")]
        [RequireUnlocked]
        public async Task GetWeaponList()
        {
            var dm = await Context.User.GetDMChannelAsync() ?? await Context.User.CreateDMChannelAsync();
            await dm.SendMessageAsync(_mv.GetWeaponNames());
        }
    }
}
