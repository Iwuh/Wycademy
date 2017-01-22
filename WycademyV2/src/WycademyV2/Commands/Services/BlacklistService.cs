using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WycademyV2.Commands.Enums;

namespace WycademyV2.Commands.Services
{
    public class BlacklistService
    {
        private List<ulong> _userBlacklist;
        private List<ulong> _guildBlacklist;
        private List<ulong> _guildOwnerBlacklist;

        public BlacklistService()
        {
            _userBlacklist = new List<ulong>();
            _guildBlacklist = new List<ulong>();
            _guildOwnerBlacklist = new List<ulong>();
        }

        /// <summary>
        /// Adds an ID to a blacklist, if it's not already there.
        /// </summary>
        /// <param name="id">The ID to add.</param>
        /// <param name="category">The category to add the ID to.</param>
        public async Task AddToBlacklist(ulong id, BlacklistType category)
        {
            switch (category)
            {
                case BlacklistType.User:
                    if (!CheckBlacklist(id, BlacklistType.User)) _userBlacklist.Add(id);
                    break;
                case BlacklistType.Guild:
                    if (!CheckBlacklist(id, BlacklistType.Guild)) _guildBlacklist.Add(id);
                    break;
                case BlacklistType.GuildOwner:
                    if (!CheckBlacklist(id, BlacklistType.GuildOwner)) _guildOwnerBlacklist.Add(id);
                    break;
            }

            await SaveAsync();
        }

        /// <summary>
        /// Removes an item from the specified blacklist.
        /// </summary>
        /// <param name="id">The ID to remove.</param>
        /// <param name="category">The blacklist to remove it from.</param>
        /// <returns>Whether or not the operation was successful.</returns>
        public bool RemoveFromBlacklist(ulong id, BlacklistType category)
        {
            List<ulong> blacklistToCheck = null;

            switch (category)
            {
                case BlacklistType.User:
                    blacklistToCheck = _userBlacklist;
                    break;
                case BlacklistType.Guild:
                    blacklistToCheck = _guildBlacklist;
                    break;
                case BlacklistType.GuildOwner:
                    blacklistToCheck = _guildOwnerBlacklist;
                    break;
            }

            // Category always has to be one of the 3 blacklist types, so we don't need to worry about blacklistToCheck being null.
            return blacklistToCheck.Remove(id);
        }

        /// <summary>
        /// Checks if a certain blacklist contains an ID.
        /// </summary>
        /// <param name="id">The ID to check for.</param>
        /// <param name="category">The category to check in.</param>
        /// <returns>Whether or not the ID is on the specified blacklist.</returns>
        public bool CheckBlacklist(ulong id, BlacklistType category)
        {
            List<ulong> blacklistToCheck = null;

            switch (category)
            {
                case BlacklistType.User:
                    blacklistToCheck = _userBlacklist;
                    break;
                case BlacklistType.Guild:
                    blacklistToCheck = _guildBlacklist;
                    break;
                case BlacklistType.GuildOwner:
                    blacklistToCheck = _guildOwnerBlacklist;
                    break;
            }

            // This will never be null, because category will never not be one of the 3 blacklist types.
            return blacklistToCheck.Contains(id);
        }

        /// <summary>
        /// Gets all the IDs on the blacklist.
        /// </summary>
        /// <returns>The IDs on the blacklist, formatted as a string.</returns>
        public string GetBlacklist()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Users:");
            sb.AppendLine(string.Join(" ", _userBlacklist));

            sb.AppendLine("Servers:");
            sb.AppendLine(string.Join(" ", _guildBlacklist));

            sb.AppendLine("Server Owners:");
            sb.AppendLine(string.Join(" ", _guildOwnerBlacklist));

            return sb.ToString();
        }

        /// <summary>
        /// Populate the blacklists with the contents of the files.
        /// </summary>
        public async Task LoadAsync()
        {
            using (FileStream userBlacklist = File.Open(@".\userblacklist.txt", FileMode.OpenOrCreate))
            using (FileStream guildBlacklist = File.Open(@".\guildblacklist.txt", FileMode.OpenOrCreate))
            using (FileStream guildOwnerBlacklist = File.Open(@".\guildownerblacklist.txt", FileMode.OpenOrCreate))
            {
                // Create a byte array for each file.
                byte[] userBytes = new byte[userBlacklist.Length];
                byte[] guildBytes = new byte[guildBlacklist.Length];
                byte[] guildOwnerBytes = new byte[guildOwnerBlacklist.Length];

                // Read into each byte array.
                await userBlacklist.ReadAsync(userBytes, 0, (int)userBlacklist.Length);
                await guildBlacklist.ReadAsync(guildBytes, 0, (int)guildBlacklist.Length);
                await guildOwnerBlacklist.ReadAsync(guildOwnerBytes, 0, (int)guildOwnerBlacklist.Length);

                // Convert the byte arrays into List<ulong>s and assign them to the blacklists.
                UnicodeEncoding unicode = new UnicodeEncoding();
                _userBlacklist = unicode.GetString(userBytes, 0, userBytes.Length).Split(' ').Select(n => ulong.Parse(n)).ToList();
                _guildBlacklist = unicode.GetString(guildBytes, 0, guildBytes.Length).Split(' ').Select(n => ulong.Parse(n)).ToList();
                _guildOwnerBlacklist = unicode.GetString(guildOwnerBytes, 0, guildOwnerBytes.Length).Split(' ').Select(n => ulong.Parse(n)).ToList();
            }
        }

        private async Task SaveAsync()
        {
            using (FileStream userBlacklist = File.Open(@".\userblacklist.txt", FileMode.OpenOrCreate))
            using (FileStream guildBlacklist = File.Open(@".\guildblacklist.txt", FileMode.OpenOrCreate))
            using (FileStream guildOwnerBlacklist = File.Open(@".\guildownerblacklist.txt", FileMode.OpenOrCreate))
            {
                // Join each blacklist into a string separated by spaces then convert to a byte array.
                UnicodeEncoding unicode = new UnicodeEncoding();
                byte[] userBytes = unicode.GetBytes(string.Join(" ", _userBlacklist));
                byte[] guildBytes = unicode.GetBytes(string.Join(" ", _guildBlacklist));
                byte[] guildOwnerBytes = unicode.GetBytes(string.Join(" ", _guildOwnerBlacklist));

                // Write to the files.
                await userBlacklist.WriteAsync(userBytes, 0, userBytes.Length);
                await guildBlacklist.WriteAsync(guildBytes, 0, guildBytes.Length);
                await guildOwnerBlacklist.WriteAsync(guildOwnerBytes, 0, guildOwnerBytes.Length);
            }
        }
    }
}
