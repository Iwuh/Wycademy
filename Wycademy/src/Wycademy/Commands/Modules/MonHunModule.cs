﻿using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Wycademy.Commands.Entities;
using Wycademy.Commands.Enums;
using Wycademy.Commands.Preconditions;
using Wycademy.Commands.Services;

namespace Wycademy.Commands.Modules
{
    [Name("monhun")]
    [Summary("General MH Commands")]
    public class MonHunModule : ModuleBase<SocketCommandContext>
    {
        private MonsterInfoService _minfo;
        private LockerService _locker;
        private MotionValueService _mv;
        private CommandCacheService _cache;
        private DamageCalculatorService _damagecalc;
        private WeaponInfoService _weapon;
        private ReactionMenuService _reactions;

        public MonHunModule(MonsterInfoService mis, LockerService ls, MotionValueService mv, CommandCacheService ccs, DamageCalculatorService dcs, WeaponInfoService wis, ReactionMenuService rms)
        {
            _minfo = mis;
            _locker = ls;
            _mv = mv;
            _cache = ccs;
            _damagecalc = dcs;
            _weapon = wis;
            _reactions = rms;
        } 

        [Command("motionvalue")]
        [Alias("motionvalues", "movementvalue", "movementvalues", "mv")]
        [Summary("Gets the motion values for a specific weapon.")]
        [RequireUnlocked]
        public async Task GetMV([Remainder, Summary("The weapon to find motion values for. Can be the shortened form of the weapon (ex. gs, hh) or the full name (ex. hammer, dual blades).")] string weapon)
        {
            try
            {
                var tuple = _mv.GetMotionValues(string.Join("-", weapon.ToLower().Split(' ', '_')));
                if (tuple.splitPoint != null)
                {
                    await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, tuple.text.Substring(0, tuple.splitPoint.Value));
                    await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, tuple.text.Substring(tuple.splitPoint.Value));
                }
                else
                {
                    await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, tuple.text);
                }
            }
            catch (ArgumentException)
            {
                await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, weapon + WycademyConst.INVALID_MV_WEAPON_NAME);
            }
        }

        [Command("damagecalculator")]
        [Alias("dc")]
        [Summary("Finds the expected raw and element for a weapon based on the inputted data.")]
        [RequireUnlocked]
        [RequireBotPermission(ChannelPermission.AddReactions)]
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

        [Command("teostratimer")]
        [Alias("toast", "tost", "toasttimer", "tosttimer")]
        [Summary("Sends a timer menu to help you keep track of Teostra novas.")]
        [RequireUnlocked]
        [RequireBotPermission(ChannelPermission.AddReactions)]
        public async Task ToastTimer()
        {
            var message = await _reactions.SendReactionMenuMessageAsync(Context.Channel, new ToastTimerMessage(Context.User));
            _cache.Add(Context.Message.Id, message.Id);
        }
    }
}
