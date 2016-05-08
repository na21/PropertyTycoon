using Newtonsoft.Json;
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
        [JsonIgnoreAttribute]
        public virtual ICollection<BoardUser> BoardUsers { get; set; }

        [JsonIgnoreAttribute]
        public virtual ICollection<Friends> Friendships { get; set; }

        public int SkillPoints { get; set; }

        public virtual ICollection<PointsEarned> PointsEarned { get; set; }

        [JsonIgnoreAttribute]
        public virtual IEnumerable<Board> Boards
        {
            get
            {
                if (BoardUsers == null)
                    return null;
                else
                    return BoardUsers.Select(bu => bu.Board);
            }
        }

        [JsonIgnoreAttribute]
        public virtual IEnumerable<User> Friends
        {
            get
            {
                if (Friendships == null)
                    return null;
                else
                    return Friendships.Select(f => f.User1);
            }
        }
    }
}
