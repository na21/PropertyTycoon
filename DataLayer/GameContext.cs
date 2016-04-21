using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class GameContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Board> Boards { get; set; }

        public virtual DbSet<BoardUser> BoardUsers { get; set; }
    }
}
