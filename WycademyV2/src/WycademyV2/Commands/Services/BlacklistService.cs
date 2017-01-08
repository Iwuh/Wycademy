using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WycademyV2.Commands.Enums;

namespace WycademyV2.Commands.Services
{
    public class BlacklistService
    {
        private List<ulong> _userBlacklist;
        private List<ulong> _serverBlacklist;
        private List<ulong> _serverOwnerBlacklist;

        public BlacklistService()
        {
            _userBlacklist = new List<ulong>();
            _serverBlacklist = new List<ulong>();
            _serverOwnerBlacklist = new List<ulong>();
        }

        /// <summary>
        /// Adds an ID to a blacklist, if it's not already there.
        /// </summary>
        /// <param name="id">The ID to add.</param>
        /// <param name="category">The category to add the ID to.</param>
        public void AddToBlackList(ulong id, BlacklistType category)
        {
            switch (category)
            {
                case BlacklistType.User:
                    if (!CheckBlacklist(id, BlacklistType.User)) _userBlacklist.Add(id);
                    break;
                case BlacklistType.Server:
                    if (!CheckBlacklist(id, BlacklistType.Server)) _serverBlacklist.Add(id);
                    break;
                case BlacklistType.ServerOwner:
                    if (!CheckBlacklist(id, BlacklistType.ServerOwner)) _serverOwnerBlacklist.Add(id);
                    break;
            }
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
                case BlacklistType.Server:
                    blacklistToCheck = _serverBlacklist;
                    break;
                case BlacklistType.ServerOwner:
                    blacklistToCheck = _serverOwnerBlacklist;
                    break;
            }

            // This will never be null, because category will never not be one of the 3 blacklist types.
            return blacklistToCheck.Contains(id);
        }
    }
}
