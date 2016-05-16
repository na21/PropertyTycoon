using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class GameInvitation
    {
        [Key]
        public string UserName { get; set; }

        public User InvitedUser1 { get; set; }
        public User InvitedUser2 { get; set; }
        public User InvitedUser3 { get; set; }

        public virtual Board Board { get; set; }
    }
}
