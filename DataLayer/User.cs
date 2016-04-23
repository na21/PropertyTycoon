using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class User
    {
        [Key]
        public string UserName { get; set; }

        public ICollection<BoardUser> BoardUsers { get; set; }

        public ICollection<Friends> Friendships { get; set; }

        public int SkillPoints { get; set; }

        public ICollection<PointsEarned> PointsEarned { get; set; }

        public IEnumerable<Board> Boards
        {
            get
            {
                return BoardUsers.Select(bu => bu.Board);
            }
        }

        public IEnumerable<User> Friends
        {
            get
            {
                return Friendships.Select(f => f.User1);
            }
        }
    }
}
