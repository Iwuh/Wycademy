using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WycademyV2.Commands.Services
{
    public class CommandCacheService : IDictionary<ulong, HashSet<ulong>>, IDisposable
    {
        private int _maxCapacity;
        private Dictionary<ulong, HashSet<ulong>> _cache;
        private Timer _purgeOldMessages;

        /// <summary>
        /// Initialize the class, with a maximum capacity for the cache.
        /// </summary>
        /// <param name="capacity">The maximum amount of command messages to cache at once.</param>
        public CommandCacheService(DiscordSocketClient client, int capacity = 200)
        {
            _maxCapacity = capacity;
            _cache = new Dictionary<ulong, HashSet<ulong>>();

            _purgeOldMessages = new Timer(_ =>
            {
                // Lock the cache to ensure thread-safety while the callback is executing, as Timer executes its callback on another thread.
                lock (_cache)
                {
                    /*
                     * Get all messages where the timestamp is older than 2 hours. Then convert it to a list. The reason for this is that
                     * Where is lazy, and the elements of the IEnumerable are merely references to the elements of the original collection.
                     * So, iterating over the query result and removing each element from the original collection will throw an exception.
                     * By using ToList, the elements are copied over to a new collection, and thus will not throw an exception.
                     */
                    var purge = _cache.Where(p =>
                    {
                        // The timestamp of a message can be calculated by getting the leftmost 42 bits of the ID, then
                        // adding January 1, 2015 as a Unix timestamp.
                        DateTimeOffset timestamp = DateTimeOffset.FromUnixTimeMilliseconds((long)((p.Key >> 22) + 1420070400000UL));
                        TimeSpan difference = DateTimeOffset.UtcNow - timestamp;

                        return difference.TotalHours >= 2.0;
                    }).ToList();

                    foreach (var item in purge)
                    {
                        Remove(item);
                    }
                }
            }, null, 7200000, 7200000);

            client.MessageDeleted += async (cacheable, channel) =>
            {
                if (ContainsKey(cacheable.Id))
                {
                    var messages = this[cacheable.Id];

                    foreach (var messageId in messages)
                    {
                        try
                        {
                            var message = await channel.GetMessageAsync(messageId);
                        }
                        catch (NullReferenceException)
                        {
                            // If we get here the message was already deleted and there's nothing we can do.
                        }
                    }

                    Remove(cacheable.Id);
                }
            };
        }

        /// <summary>
        /// Gets the maximum amount of command:response pairs to store before old ones are pushed off the end.
        /// </summary>
        public int MaxCapacity => _maxCapacity;

        /// <summary>
        /// Gets a list of all keys in the cache.
        /// </summary>
        public ICollection<ulong> Keys => _cache.Keys;

        /// <summary>
        /// Gets a list of all values in the cache.
        /// </summary>
        public ICollection<HashSet<ulong>> Values => _cache.Values;

        /// <summary>
        /// Gets the amount of items in the cache.
        /// </summary>
        public int Count => _cache.Count;

        /// <summary>
        /// Only here because it has to be. Why would you want a command cache to be read-only?
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets or sets a value of a key.
        /// </summary>
        /// <param name="key">The key to search for.</param>
        /// <returns>The values of the passed key.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the key is not found.</exception>
        public HashSet<ulong> this[ulong key]
        {
            get
            {
                return _cache[key];
            }
            set
            {
                _cache[key] = value;
            }
        }

        /// <summary>
        /// Adds a new command:response set to the cache.
        /// </summary>
        /// <param name="key">The ID of the command message.</param>
        /// <param name="value">The ID of the response message.</param>
        public void Add(ulong key, HashSet<ulong> values)
        {
            if (_cache.Count >= _maxCapacity)
            {
                // Always remove at least one item.
                int itemsToRemove = (_cache.Count - _maxCapacity) + 1;
                // The leftmost 44 bits of an ID represent the timestamp.
                var orderedKeys = Keys.OrderBy(k => k >> 22).ToList();
                for (int i = 0; i < itemsToRemove; i++)
                {
                    Remove(orderedKeys[i]);
                }
            }

            _cache.Add(key, values);
        }

        /// <summary>
        /// Adds a command:response set to the cache.
        /// </summary>
        /// <param name="item">The KeyValuePair to add.</param>
        public void Add(KeyValuePair<ulong, HashSet<ulong>> item) => Add(item.Key, item.Value);

        /// <summary>
        /// Adds a new value to an existing key, or adds a new key:value set if it doesn't exist. Attempting to add a value that is already associated with the key will silently fail.
        /// </summary>
        /// <param name="key">The key to add or update.</param>
        /// <param name="value">The value to insert.</param>
        public void Add(ulong key, ulong value)
        {
            if (ContainsKey(key))
            {
                _cache[key].Add(value);
            }
            else
            {
                Add(key, new HashSet<ulong>() { value });
            }
        }

        /// <summary>
        /// Adds a new key and several values to the cache.
        /// </summary>
        /// <param name="key">The key to use.</param>
        /// <param name="values">One or more values to associate with the key.</param>
        public void Add(ulong key, params ulong[] values)
        {
            Add(key, new HashSet<ulong>(values));
        }

        /// <summary>
        /// Checks whether or not the cache contains values for a specified key.
        /// </summary>
        /// <param name="key">The key to check for.</param>
        /// <returns>true if the key is found, otherwise false.</returns>
        public bool ContainsKey(ulong key) => _cache.Keys.Contains(key);

        /// <summary>
        /// Removes a command:response set from the cache by key.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        /// <returns>true if the removal was successful, false otherwise.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the key is not found.</exception>
        public bool Remove(ulong key) => _cache.Remove(key);

        /// <summary>
        /// What do *you* think TryGetValue does?
        /// </summary>
        /// <param name="key">The key to search for.</param>
        /// <param name="value">The variable to store the result in.</param>
        /// <returns>true if the key was found, false if it wasn't (value will be 0).</returns>
        public bool TryGetValue(ulong key, out HashSet<ulong> value) => _cache.TryGetValue(key, out value);

        /// <summary>
        /// Remove everything from the cache. You monster.
        /// </summary>
        public void Clear() => _cache.Clear();

        /// <summary>
        /// Checks if the cache contains a key value pair. But tbh who's actually going to use this one?
        /// </summary>
        /// <param name="item">The KeyValuePair to search for.</param>
        /// <returns>Whether or not the cache contains the item.</returns>
        public bool Contains(KeyValuePair<ulong, HashSet<ulong>> item) => _cache.Contains(item);

        /// <summary>
        /// Copies a range of values to an array.
        /// </summary>
        /// <param name="array">The array to copy to.</param>
        /// <param name="arrayIndex">The index to begin copying from.</param>
        public void CopyTo(KeyValuePair<ulong, HashSet<ulong>>[] array, int arrayIndex) => ((IDictionary<ulong, HashSet<ulong>>)_cache).CopyTo(array, arrayIndex);

        /// <summary>
        /// Remove a set from the cache.
        /// </summary>
        /// <param name="item">The set to remove.</param>
        /// <returns>Whether or not the remove operation completed successfully.</returns>
        public bool Remove(KeyValuePair<ulong, HashSet<ulong>> item) => _cache.Remove(item.Key);

        /// <summary>
        /// Gets the enumerator for this object.
        /// </summary>
        /// <returns>The IEnumerator for this instance of CommandCacheService.</returns>
        public IEnumerator<KeyValuePair<ulong, HashSet<ulong>>> GetEnumerator()
        {
            return _cache.GetEnumerator();
        }

        /// <summary>
        /// A non-generic version of GetEnumerator.
        /// </summary>
        /// <returns>An IEnumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Safely disposes of the internal timer that clears old messages from the cache.
        /// </summary>
        public void Dispose()
        {
            _purgeOldMessages.Dispose();
            _purgeOldMessages = null;
        }
    }
}
