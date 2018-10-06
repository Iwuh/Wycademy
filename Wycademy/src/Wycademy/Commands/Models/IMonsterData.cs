using System;
using System.Collections.Generic;
using System.Text;

namespace Wycademy.Commands.Models
{
    public interface IMonsterData
    {
        string Name { get; set; }
        string Game { get; set; }
        IEnumerable<string> Values { get; }
    }
}
