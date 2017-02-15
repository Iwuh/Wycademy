﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WycademyV2.Commands.Enums;

namespace WycademyV2.Commands.Services
{
    public class DamageCalculatorService
    {
        // Emojis to choose between 4U and Gen.
        private const string FOUR = "4⃣";
        private const string G = "🇬";
        // Weapon modifiers to find true raw.
        private readonly Dictionary<CalculatorWeaponType, float> WEAPON_MODIFIERS;
        
        // Message IDs and the user to accept responses from.
        private Dictionary<ulong, DamageCalculatorMessage> _messages;
        private DiscordSocketClient _client;

        public DamageCalculatorService(DiscordSocketClient c)
        {
            _messages = new Dictionary<ulong, DamageCalculatorMessage>();
            WEAPON_MODIFIERS = new Dictionary<CalculatorWeaponType, float>()
            {
                { CalculatorWeaponType.Hammer, 5.2f },
                { CalculatorWeaponType.HH,     5.2f },
                { CalculatorWeaponType.SA,     5.4f },
                { CalculatorWeaponType.GS,     4.8f },
                { CalculatorWeaponType.CB,     3.6f },
                { CalculatorWeaponType.LS,     3.3f },
                { CalculatorWeaponType.IG,     3.1f },
                { CalculatorWeaponType.Lance,  2.3f },
                { CalculatorWeaponType.GL,     2.3f },
                { CalculatorWeaponType.HBG,    1.5f },
                { CalculatorWeaponType.SnS,    1.4f },
                { CalculatorWeaponType.DB,     1.4f },
                { CalculatorWeaponType.LBG,    1.3f },
                { CalculatorWeaponType.Bow,    1.2f }
            };
            _client = c;
            _client.ReactionAdded += OnReactionAdded;
        }

        /// <summary>
        /// Send a damage calculator message to the channel.
        /// </summary>
        /// <param name="context">The command context, used for constructing the message.</param>
        /// <param name="raw">The raw damage of the weapon.</param>
        /// <param name="element">The elemental or status damage of the weapon.</param>
        /// <param name="affinity">The affinity of the weapon.</param>
        /// <param name="sharpness">The sharpness of the weapon.</param>
        /// <param name="weapon">The type of the weapon.</param>
        /// <param name="cache">The optional CommandCacheService to add the message to.</param>
        /// <returns>The sent message.</returns>
        public async Task<IUserMessage> SendDamageCalculatorMessageAsync(CommandContext context, float raw, float element, float affinity, SharpnessType sharpness, CalculatorWeaponType weapon, CommandCacheService cache = null)
        {
            const string initialMessage = "Please confirm which game you'd like to calculate for by selecting a reaction below.";

            // Create the calculator message.
            var calculatorMessage = new DamageCalculatorMessage(context.User, raw, element, affinity, sharpness, weapon);

            // Send the message (optionally caching it)
            IUserMessage message;
            if (cache != null)
            {
                message = await context.Channel.SendCachedMessageAsync(context.Message.Id, cache, text: initialMessage, prependZWSP: true);
            }
            else
            {
                message = await context.Channel.SendMessageAsync($"\x200b{initialMessage}");
            }

            // Add reaction choices.
            await message.AddReactionAsync(FOUR);
            await message.AddReactionAsync(G);

            // Add the message ID (and associated data) to the dictionary.
            _messages.Add(message.Id, calculatorMessage);

            // Finally, return the message.
            return message;
        }

