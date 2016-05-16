using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class Stat
    {
        [Key]
        public string UserName { get; set; }

        [ForeignKey("UserName")]
        public virtual User User { get; set; }

        public int GamesCreated { get; set; }

        public int GamesJoined { get; set; }

        public int GamesForfeit { get; set; }
    }
}
