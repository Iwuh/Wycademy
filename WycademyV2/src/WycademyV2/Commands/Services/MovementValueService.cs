using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WycademyV2.Commands.Services
{
    /// <summary>
    /// Provides the movement values for weapons.
    /// </summary>
    public class MovementValueService
    {
        /// <summary>
        /// Used for statistics.
        /// </summary>
        public int Queries { get; private set; }

        /// <summary>
        /// Get the file containing the movement values for a specific weapon.
        /// </summary>
        /// <param name="name">The name of the weapon to search for.</param>
        /// <returns>Task(FileStream)</returns>
        public async Task<FileStream> GetMovementValueStream(string name)
        {

        }
    }
}
