using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WycademyV2.Commands.Services
{
    public class RandomNumberService
    {
        private Random rand;

        public RandomNumberService()
        {
            rand = new Random();
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
