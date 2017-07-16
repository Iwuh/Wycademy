using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Net;

namespace WycademyV2.Commands.Entities
{
    public class MonsterInfoMessage : ReactionMenuMessage
    {
        private const string FOUR = "4⃣";
        private const string GEN = "🇬";
        private const string CLOSE = "❌";

        private readonly Dictionary<string, string> GAME_EMOTES = new Dictionary<string, string>()
        {
            { "4U", FOUR },
            { "GEN", GEN }
        };

        private IDictionary<string, (string, int?)> _tables;
        private bool _choosing;
        private IUserMessage _message;
        private IUser _botUser;

        public MonsterInfoMessage(IUser user, IDictionary<string, (string, int?)> tables, IUser botUser) : base(user)
        {
            _tables = tables;
            _botUser = botUser;
        }

        public async override Task<IUserMessage> CreateMessageAsync(IMessageChannel channel)
        {
            _choosing = true;
            _message = await channel.SendMessageAsync("```This monster has data from more than one game. Please select a game using the reactions below.```");
            foreach (var game in _tables.Keys)
            {
                await _message.AddReactionAsync(new Emoji(GAME_EMOTES[game]));
            }

            return _message;
        }

        public async override Task HandleReaction(SocketReaction reaction)
        {
            string key;
            switch (reaction.Emote.Name)
            {
                case FOUR:
                    key = "4U";
                    break;
                case GEN:
                    key = "GEN";
                    break;
                default:
                    return;
            }

            var tuple = _tables[key];
            if (tuple.Item2 == null)
            {
                await _message.ModifyAsync(m => m.Content = tuple.Item1);
            }
            else
            {
                await _message.ModifyAsync(m => m.Content = tuple.Item1.Substring(0, tuple.Item2.Value));
                await _message.Channel.SendMessageAsync(tuple.Item1.Substring(tuple.Item2.Value));
            }

            _choosing = false;
            await CloseMenuAsync();
        }

        public async override Task CloseMenuAsync()
        {
            if (_choosing)
            {
                await _message.DeleteAsync();
            }
            else
            {   
                var guildAuthor = _botUser as SocketGuildUser;
                if (guildAuthor != null && guildAuthor.GuildPermissions.ManageMessages)
                {
                    await _message.RemoveAllReactionsAsync();
                }
                else
                {
                    foreach (var game in _tables.Keys)
                    {
                        await _message.RemoveReactionAsync(new Emoji(GAME_EMOTES[game]), _botUser);
                    }
                    await _message.RemoveReactionAsync(new Emoji(CLOSE), _botUser);
                }
            }
        }
    }
}
