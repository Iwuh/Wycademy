using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Wycademy
{
    /// <summary>
    /// Used to automatically add a reaction any time the bot is mentioned.
    /// </summary>
    static class WycademyReactions
    {
        public static string GetReactionURL(ulong channel, ulong message, string emoji)
        {
            string formattedEmoji = HttpUtility.UrlEncode(emoji);
            return $"https://canary.discordapp.com/api/v6/channels/{channel}/messages/{message}/reactions/{formattedEmoji}/@me";
        }
        public static string GetReactionURL(ulong channel, ulong message, Server.Emoji emoji)
        {
            string formattedEmoji = $"<:{emoji.Name}:{emoji.Id}>";
            return $"https://canary.discordapp.com/api/v6/channels/{channel}/messages/{message}/reactions/{formattedEmoji}/@me";
        }

        public static async Task SendReaction(string URI)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage reaction = new HttpRequestMessage(HttpMethod.Put, URI);
                reaction.Headers.Authorization = new AuthenticationHeaderValue(Environment.GetEnvironmentVariable("WYCADEMY_TOKEN", EnvironmentVariableTarget.User));

                await client.SendAsync(reaction);
            }
        }
    }
}
