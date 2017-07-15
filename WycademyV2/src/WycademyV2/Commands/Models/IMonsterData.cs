using System;
using System.Collections.Generic;
using System.Text;

namespace WycademyV2.Commands.Models
{
    public interface IMonsterData
    {
        IDictionary<string, string> Values { get; }
    }
}
