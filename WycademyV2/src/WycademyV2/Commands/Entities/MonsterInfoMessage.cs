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

        private readonly Dictionary<string, string> GAME_EMOTES = new Dictionary<string, string>()
        {
            { "4U", FOUR },
            { "GEN", GEN }
        };

        private IDictionary<string, string> _tables;
        private bool _choosing;
        private IUserMessage _message;
        private DiscordSocketClient _client;

        public MonsterInfoMessage(IUser user, IDictionary<string, string> tables, DiscordSocketClient client) : base(user)
        {
            _tables = tables;
            _client = client;
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
            switch (reaction.Emote.Name)
            {
                case FOUR:
                    await _message.ModifyAsync(m => m.Content = _tables["4U"]);
                    break;
                case GEN:
                    await _message.ModifyAsync(m => m.Content = _tables["GEN"]);
                    break;
            }
            _choosing = false;
        }

        public async override Task CloseMenuAsync()
        {
            if (_choosing)
            {
                await _message.DeleteAsync();
            }
            else
            {
                try
                {
                    // Attempt to bulk remove reactions (will throw HttpException if the bot does not have Manage Messages).
                    await _message.RemoveAllReactionsAsync();
                }
                catch (HttpException)
                {
                    foreach (var game in _tables.Keys)
                    {
                        await _message.RemoveReactionAsync(new Emoji(GAME_EMOTES[game]), _client.CurrentUser);
                    }
                }   
            }
        }
    }
}
