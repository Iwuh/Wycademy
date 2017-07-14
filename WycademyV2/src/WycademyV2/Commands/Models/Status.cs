using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WycademyV2.Commands.Models
{
    public class Status : IMonsterData
    {
        public int Id { get; set; }
        [Column("monster_id")]
        public int MonsterId { get; set; }

        [Column("name")]
        public string Name { get; set; }
        [Column("game")]
        public string Game { get; set; }

        [Column("initial_threshold")]
        public string Initial { get; set; }
        [Column("increase")]
        public string Increase { get; set; }
        [Column("max_threshold")]
        public string Max { get; set; }
        [Column("duration")]
        public string Duration { get; set; }
        [Column("reduction")]
        public string Reduction { get; set; }
        [Column("damage")]
        public string Damage { get; set; }

        public IDictionary<string, string> Values => new Dictionary<string, string>()
        {
            { "Initial", Initial },
            { "Increase", Increase },
            { "Maximum", Max },
            { "Duration", Duration },
            { "Reduction", Reduction },
            { "Damage", Damage }
        };

        public Monster Monster { get; set; }
    }
}
