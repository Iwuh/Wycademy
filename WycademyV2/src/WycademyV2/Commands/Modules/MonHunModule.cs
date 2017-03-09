using Discord.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WycademyV2.Commands.Entities;
using WycademyV2.Commands.Enums;
using WycademyV2.Commands.Preconditions;
using WycademyV2.Commands.Services;

namespace WycademyV2.Commands.Modules
{
    [Summary("Monster Hunter Commands")]
    public class MonHunModule : ModuleBase
    {
        private MonsterInfoService _minfo;
        private LockerService _locker;
        private MotionValueService _mv;
        private CommandCacheService _cache;
        private DamageCalculatorService _damagecalc;
        private ToastTimerService _toast;
        private WeaponInfoService _weapon;
        private ReactionMenuService _reactions;

        public MonHunModule(MonsterInfoService mis, LockerService ls, MotionValueService mv, CommandCacheService ccs, DamageCalculatorService dcs, ToastTimerService tts, WeaponInfoService wis, ReactionMenuService rms)
        {
            _minfo = mis;
            _locker = ls;
            _mv = mv;
            _cache = ccs;
            _damagecalc = dcs;
            _toast = tts;
            _weapon = wis;
            _reactions = rms;
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
                await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, text: table, prependZWSP: true);
            }
            catch (FileNotFoundException)
            {
                await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, text: monster + WycademyConst.INVALID_MONSTER_NAME, prependZWSP: true);
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
                await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, text: table, prependZWSP: true);
            }
            catch (FileNotFoundException)
            {
                await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, text: monster + WycademyConst.INVALID_MONSTER_NAME, prependZWSP: true);
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
                await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, text: table, prependZWSP: true);
            }
            catch (FileNotFoundException)
            {
                await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, text: monster + WycademyConst.INVALID_MONSTER_NAME, prependZWSP: true);
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
                await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, text: table, prependZWSP: true);
            }
            catch (FileNotFoundException)
            {
                await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, text: monster + WycademyConst.INVALID_MONSTER_NAME, prependZWSP: true);
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
            try
            {
                var mvStream = _mv.GetMotionValueStream(weapon);
                await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, file: mvStream, fileName: mvStream.Name);
            }
            catch (ArgumentException)
            {
                await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, text: weapon + WycademyConst.INVALID_MV_WEAPON_NAME, prependZWSP: true);
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

        [Command("damagecalculator")]
        [Alias("dc")]
        [Summary("Finds the expected raw and element for a weapon based on the inputted data.")]
        [RequireUnlocked]
        public async Task DamageCalculator([Summary("The weapon to calculate for. Can be either the full name or an abbreviation, but must be one word. ex. greatsword/great_sword/gs.")] string weapon,
            [Summary("The raw damage of the weapon. Can be a whole number or a decimal.")] float raw,
            [Summary("The elemental (or status) damage of the weapon. Can be a whole number or a decimal.")] float element,
            [Summary("The affinity of the weapon. Can be a whole number or a decimal (don't include the % sign).")] float affinity,
            [Summary("The sharpness colour of the weapon. Can be the full name or just the first letter. ex. r/red.")] string sharpness)
        {
            // Validate the weapon type.
            WeaponType? wType = _damagecalc.ValidateWeapon(weapon);
            if (wType == null)
            {
                await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, text: weapon + WycademyConst.INVALID_DC_WEAPON_NAME, prependZWSP: true);
                return;
            }
            // Show a notice if the weapon type is currently not supported.
            else if (wType == WeaponType.LBG || wType == WeaponType.HBG || wType == WeaponType.Bow || wType == WeaponType.Prowler)
            {
                await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, text: WycademyConst.DC_WEAPON_TYPE_NOT_SUPPORTED, prependZWSP: true);
                return;
            }

            // Validate the sharpness colour.
            SharpnessType? sType = _damagecalc.ValidateSharpness(sharpness);
            if (sType == null)
            {
                await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, text: sharpness + WycademyConst.INVALID_DC_SHARPNESS_NAME, prependZWSP: true);
                return;
            }

            // If everything is good, then send the message.
            await _damagecalc.SendDamageCalculatorMessageAsync(Context, raw, element, affinity, sType.Value, wType.Value, _cache);
        }

        [Command("toasttimer")]
        [Alias("toast", "tost")]
        [Summary("Sends a timer menu to help you keep track of Teostra novas.")]
        [RequireUnlocked]
        public async Task ToastTimer()
        {
            await _toast.SendToastTimerMessageAsync(Context, _cache);
        }

        [Command("weaponinfo")]
        [Summary("Gets the info for a specific weapon.")]
        [RequireUnlocked]
        public async Task GetWeaponInfo([Remainder, Summary("All or part of the weapons name. |<number> after the name lets you optionally specify a starting level.")] string query)
        {
            var split = query.Split('|');

            var results = _weapon.SearchWeaponInfo(split[0]);
            if (results.Count == 0)
            {
                await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, text: $"No weapon was found containing the string \"{split[0]}\"", prependZWSP: true);
            }
            else if (results.Count > 1)
            {
                await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, prependZWSP: true,
                    text: $"Multiple matches were found:\n{string.Join("\n", results.Select(r => r.Name))}");
            }
            else
            {
                var pages = _weapon.BuildWeaponInfoPages(results[0]);
                var message = await _reactions.SendReactionMenuMessageAsync(Context.Channel, 
                    new WeaponInfoMessage(Context.User, pages, ValidatePageNumber()));
                await Task.Delay(1000);
                _cache.Add(Context.Message.Id, message.Id);
            }

            int ValidatePageNumber()
            {
                // Return 0 (default page) if no page is specified.
                if (split.Length == 1) return 0;

                // Return the page index if it can be parsed, otherwise 0.
                if (int.TryParse(split[1], out int result))
                {
                    return result;
                }
                return 0;
            }
        }
    }
}
