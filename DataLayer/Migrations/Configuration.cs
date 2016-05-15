namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<DataLayer.GameContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(DataLayer.GameContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            
            context.Users.AddOrUpdate(x => x.UserName,
                new User() { UserName = "Joe" },
                new User() { UserName = "Paul" },
                new User() { UserName = "James" });

            context.Boards.AddOrUpdate(x => x.Id,
                new Board() { Id = 888 },
                new Board() { Id = 889 },
                new Board() { Id = 890 });

            context.UserPointsEarned.AddOrUpdate(x => x.Id,
                new PointsEarned() { Id = 100, Points = 10, UserName = "Joe", CreatedAt = DateTime.Now, BoardId = 888},
                new PointsEarned() { Id = 101, Points = 20, UserName = "Paul", CreatedAt = DateTime.Now, BoardId = 889 },
                new PointsEarned() { Id = 102, Points = 30, UserName = "James", CreatedAt = DateTime.Now, BoardId = 890 },
                new PointsEarned() { Id = 103, Points = 40, UserName = "Joe", CreatedAt = DateTime.Now, BoardId = 888 },
                new PointsEarned() { Id = 104, Points = 50, UserName = "Paul", CreatedAt = DateTime.Now, BoardId = 889 },
                new PointsEarned() { Id = 105, Points = 60, UserName = "James", CreatedAt = DateTime.Now, BoardId = 890 },
                new PointsEarned() { Id = 106, Points = 70, UserName = "Joe", CreatedAt = DateTime.Now, BoardId = 888 },
                new PointsEarned() { Id = 107, Points = 80, UserName = "Paul", CreatedAt = DateTime.Now, BoardId = 889 },
                new PointsEarned() { Id = 108, Points = 90, UserName = "James", CreatedAt = DateTime.Now, BoardId = 890 },
                new PointsEarned() { Id = 109, Points = 100, UserName = "Joe", CreatedAt = DateTime.Now, BoardId = 888 }
                );
        }
    }
}
