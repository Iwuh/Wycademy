using System;
using System.Collections.Generic;
using System.Text;

namespace Wycademy.Commands.Entities
{
    public class EvalEnvironment
    {
        private Dictionary<string, object> _storage;

        public EvalEnvironment()
        {
            _storage = new Dictionary<string, object>();
        }

        /// <summary>
        /// Adds a new object to the environment with a name to retrieve it later.
        /// </summary>
        /// <param name="name">The name of the object that will be used to retrieve it.</param>
        /// <param name="value">The object to store.</param>
        /// <exception cref="ArgumentException">Thrown if an object is already in the environment with the same name as <paramref name="name"/>.</exception>
        public void Add(string name, object value) => _storage.Add(name, value);

        /// <summary>
        /// Retrieves an object from the environment by its name.
        /// </summary>
        /// <param name="name">The name of the object to retrieve.</param>
        /// <returns>The object stored with <paramref name="name"/> as its name.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if <paramref name="name"/> is not the name of any object in the environment.</exception>
        public object Get(string name) => _storage[name];

        /// <summary>
        /// Retrieves an object from the environment by its name and attempts to cast it to a type.
        /// </summary>
        /// <typeparam name="T">The type that the object should be cast to.</typeparam>
        /// <param name="name">The name of the object to retrieve.</param>
        /// <returns>The object as <typeparamref name="T"/>, or <see cref="default(T)"/> if the cast was unsuccessful.</returns>
        /// <exception cref="KeyNotFoundException"><paramref name="name"/> is not the name of any object in the environment.</exception>
        public T GetTyped<T>(string name)
        {
            object obj = _storage[name];
            try
            {
                return (T)obj;
            }
            catch (InvalidCastException)
            {
                return default(T);
            }
        }

        /// <summary>
        /// Clears all objects from the environment.
        /// </summary>
        public void Clear() => _storage.Clear();

        /// <summary>
        /// Removes an object from the environment by its name.
        /// </summary>
        /// <param name="name">The name of the object to remove.</param>
        /// <returns>Whether or not the removal was successful.</returns>
        public bool Remove(string name) => _storage.Remove(name);

        /// <summary>
        /// Sets or retrieves an object from the environment.
        /// </summary>
        /// <param name="name">The object's name.</param>
        public object this[string name]
        {
            get => _storage[name];
            set => _storage[name] = value;
        }
    }
}
