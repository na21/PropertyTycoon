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
        [Display(Name = "Max Players")]
        [RangeAttribute(2, 4)]
        public int MaximumPlayers { get; set; }

        [Required]
        [Display(Name = "Max Rounds")]
        [RangeAttribute(2, 50)]
        public int MaximumRounds { get; set; }

        public virtual User ActiveBoardPlayer { get; set; }

        public virtual User Winner { get; set; }

        public virtual User Host { get; set; }
        
        public virtual ICollection<BoardUser> BoardUsers { get; set; }

        public virtual ICollection<Move> Moves { get; set; }

        public virtual ICollection<Property> Properties { get; set; }

        public virtual ICollection<PointsEarned>  PointsEarned { get; set; }

        public virtual IEnumerable<User> Users
        {
            get
            {
                if (BoardUsers == null)
                    return null;
                else
                    return BoardUsers.Select(bu => bu.User);
            }
        }
        
        public string Status { get; set; }

        [Required]
        [Display(Name = "Min Skill")]
        public int minSkillRange { get; set; }

        [Required]
        [Display(Name = "Max Skill")]
        public int maxSkillRange { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}
