using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WycademyV2.Commands.Services
{
    public class CommandCacheService : IDictionary<ulong, ulong>
    {
        private int _maxCapacity;
        private List<KeyValuePair<ulong, ulong>> _cache;

        /// <summary>
        /// Initialize the class, with a maximum capacity for the cache.
        /// </summary>
        /// <param name="capacity">The maximum amount of command messages to cache at once.</param>
        public CommandCacheService(int capacity = 200)
        {
            _maxCapacity = capacity;
            _cache = new List<KeyValuePair<ulong, ulong>>();
        }

        /// <summary>
        /// Gets the maximum amount of command:response pairs to store before old ones are pushed off the end.
        /// </summary>
        public int MaxCapacity
        {
            get { return _maxCapacity; }
        }

        /// <summary>
        /// Gets a list of all keys in the cache.
        /// </summary>
        public ICollection<ulong> Keys
        {
            get { return _cache.Select(x => x.Key).ToList(); }
        }

        /// <summary>
        /// Gets a list of all values in the cache.
        /// </summary>
        public ICollection<ulong> Values
        {
            get { return _cache.Select(x => x.Value).ToList(); }
        }

        /// <summary>
        /// Gets the amount of items in the cache.
        /// </summary>
        public int Count
        {
            get { return _cache.Count; }
        }

        /// <summary>
        /// Only here because it has to be. Why would you want a command cache to be read-only?
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets or sets a value of a key.
        /// </summary>
        /// <param name="key">The key to search for.</param>
        /// <returns>The value of the passed key.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the key is not found.</exception>
        public ulong this[ulong key]
        {
            get
            {
                var pair = _cache.FirstOrDefault(x => x.Key == key);

                if (!pair.Equals(default(KeyValuePair<ulong, ulong>)))
                {
                    return pair.Value;
                }
                throw new KeyNotFoundException($"The key {key} was not found.");
            }
            set
            {
                var pair = _cache.FirstOrDefault(x => x.Key == key);

                if (!pair.Equals(default(KeyValuePair<ulong, ulong>)))
                {
                    int index = _cache.IndexOf(pair);
                    _cache.Remove(pair);
                    _cache.Insert(index, new KeyValuePair<ulong, ulong>(pair.Key, value));
                }
                throw new KeyNotFoundException($"The key {key} was not found.");
            }
        }

        /// <summary>
        /// Adds a new command:response pair to the cache.
        /// </summary>
        /// <param name="key">The ID of the command message.</param>
        /// <param name="value">The ID of the response message.</param>
        public void Add(ulong key, ulong value)
        {
            Add(new KeyValuePair<ulong, ulong>(key, value));
        }

        /// <summary>
        /// Checks whether or not the cache contains a value for a specified key.
        /// </summary>
        /// <param name="key">The key to check for.</param>
        /// <returns>true if the key is found, otherwise false.</returns>
        public bool ContainsKey(ulong key)
        {
            return _cache.Select(x => x.Key).Contains(key);
        }

        /// <summary>
        /// Removes a command:response pair from the cache by key.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        /// <returns>true if the removal was successful, false otherwise.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the key is not found.</exception>
        public bool Remove(ulong key)
        {
            var pair = _cache.FirstOrDefault(x => x.Key == key);

            if (!pair.Equals(default(KeyValuePair<ulong, ulong>)))
            {
                return _cache.Remove(pair);
            }
            throw new KeyNotFoundException($"The key {key} was not found.");
        }

        /// <summary>
        /// What do *you* think TryGetValue does?
        /// </summary>
        /// <param name="key">The key to search for.</param>
        /// <param name="value">The variable to store the result in.</param>
        /// <returns>true if the key was found, false if it wasn't (value will be 0).</returns>
        public bool TryGetValue(ulong key, out ulong value)
        {
            var pair = _cache.FirstOrDefault(x => x.Key == key);

            if (!pair.Equals(default(KeyValuePair<ulong, ulong>)))
            {
                value = pair.Value;
                return true;
            }
            value = 0;
            return false;
        }

        /// <summary>
        /// Adds a command:response pair to the cache.
        /// </summary>
        /// <param name="item">The KeyValuePair to add.</param>
        public void Add(KeyValuePair<ulong, ulong> item)
        {
            if (_cache.Count >= _maxCapacity)
            {
                // Find how many elements need to be removed to bring the count to _maxCapacity - 1.
                int i = 0;
                do
                {
                    // i can be incremented once without checking the condition because this block will only be entered if the number of items is greater than or equal to the max.
                    i++;
                } while (_cache.Count - i >= _maxCapacity);
                _cache.RemoveRange(0, i);
            }
            _cache.Add(item);
        }

        /// <summary>
        /// Remove everything from the cache. You monster.
        /// </summary>
        public void Clear()
        {
            _cache.Clear();
        }

        /// <summary>
        /// Checks if the cache contains a key value pair. But tbh who's actually going to use this one?
        /// </summary>
        /// <param name="item">The KeyValuePair to search for.</param>
        /// <returns>Whether or not the cache contains the item.</returns>
        public bool Contains(KeyValuePair<ulong, ulong> item)
        {
            return _cache.Contains(item);
        }

        /// <summary>
        /// Copies a range of items to an array (why though).
        /// </summary>
        /// <param name="array">The destination of the copy.</param>
        /// <param name="arrayIndex">Where to start copying.</param>
        public void CopyTo(KeyValuePair<ulong, ulong>[] array, int arrayIndex)
        {
            _cache.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes a key value pair from the cache. But again, who's actually going to do this with a KeyValuePair?
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>Whether or not the removal operation was successful.</returns>
        public bool Remove(KeyValuePair<ulong, ulong> item)
        {
            return _cache.Remove(item);
        }

        /// <summary>
        /// Gets the enumerator for this object.
        /// </summary>
        /// <returns>The IEnumerator for this instance of CommandCacheService.</returns>
        public IEnumerator<KeyValuePair<ulong, ulong>> GetEnumerator()
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
    }
}
