using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class BoardUser
    {
        [Key]
        public string UserName { get; set; }

        [Key]
        public int BoardId { get; set; }

        public virtual User User { get; set; }

        public virtual Board Board { get; set; }

        public int Money { get; set; }

        public bool InJail { get; set; }

        public int TurnsRemaining { get; set; }
    }
}
