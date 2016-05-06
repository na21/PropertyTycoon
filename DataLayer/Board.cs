using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class Board
    {
        public static int PassGoMoney = 200;
        public static int GoToJailPosition = 31;
        public static int JailPosition = 11;
        public static int StartinSkillPoints = 100;
        public static int LowestSkillPoints = 50;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int MaximumPlayers { get; set; }

        public User ActiveBoardPlayer { get; set; }

        public User Winner { get; set; }
        
        public virtual ICollection<BoardUser> BoardUsers { get; set; }

        public virtual ICollection<Move> Moves { get; set; }

        public virtual ICollection<Property> Properties { get; set; }

        public virtual ICollection<PointsEarned>  PointsEarned { get; set; }

        public virtual IEnumerable<User> Users
        {
            get
            {
                return BoardUsers.Select(bu => bu.User);
            }
        }
        
        public string Status { get; set; }

        [Required]
        public int minSkillRange { get; set; }

        [Required]
        public int maxSkillRange { get; set; }
    }
}
