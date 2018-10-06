using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Wycademy.Commands.Models
{
    public class StaggerZone : IMonsterData
    {
        public int Id { get; set; }
        [Column("monster_id")]
        public int MonsterId { get; set; }

        [Column("name")]
        public string Name { get; set; }
        [Column("game")]
        public string Game { get; set; }

        [Column("stagger_value")]
        public string Stagger { get; set; }
        [Column("sever_value")]
        public string Sever { get; set; }
        [Column("extract_colour")]
        public string Extract { get; set; }

        [NotMapped]
        public IEnumerable<string> Values => new List<string>() { Stagger, Sever, Extract };

        public Monster Monster { get; set; }
    }
}
