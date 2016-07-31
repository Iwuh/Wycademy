using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wycademy
{
    class MonsterInfo
    {
        public string Category { get; private set; }
        public Dictionary<string, string[]> Data;

        public MonsterInfo(string category)
        {
            Category = category;
        }
    }
}
