using System;
using System.Collections.Generic;
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
        public void AddToBlacklist(ulong id, BlacklistType category)
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
    }
}
