using System;
using System.Collections.Generic;
using System.Text;

namespace WycademyV2.Commands.Models
{
    public interface IMonsterData
    {
        string Name { get; set; }
        string Game { get; set; }
        IEnumerable<string> Values { get; }
    }
}
