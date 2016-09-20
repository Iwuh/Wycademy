using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wycademy
{
    public static class DiscordExtensions
    {
        public static async Task<Message> SendMessageZWSP(this Channel c, string message)
        {
            return await c.SendMessage("\x200b" + message);
        }
        public static async Task<Message> SendMessageZWSP(this User u, string message)
        {
            return await u.SendMessage("\x200b" + message);
        }
    }
}