        /// <summary>
        /// Checks if the input is a valid sharpness type.
        /// </summary>
        /// <param name="input">The string to verify.</param>
        /// <returns>The SharpnessType version of the input if it's valid, otherwise null.</returns>
        public SharpnessType? ValidateSharpness(string input)
        {
            switch (input.ToLower())
            {
                case "red":
                case "r":
                    return SharpnessType.Red;
                case "orange":
                case "o":
                    return SharpnessType.Orange;
                case "yellow":
                case "y":
                    return SharpnessType.Yellow;
                case "green":
                case "g":
                    return SharpnessType.Green;
                case "blue":
                case "b":
                    return SharpnessType.Blue;
                case "white":
                case "w":
                    return SharpnessType.White;
                case "purple":
                case "p":
                    return SharpnessType.Purple;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Checks if the input is a valid weapon type.
        /// </summary>
        /// <param name="input">The string to verify.</param>
        /// <returns>The WeaponType version of the input if it's valid, otherwise null.</returns>
        public CalculatorWeaponType? ValidateWeapon(string input)
        {
            switch (input.ToLower())
            {
                case "greatsword":
                case "great_sword":
                case "gs":
                    return CalculatorWeaponType.GS;

                case "longsword":
                case "long_sword":
                case "ls":
                    return CalculatorWeaponType.LS;

                case "swordandshield":
                case "sword_and_shield":
                case "sword&shield":
                case "s&s":
                case "sns":
                    return CalculatorWeaponType.SnS;

                case "dualblades":
                case "dual_blades":
                case "db":
                    return CalculatorWeaponType.DB;

                case "lance":
                    return CalculatorWeaponType.Lance;

                case "gunlance":
                case "gun_lance":
                case "gl":
                    return CalculatorWeaponType.GL;

                case "hemmer":
                case "hemmr":
                case "hammer":
                    return CalculatorWeaponType.Hammer;

                case "huntinghorn":
                case "hunting_horn":
                case "doot":
                case "hh":
                    return CalculatorWeaponType.HH;

                case "switch_axe":
                case "switchaxe":
                case "swaxe":
                case "sa":
                    return CalculatorWeaponType.SA;

                case "charge_blade":
                case "chargeblade":
                case "cb":
                    return CalculatorWeaponType.CB;

                case "lightbowgun":
                case "light_bowgun":
                case "lbg":
                    return CalculatorWeaponType.LBG;

                case "heavybowgun":
                case "heavy_bowgun":
                case "hbg":
                    return CalculatorWeaponType.HBG;

                case "bow":
                    return CalculatorWeaponType.Bow;

                case "prowler":
                    return CalculatorWeaponType.Prowler;

                case "insectglaive":
                case "insect_glaive":
                case "ig":
                    return CalculatorWeaponType.IG;

                default:
                    return null;
            }
        }

        /// <summary>
        /// Process reactions added to a message.
        /// </summary>
        /// <param name="id">The id of the message that the reaction was added to.</param>
        /// <param name="userMessage">The message object itself.</param>
        /// <param name="reaction">The reaction added.</param>
        private async Task OnReactionAdded(ulong id, Optional<SocketUserMessage> userMessage, SocketReaction reaction)
        {
            var message = userMessage.GetValueOrDefault();
            // If message is null then we can't check anything so just return.
            if (message == null) return;
            // If the reaction doesn't have a user associated with it assume it's the incorrect user.
            if (!reaction.User.IsSpecified) return;

            // Make sure the reaction is added to a DamageCalculatorMessage and not something else.
            DamageCalculatorMessage calcMessage;
            if (_messages.TryGetValue(id, out calcMessage))
            {
                // Don't trigger anything when the bot is originally adding the reactions.
                if (_client.CurrentUser.Id == reaction.UserId) return;
                // Ignore the reaction if it isn't by the user who added the command. 
                if (calcMessage.User.Id != reaction.UserId) return;

                // Check whether the user chose 4U or Gen and edit the message accordingly.
                switch (reaction.Emoji.Name)
                {
                    case FOUR:
                        if (calcMessage.Weapon == CalculatorWeaponType.Prowler)
                        {
                            await message.ModifyAsync(x => x.Content = "Nice try, but Prowlers aren't in 4U.");
                            _messages.Remove(id);
                        }
                        else
                        {
                            await message.ModifyAsync(x => x.Content = GetResponseMessage4U(calcMessage));
                            _messages.Remove(id);
                        }
                        break;
                    case G:
                        await message.ModifyAsync(x => x.Content = GetResponseMessageGen(calcMessage));
                        _messages.Remove(id);
                        break;
                    default:
                        // If a different reaction was added, then just ignore it.
                        break;
                }
            }
        }

        /// <summary>
        /// Calculates and makes a string out of the inputted message, using Monster Hunter Generations numbers.
        /// </summary>
        /// <param name="message">The data message to calculate from.</param>
        /// <returns>The message to show the user.</returns>
        private string GetResponseMessageGen(DamageCalculatorMessage message)
        {
            var modifiers = message.GetSharpnessModifiers();
            // Expected raw follows the following formula: (Attack * (1 + Crit Modifier * (Affinity / 100))) * Raw Sharpness Modifier.
            // The crit modifier is 0.25, or 0.4 with crit boost.
            float expectedRaw = (message.RawDamage * (1 + 0.25f * (message.Affinity / 100.0f))) * modifiers.RawModifier;
            float expectedRawCritBoost = (message.RawDamage * (1 + 0.4f * (message.Affinity / 100.0f))) * modifiers.RawModifier;
            // Expected element/status is simply: Attack * Elemental/Status Sharpness Modifier.
            float expectedElement = message.ElementDamage * modifiers.ElementModifier;

            var sb = new StringBuilder();
            sb.AppendLine("```");
            sb.AppendLine($"Expected raw: {expectedRaw}");
            sb.AppendLine($"Expected raw with Crit Boost: {expectedRawCritBoost}");
            sb.AppendLine($"Expected element: {expectedElement}");
            sb.AppendLine("Note: these numbers do not take into account motion values, hitzones, rank modifiers, and individual quest modifiers.");
            sb.AppendLine("```");

            return sb.ToString();
        }

        /// <summary>
        /// Calculates and makes a string out of the inputted message, using Monster Hunter 4 Ultimate numbers (i.e. converts display raw to true).
        /// </summary>
        /// <param name="message">The data message to calculate from.</param>
        /// <returns>The messag to show the user.</returns>
        private string GetResponseMessage4U(DamageCalculatorMessage message)
        {
            var modifiers = message.GetSharpnessModifiers();
            // Expected raw in 4u is: (Attack / Weapon Modifier * (1 + 0.25 * (Affinity / 100))) * Raw Sharpness Modifier.
            float expectedRaw = (message.RawDamage / WEAPON_MODIFIERS[message.Weapon] * (1 + 0.25f * (message.Affinity / 100))) * modifiers.RawModifier;
            // Expected element/status in 4u is Attack / 10 * Elemental/Status Sharpness Modifier.
            float expectedElement = message.ElementDamage / 10.0f * modifiers.ElementModifier;

            var sb = new StringBuilder();
            sb.AppendLine("```");
            sb.AppendLine($"Expected raw: {expectedRaw}");
            sb.AppendLine($"Expected element: {expectedElement}");
            sb.AppendLine("Note: these numbers do not take into account motion values, hitzones, rank modifiers, and individual quest modifiers.");
            sb.AppendLine("```");

            return sb.ToString();
        }

        private class DamageCalculatorMessage
        {
            public IUser User { get; private set; }
            public float RawDamage { get; private set; }
            public float ElementDamage { get; private set; }
            public float Affinity { get; private set; }
            public SharpnessType Sharpness { get; private set; }
            public CalculatorWeaponType Weapon { get; private set; }

            public DamageCalculatorMessage(IUser user, float dmg, float ele, float aff, SharpnessType sharpness, CalculatorWeaponType weapon)
            {
                User = user;
                RawDamage = dmg;
                ElementDamage = ele;
                Affinity = aff;
                Sharpness = sharpness;
                Weapon = weapon;
            }

            /// <summary>
            /// Gets the raw & elemental sharpness modifiers for a given sharpness value.
            /// </summary>
            /// <returns>A SharpnessModifierPair representing the modifiers.</returns>
            /// <exception cref="ArgumentException">Thrown if the supplied sharpness name is invalid.</exception>
            public SharpnessModifierPair GetSharpnessModifiers()
            {
                switch (Sharpness)
                {
                    case SharpnessType.Red:
                        return new SharpnessModifierPair(0.5f, 0.25f);
                    case SharpnessType.Orange:
                        return new SharpnessModifierPair(0.75f, 0.5f);
                    case SharpnessType.Yellow:
                        return new SharpnessModifierPair(1.0f, 0.75f);
                    case SharpnessType.Green:
                        return new SharpnessModifierPair(1.125f, 1.0f);
                    case SharpnessType.Blue:
                        return new SharpnessModifierPair(1.25f, 1.0625f);
                    case SharpnessType.White:
                        return new SharpnessModifierPair(1.32f, 1.125f);
                    case SharpnessType.Purple:
                        return new SharpnessModifierPair(1.44f, 1.2f);
                    default:
                        throw new ArgumentException($"{Sharpness} is not a valid sharpness.");
                }
            }
        }

        private struct SharpnessModifierPair
        {
            public float RawModifier { get; private set; }
            public float ElementModifier { get; private set; }

            public SharpnessModifierPair(float raw, float ele)
            {
                RawModifier = raw;
                ElementModifier = ele;
            }
        }
    }
}
