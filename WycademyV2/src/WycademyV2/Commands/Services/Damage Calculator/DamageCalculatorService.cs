using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WycademyV2.Commands.Services
{
    public class DamageCalculatorService
    {
        // Emojis to choose between 4U and Gen.
        private const string FOUR = "4⃣";
        private const string G = "🇬";
        // Weapon modifiers to find true raw.
        private readonly Dictionary<WeaponType, float> WEAPON_MODIFIERS;
        
        // Message IDs and the user to accept responses from.
        private Dictionary<ulong, DamageCalculatorMessage> _messages;
        private DiscordSocketClient _client;

        public DamageCalculatorService(DiscordSocketClient c)
        {
            _messages = new Dictionary<ulong, DamageCalculatorMessage>();
            WEAPON_MODIFIERS = new Dictionary<WeaponType, float>()
            {
                { WeaponType.Hammer, 5.2f },
                { WeaponType.HH,     5.2f },
                { WeaponType.SA,     5.4f },
                { WeaponType.GS,     4.8f },
                { WeaponType.CB,     3.6f },
                { WeaponType.LS,     3.3f },
                { WeaponType.IG,     3.1f },
                { WeaponType.Lance,  2.3f },
                { WeaponType.GL,     2.3f },
                { WeaponType.HBG,    1.5f },
                { WeaponType.SnS,    1.4f },
                { WeaponType.DB,     1.4f },
                { WeaponType.LBG,    1.3f },
                { WeaponType.Bow,    1.2f }
            };
            _client = c;
            _client.ReactionAdded += OnReactionAdded;
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
                        if (calcMessage.Weapon == WeaponType.Prowler)
                        {
                            await message.ModifyAsync(x => x.Content = "Nice try, but Prowlers aren't in 4U.");
                        }
                        else
                        {
                            await message.ModifyAsync(x => x.Content = GetResponseMessage4U(calcMessage));
                        }
                        break;
                    case G:
                        await message.ModifyAsync(x => x.Content = GetResponseMessageGen(calcMessage));
                        break;
                    default:
                        // If a different reaction was added, then just ignore it.
                        break;
                }
            }
        }

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
            public WeaponType Weapon { get; private set; }

            public DamageCalculatorMessage(IUser user, int dmg, int ele, int aff, SharpnessType sharpness, WeaponType weapon)
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
