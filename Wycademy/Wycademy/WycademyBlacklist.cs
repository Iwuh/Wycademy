using Discord;
using Discord.Commands.Permissions.Userlist;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wycademy
{
    static class WycademyBlacklist
    {
        public static List<ulong> ServerBlacklist = InitializeServerBlacklist();
        public static List<ulong> ServerOwnerBlacklist = InitializeServerOwnerBlacklist();

        public static ulong[] InitializeUserBlacklist()
        {
            // Populate the user blacklist
            using (StreamReader sr = new StreamReader("userblacklist.txt"))
            {
                string text = sr.ReadToEnd();
                if (text != string.Empty)
                {
                    return text.Split(',').Select(x => ulong.Parse(x)).ToArray();
                }
                return new ulong[0];
            }
        }

        public static async Task UpdateUserBlacklist(DiscordClient _client)
        {
            // Converts Blacklist to a string and then writes it to a file.
            string blacklistString = string.Join(",", _client.GetBlacklistedUserIds());

            using (StreamWriter sw = new StreamWriter("userblacklist.txt", false))
            {
                await sw.WriteAsync(blacklistString);
            }
        }

        private static List<ulong> InitializeServerBlacklist()
        {
            // Populate the server blacklist
            using (StreamReader sr = new StreamReader("serverblacklist.txt"))
            {
                string text = sr.ReadToEnd();
                if (text != string.Empty)
                {
                    return text.Split(',').Select(x => ulong.Parse(x)).ToList();
                }
                return new List<ulong>();
            }
        }

        public static async Task UpdateServerBlacklist()
        {
            // Converts Blacklist to a string and then writes it to a file.
            string blacklistString = string.Join(",", ServerBlacklist);

            using (StreamWriter sw = new StreamWriter("serverblacklist.txt", false))
            {
                await sw.WriteAsync(blacklistString);
            }
        }

        private static List<ulong> InitializeServerOwnerBlacklist()
        {
            // Populate the server blacklist
            using (StreamReader sr = new StreamReader("serverownerblacklist.txt"))
            {
                string text = sr.ReadToEnd();
                if (text != string.Empty)
                {
                    return text.Split(',').Select(x => ulong.Parse(x)).ToList();
                }
                return new List<ulong>();
            }
        }

        public static async Task UpdateServerOwnerBlacklist()
        {
            // Converts Blacklist to a string and then writes it to a file.
            string blacklistString = string.Join(",", ServerBlacklist);

            using (StreamWriter sw = new StreamWriter("serverownerblacklist.txt", false))
            {
                await sw.WriteAsync(blacklistString);
            }
        }
    }
}
