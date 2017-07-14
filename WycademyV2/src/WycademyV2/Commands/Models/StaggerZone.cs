using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WycademyV2.Commands.Models
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

        public IDictionary<string, string> Values => new Dictionary<string, string>()
        {
            { "Stagger", Stagger },
            { "Sever", Sever },
            { "Extract Colour", Extract }
        };

        public Monster Monster { get; set; }
    }
}
