using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WycademyV2.Commands.Models
{
    public class Item : IMonsterData
    {
        public int Id { get; set; }
        [Column("monster_id")]
        public int MonsterId { get; set; }

        [Column("name")]
        public string Name { get; set; }
        [Column("game")]
        public string Game { get; set; }

        [Column("normal")]
        public string Normal { get; set; }
        [Column("enraged")]
        public string Enraged { get; set; }
        [Column("fatigued")]
        public string Fatigued { get; set; }

        public IDictionary<string, string> Values => new Dictionary<string, string>()
        {
            { "Normal", Normal },
            { "Enraged", Enraged },
            { "Fatigued", Fatigued }
        };

        public Monster Monster { get; set; }
    }
}
