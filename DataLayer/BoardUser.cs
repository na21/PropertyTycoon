using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DataLayer
{
    public class BoardUser
    {
        [Key, Column(Order = 1)]
        public int BoardId { get; set; }

        [Key, Column(Order = 2)]
        public string UserName { get; set; }

        [JsonIgnoreAttribute]
        [ForeignKey("UserName")]
        public virtual User User { get; set; }

        [JsonIgnoreAttribute]
        [ForeignKey("BoardId")]
        public virtual Board Board { get; set; }

        public int Turn { get; set; }

        public int Position { get; set; }

        public int Money { get; set; }

        public bool InJail { get; set; }

        public bool HasGetOutOfJail { get; set; }

        public int TurnsRemaining { get; set; }
    }
}
