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

        [MaxLength(2)]
        public string [] InvitedUsers { get; set; }

        [MaxLength(2)]
        public bool [] IsAccepted { get; set; }

        public virtual Board Board { get; set; }
    }
}
