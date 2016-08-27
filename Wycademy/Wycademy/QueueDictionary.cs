using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wycademy
{
    /// <summary>
    /// Represents a Dictionary that has a fixed max size and retains insertion order.
    /// </summary>
    /// <typeparam name="TKey">Represents the type of the keys.</typeparam>
    /// <typeparam name="TValue">Represents the type of the values.</typeparam>
    class QueueDictionary<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>>
        where TKey : class
        where TValue : class
    {
        #region Constructors
        public QueueDictionary() : this(200)
        {
        }
        public QueueDictionary(int capacity)
        {
            // Sets the maximum capacity of the instance.
            _capacity = capacity;
        }
        #endregion

        #region ICollection Implementation
        public int Count
        {
            get
            {
                return _items.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return _readOnly;
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            if (_items.Count >= Capacity)
            {
                _items.RemoveAt(0);
                _items.Add(item);
                return;
            }
            _items.Add(item);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return _items.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region Properties
        public int Capacity
        {
            get { return _capacity; }
        }
        public IEnumerable<TKey> Keys
        {
            get { return _items.Select(x => x.Key); }
        }
        public IEnumerable<TValue> Values
        {
            get { return _items.Select(x => x.Value); }
        }
        #endregion

        #region Methods
        public bool ContainsKey(TKey key)
        {
            // Returns true if any keys in the list match the argument.
            return _items.Select(x => x.Key).Contains(key);
        }
        public void Add(TKey key, TValue value)
        {
            if (_items.Count >= _capacity)
            {
                _items.RemoveAt(0);
                _items.Add(new KeyValuePair<TKey, TValue>(key, value));
                return;
            }
            _items.Add(new KeyValuePair<TKey, TValue>(key, value));
        }
        public void RemoveByKey(TKey key)
        {
            var itemToRemove = _items.FirstOrDefault(x => x.Key == key);
            if (itemToRemove.Equals(default(KeyValuePair<TKey, TValue>)))
            {
                // Throws an exception if the found item is the default value of a KeyValuePair (i.e.: The key was not found in _items).
                throw new ArgumentException("The specified key was not found.");
            }
            else
            {
                _items.Remove(itemToRemove);
            }
        }
        #endregion

        #region Indexers
        public TValue this[TKey key]
        {
            get
            {
                var pair = _items.FirstOrDefault(x => x.Key == key);
                if (pair.Equals(default(KeyValuePair<TKey, TValue>)))
                {
                    throw new ArgumentException("The specified key was not found.");
                }
                else
                {
                    return pair.Value;
                }
            }
        }
        public TValue this[int index]
        {
            get { return _items[index].Value; }
        }
        #endregion

        #region Private Members
        private int _capacity;
        private List<KeyValuePair<TKey, TValue>> _items = new List<KeyValuePair<TKey, TValue>>();
        private bool _readOnly = false;
        #endregion
    }
}
