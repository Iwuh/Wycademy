using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wycademy.Commands.Services
{
    /// <summary>
    /// Tracks a "lock" status that can be used to prevent execution of commands.
    /// </summary>
    public class LockerService
    {
        /// <summary>
        /// Tracks the lock status.
        /// </summary>
        public bool IsLocked { get; private set; }

        /// <summary>
        /// Sets the lock to true.
        /// </summary>
        public void Lock()
        {
            IsLocked = true;
        }
        /// <summary>
        /// Sets the lock to false.
        /// </summary>
        public void Unlock()
        {
            IsLocked = false;
        }
    }
}
