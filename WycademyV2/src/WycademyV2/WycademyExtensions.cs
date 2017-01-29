using Discord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WycademyV2.Commands.Services;

namespace WycademyV2
{
    public static class WycademyExtensions
    {
        /// <summary>
        /// Sends a message to a channel and caches the IDs of the command and response messages.
        /// </summary>
        /// <param name="channel">The channel to send the message in.</param>
        /// <param name="text">The message to send.</param>
        /// <param name="commandID">The ID of the command message.</param>
        /// <param name="cache">The CommandCacheService to add the IDs to.</param>
        /// <param name="prependZWSP">Whether or not to add a zero-width space to the beginning of the message.</param>
        /// <param name="file">A Stream to be uploaded with the message.</param>
        /// <param name="fileName">The name to use for the uploaded stream.</param>
        /// <returns>A Task containing an IUserMessage.</returns>
        public static async Task<IUserMessage> SendCachedMessageAsync(this IMessageChannel channel, ulong commandID, CommandCacheService cache, string text = "", bool prependZWSP = false, Stream file = null, string fileName = "No Name", EmbedBuilder embed = null)
        {
            IUserMessage m;
            if (file == null)
            {
                m = await channel.SendMessageAsync(prependZWSP ? "\x200b" + text : text, embed: embed);
            }
            else
            {
                m = await channel.SendFileAsync(file, fileName, prependZWSP ? "\x200b" + text : text);
            }
            await Task.Delay(1000);
            cache.Add(commandID, m.Id);
            return m;
        }

        /// <summary>
        /// Checks that none of the elements of an IEnumerable match a predicate.
        /// </summary>
        /// <typeparam name="TSource">The element type of the collection</typeparam>
        /// <param name="enumerable">The enumerable to check.</param>
        /// <param name="predicate">The condition to check for.</param>
        /// <returns>True if the predicate matched nothing.</returns>
        public static bool None<TSource>(this IEnumerable<TSource> enumerable, Func<TSource, bool> predicate)
        {
            return !enumerable.All(predicate);
        }
    }
}
