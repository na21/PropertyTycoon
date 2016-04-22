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

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int MaximumPlayers { get; set; }

        public User ActiveBoardPlayer { get; set; }

        public ICollection<BoardUser> BoardUsers { get; set; }

        public ICollection<Move> Moves { get; set; }

        public ICollection<Property> Properties { get; set; }

        public IEnumerable<User> Users
        {
            get
            {
                return BoardUsers.Select(bu => bu.User);
            }
        }
    }
}
