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
            return !enumerable.Any(predicate);
        }

        /// <summary>
        /// Converts a collection of strings to Discord IDs, ignoring any that are invalid.
        /// </summary>
        /// <param name="text">The collection to convert.</param>
        /// <returns>A collection of IDs.</returns>
        public static IEnumerable<ulong> ParseIDs(this IEnumerable<string> text)
        {
            List<ulong> ids = new List<ulong>();

            foreach (string item in text)
            {
                ulong parsed;
                if (ulong.TryParse(item, out parsed))
                {
                    ids.Add(parsed);
                }
            }

            return ids;
        }

        /// <summary>
        /// Uses the user's highest role colour if the user is a guild user, otherwise uses the default colour.
        /// </summary>
        /// <param name="user">The user to check. <see cref="Color.Default"/> will be used if the user is not an IGuildUser, or is null.</param>
        /// <returns>The EmbedBuilder instance.</returns>
        public static EmbedBuilder WithHighestRoleColour(this EmbedBuilder builder, IUser user)
        {
            var colour = Color.Default; // 'Colour' should have a 'u', fight me irl
            if (user != null && user is IGuildUser guildUser)
            {
                var roles = guildUser.RoleIds.Select(i => guildUser.Guild.GetRole(i)).OrderByDescending(r => r.Position);
                colour = roles.First().Color;
            }

            return builder.WithColor(colour); // Personal preference > consistency
        }

        /// <summary>
        /// Gets the colour of a user's highest role.
        /// </summary>
        /// <returns>The user's highest role colour, or <see cref="Color.Default"/> if the user does not have any coloured roles.</returns>
        public static Color GetHighestRoleColour(this IUser user)
        {
            if (user is IGuildUser guildUser)
            {
                // Get all the user's roles, ordered by position in the role hierarchy.
                var roles = guildUser.RoleIds.Select(i => guildUser.Guild.GetRole(i)).OrderByDescending(r => r.Position);
                // Get the first role that has a colour, or null if none have a colour.
                var firstColouredRole = roles.FirstOrDefault(r => !r.Color.Equals(Color.Default));
                // Return the colour if applicable, otherwise return the default colour.
                return firstColouredRole?.Color ?? Color.Default;
            }
            return Color.Default;
        }
    }
}
