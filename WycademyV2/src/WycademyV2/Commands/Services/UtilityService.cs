using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WycademyV2.Commands.Services
{
    public class UtilityService
    {
        private Random rand;

        public DateTime StartTime { get; private set; }

        public CancellationTokenSource Shutdown { get; private set; }

        public UtilityService(CancellationTokenSource cancel)
        {
            rand = new Random();
            StartTime = DateTime.Now;
            Shutdown = cancel;
        }

        public int GetRandomNumber()
        {
            return rand.Next();
        }

        public int GetRandomNumber(int max)
        {
            return rand.Next(max);
        }

        public int GetRandomNumber(int min, int max)
        {
            return rand.Next(min, max);
        }
    }
}
