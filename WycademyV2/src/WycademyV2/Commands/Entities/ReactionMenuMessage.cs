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
        public string[] Buttons { get; protected set; }

        /// <summary>
        /// Initializes the User to accept reactions from. (having this constructor forces any derived class to call it with base).
        /// </summary>
        /// <param name="user">The user that sent the command.</param>
        /// <param name="emojis">All the emojis that should be added to the message as menu buttons.</param>
        protected ReactionMenuMessage(IUser user, params string[] emojis)
        {
            User = user;
            Buttons = emojis;
        }

        public abstract Task HandleReaction(SocketReaction reaction);

        public abstract Task<IUserMessage> SendMessage(IMessageChannel channel);
    }
}
