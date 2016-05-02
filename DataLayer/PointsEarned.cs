using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class PointsEarned
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string UserName { get; set; }

        [ForeignKey("UserName")]
        public User User { get; set; }

        public int BoardId { get; set; }

        [ForeignKey("BoardId")]
        public Board Board { get; set; }

        public DateTime CreatedAt { get; set; }

        public int Points { get; set; }
    }
}
