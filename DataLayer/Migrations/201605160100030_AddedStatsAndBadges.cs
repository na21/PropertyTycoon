namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedStatsAndBadges : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Stats",
                c => new
                    {
                        UserName = c.String(nullable: false, maxLength: 128),
                        GamesCreated = c.Int(nullable: false),
                        GamesJoined = c.Int(nullable: false),
                        GamesForfeit = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserName)
                .ForeignKey("dbo.Users", t => t.UserName)
                .Index(t => t.UserName);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Stats", "UserName", "dbo.Users");
            DropIndex("dbo.Stats", new[] { "UserName" });
            DropTable("dbo.Stats");
        }
    }
}
