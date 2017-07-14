using System;
using System.Collections.Generic;
using System.Text;

namespace WycademyV2.Commands.Models
{
    interface IMonsterData
    {
        IDictionary<string, string> Values { get; }
    }
}
