using Discord;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wycademy
{
    /// <summary>
    /// Stores bot commmands and their responses.
    /// </summary>
    class CommandCache : ICollection<KeyValuePair<CachedMessage, CachedMessage>>
    {
        #region Constructors
        /// <summary>
        /// Create a new instance with the default capacity.
        /// </summary>
        public CommandCache() : this(200)
        {
        }
        /// <summary>
        /// Create a new instance with the specified capacity.
        /// </summary>
        /// <param name="capacity">32-bit integer that represents the maximum capacity.</param>
        public CommandCache(int capacity)
        {
            _items = new List<KeyValuePair<CachedMessage, CachedMessage>>();
            _capacity = capacity;
            _pruneOldMessages = new Timer(PruneOldMessages, _items, 0, 7200000);
        }
        #endregion

        #region ICollection Implementation
        /// <summary>
        /// Returns the number of command:response pairs currently stored.
        /// </summary>
        public int Count
        {
            get
            {
                return _items.Count;
            }
        }
        /// <summary>
        /// Only here because it has to be.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// Adds a new command:response pair to the cache.
        /// </summary>
        /// <param name="item">A KeyValuePair representing the ID of the command message and the ID of the response.</param>
        public void Add(KeyValuePair<CachedMessage, CachedMessage> item)
        {
            // If the cache is full, remove the first item before appending the new item.
            if (_items.Count >= _capacity)
            {
                _items.RemoveAt(0);
                _items.Add(item);
                return;
            }
            _items.Add(item);
        }
        /// <summary>
        /// Clears the cache.
        /// </summary>
        public void Clear()
        {
            _items.Clear();
        }
        /// <summary>
        /// Checks if the cache contains the specified KeyValuePair.
        /// </summary>
        /// <param name="item">The KeyValuePair to search for.</param>
        /// <returns>Returns true if the item is found, otherwise returns false.</returns>
        public bool Contains(KeyValuePair<CachedMessage, CachedMessage> item)
        {
            return _items.Contains(item);
        }
        /// <summary>
        /// Copies the specified elements to an array.
        /// </summary>
        /// <param name="array">The array to copy to.</param>
        /// <param name="arrayIndex">The starting point of the copy.</param>
        public void CopyTo(KeyValuePair<CachedMessage, CachedMessage>[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }
        /// <summary>
        /// Removes the specified KeyValuePair from the cache.
        /// </summary>
        /// <param name="item">The KeyValuePair to remove.</param>
        /// <returns>Returns true if the remove operation was successful. Otherwise returns false.</returns>
        public bool Remove(KeyValuePair<CachedMessage, CachedMessage> item)
        {
            return _items.Remove(item);
        }
        /// <summary>
        /// Gets the enumerator for the collection.
        /// </summary>
        /// <returns>IEnumerator</returns>
        public IEnumerator<KeyValuePair<CachedMessage, CachedMessage>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }
        /// <summary>
        /// Gets the enumerator for the collection.
        /// </summary>
        /// <returns>IEnumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds a new command:response pair to the cache.
        /// </summary>
        /// <param name="command">The ID of the command message.</param>
        /// <param name="response">The ID of the response message.</param>
        public void Add(Message command, Message response)
        {
            if (_items.Count >= _capacity)
            {
                _items.RemoveAt(0);
                _items.Add(new KeyValuePair<CachedMessage, CachedMessage>(new CachedMessage(command.Id, command.Timestamp), 
                    new CachedMessage(response.Id, response.Timestamp)));
                return;
            }
            _items.Add(new KeyValuePair<CachedMessage, CachedMessage>(new CachedMessage(command.Id, command.Timestamp), new CachedMessage(response.Id, response.Timestamp)));
        }

        /// <summary>
        /// Checks whether the cache contains a specific key.
        /// </summary>
        /// <param name="key">The key to search for.</param>
        /// <returns>True if the key is found, false otherwise.</returns>
        public bool ContainsID(ulong key)
        {
            return _items.Select(x => x.Key.ID).Contains(key);
        }

        /// <summary>
        /// Removes an item from the cache by key.
        /// </summary>
        /// <param name="key">The key to remove by.</param>
        /// <exception cref="System.ArgumentException">Thrown if the key is not found.</exception>
        public void RemoveByID(ulong key)
        {
            var itemToRemove = _items.FirstOrDefault(x => x.Key.ID == key);
            if (itemToRemove.Equals(default(KeyValuePair<CachedMessage, CachedMessage>)))
            {
                throw new ArgumentException("The specified key was not found.");
            }
            else
            {
                _items.Remove(itemToRemove);
            }
        }

        private void PruneOldMessages(object state)
        {
            var items = (List<KeyValuePair<CachedMessage, CachedMessage>>)state;
            var oldMessages = items.Where(x => (DateTime.Now - x.Key.Timestamp).TotalHours > 2).Select(x => x.Key.ID);

            foreach (var msg in oldMessages)
            {
                RemoveByID(msg);
            }
        }
        #endregion

        #region Properties
        public int MaxCapacity
        {
            get { return _capacity; }
        }
        #endregion

        #region Indexers
        public ulong this[ulong key]
        {
            get
            {
                var pair = _items.FirstOrDefault(x => x.Key.ID == key);
                if (pair.Equals(default(KeyValuePair<CachedMessage, CachedMessage>)))
                {
                    throw new ArgumentException("The specified key was not found.");
                }
                return pair.Value.ID;
            }
        }
        #endregion

        #region Private Members
        private List<KeyValuePair<CachedMessage, CachedMessage>> _items;
        private int _capacity;
        private Timer _pruneOldMessages;
        #endregion
    }
}
