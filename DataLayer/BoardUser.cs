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
        [Required]
        public string UserName { get; set; }

        [ForeignKey("UserName")]
        public virtual User User { get; set; }

        [Required]
        public int BoardId { get; set; }

        [ForeignKey("BoardId")]
        public virtual Board Board { get; set; }

        public int Money { get; set; }
    }
}
