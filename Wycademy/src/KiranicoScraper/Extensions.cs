using Newtonsoft.Json.Linq;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KiranicoScraper
{
    static class Extensions
    {
        /// <summary>
        /// Removes a range of tokens from a <see cref="JArray"/>, from <paramref name="startIndex"/> up to but not including <paramref name="stopIndex"/>.
        /// </summary>
        /// <param name="startIndex">The beginning index of the range to be removed.</param>
        /// <param name="stopIndex">The ending index of the range to be removed. All indices from <paramref name="startIndex"/> up to this one will be removed, however this one will not be removed. If omitted, all elements are removed until the end of the sequence.</param>
        public static void RemoveRange(this JArray array, int startIndex, int? stopIndex = null)
        {
            var end = stopIndex ?? array.Count;

            for (int i = startIndex; i < end; i++)
            {
                // As items are removed from the middle of the array, all items to the right will "shift" to the left, decreasing their index.
                // We only have to remove startIndex a certain amount of times to effectively remove all items from startIndex until the end.
                array.RemoveAt(startIndex);
            }
        }

        /// <summary>
        /// Executes an action for every element of an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="IEnumerable{T}"/>'s elements.</typeparam>
        /// <param name="action">The action that will be executed with each element.</param>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
            {
                action(item);
            }
        }

        /// <summary>
        /// Swaps the positions of two elements in an <see cref="IList{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the list's elements.</typeparam>
        /// <param name="index1">The index of the first element.</param>
        /// <param name="index2">The index of the second element.</param>
        public static void Swap<T>(this IList<T> list, int index1, int index2)
        {
            var temp = list[index1];
            list[index1] = list[index2];
            list[index2] = temp;
        }

        /// <summary>
        /// Adds a number of <see cref="NpgsqlParameter"/> to an <see cref="NpgsqlCommand"/>.
        /// </summary>
        /// <param name="parameters">The parameters to add. Can be <see cref="NpgsqlParameter"/> or any derived type.</param>
        public static void AddParameters(this NpgsqlCommand cmd, IEnumerable<NpgsqlParameter> parameters)
        {
            foreach (var parameter in parameters)
            {
                cmd.Parameters.Add(parameter);
            }
        }

        /// <summary>
        /// Adds a number of <see cref="NpgsqlParameter"/> to an <see cref="NpgsqlCommand"/>.
        /// </summary>
        /// <param name="parameters">The parameters to add. Can be <see cref="NpgsqlParameter"/> or any derived type.</param>
        public static void AddParameters(this NpgsqlCommand cmd, params NpgsqlParameter[] parameters)
            => cmd.AddParameters(parameters as IEnumerable<NpgsqlParameter>);
    }
}
