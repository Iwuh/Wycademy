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
        #endregion

        #region Methods
        public bool ContainsKey(TKey key)
        {
            return _items.Select(x => x.Key).Contains(key);
        }
        public void Add(TKey key, TValue value)
        {
            _items.Add(new KeyValuePair<TKey, TValue>(key, value));
        }
        public void RemoveByKey(TKey key)
        {
            var itemToRemove = _items.FirstOrDefault(x => x.Key == key);
            _items.Remove(itemToRemove);
        }
        #endregion

        #region Private Members
        private int _capacity;
        private List<KeyValuePair<TKey, TValue>> _items = new List<KeyValuePair<TKey, TValue>>();
        private bool _readOnly = false;
        #endregion
    }
}
