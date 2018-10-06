using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Wycademy.Commands.Models
{
    public class Monster
    {
        public int Id { get; set; }

        [Column("web_name")]
        public string WebName { get; set; }

        [Column("proper_name")]
        public string TitleName { get; set; }

        public List<Hitzone> Hitzones { get; set; }
        public List<Status> Status { get; set; }
        public List<Item> Items { get; set; }
        public List<StaggerZone> Stagger { get; set; }
    }
}
