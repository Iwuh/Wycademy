using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WycademyV2.Commands.Entities
{
    public abstract class ReactionMenuMessage
    {
        public IUser User { get; protected set; }

        /// <summary>
        /// Initializes the User to accept reactions from. (having this constructor forces any derived class to call it with base).
        /// </summary>
        /// <param name="user">The user that sent the command.</param>
        /// <param name="emojis">All the emojis that should be added to the message as menu buttons.</param>
        protected ReactionMenuMessage(IUser user)
        {
            User = user;
        }

        /// <summary>
        /// Handle the reaction, and change the message appropriately.
        /// </summary>
        /// <param name="reaction">The reaction that was added.</param>
        /// <returns>An awaitable Task.</returns>
        public abstract Task HandleReaction(SocketReaction reaction);

        /// <summary>
        /// Send the message, and add any reactions.
        /// </summary>
        /// <param name="channel">The channel to send the message to.</param>
        /// <returns>An awaitable Task containing an IUserMessage.</returns>
        public abstract Task<IUserMessage> CreateMessageAsync(IMessageChannel channel);

        /// <summary>
        /// Stop the reaction menu, using a user defined implementation.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        public abstract Task CloseMenuAsync();
    }
}
