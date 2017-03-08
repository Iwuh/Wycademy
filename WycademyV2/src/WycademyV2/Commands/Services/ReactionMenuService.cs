using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using WycademyV2.Commands.Entities;
using Discord;
using System.Threading.Tasks;

namespace WycademyV2.Commands.Services
{
    public class ReactionMenuService
    {
        private Dictionary<ulong, ReactionMenuMessage> _messages;
        private DiscordSocketClient _client;

        public ReactionMenuService(DiscordSocketClient client)
        {
            _messages = new Dictionary<ulong, ReactionMenuMessage>();
            _client = client;
            client.ReactionAdded += OnReactionAdded;
        }

        public async Task<IUserMessage> SendReactionMenuMessageAsync(IMessageChannel channel, ReactionMenuMessage menu)
        {
            // Send the message, using the implementation defined by the reaction menu message.
            var message = await menu.CreateMessageAsync(channel);

            // Add the message to the cache.
            _messages.Add(message.Id, menu);

            return message;
        }

        private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> cachedMessage, ISocketMessageChannel channel, SocketReaction reaction)
        {
            var message = await cachedMessage.GetOrDownloadAsync();
            // If the reaction doesn't have a valid user, ignore it.
            if (!reaction.User.IsSpecified) return;

            // Make sure the message is actually in the list.
            if (_messages.TryGetValue(message.Id, out ReactionMenuMessage menu))
            {
                // Don't trigger anything when originally adding the reactions.
                if (reaction.UserId == _client.CurrentUser.Id) return;
                // Ignore any reactions that aren't by the user that used the command.
                if (reaction.UserId != menu.User.Id) return;

                await menu.HandleReaction(reaction);

                // Remove the user's reaction if the message is in a guild and the bot has the Manage Messages permission.
                var guildAuthor = message.Author as SocketGuildUser;
                if (guildAuthor.GuildPermissions.ManageMessages)
                {
                    if (reaction.Emoji.Id == null) // Unicode emoji
                    {
                        await message.RemoveReactionAsync(reaction.Emoji.Name, menu.User);
                    }
                    else // Discord emoji
                    {
                        await message.RemoveReactionAsync(reaction.Emoji, menu.User);
                    }
                }
            }
        }
    }
}
