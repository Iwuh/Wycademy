using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WycademyV2.Commands.Models
{
    public class Hitzone : IMonsterData
    {
        public int Id { get; set; }
        [Column("monster_id")]
        public int MonsterId { get; set; }

        [Column("name")]
        public string Name { get; set; }
        [Column("game")]
        public string Game { get; set; }

        [Column("cut")]
        public string Cut { get; set; }
        [Column("impact")]
        public string Impact { get; set; }
        [Column("shot")]
        public string Shot { get; set; }
        [Column("fire")]
        public string Fire { get; set; }
        [Column("water")]
        public string Water { get; set; }
        [Column("ice")]
        public string Ice { get; set; }
        [Column("thunder")]
        public string Thunder { get; set; }
        [Column("dragon")]
        public string Dragon { get; set; }

        [NotMapped]
        public IEnumerable<string> Values => new List<string>() { Cut, Impact, Shot, Fire, Water, Ice, Thunder, Dragon };

        public Monster Monster { get; set; }
    }
}
