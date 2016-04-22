namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMaxPlayersToBoard : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Boards",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MaximumPlayers = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BoardUsers",
                c => new
                    {
                        BoardId = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 128),
                        Turn = c.Int(nullable: false),
                        Money = c.Int(nullable: false),
                        InJail = c.Boolean(nullable: false),
                        TurnsRemaining = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.BoardId, t.UserName })
                .ForeignKey("dbo.Boards", t => t.BoardId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserName, cascadeDelete: true)
                .Index(t => t.BoardId)
                .Index(t => t.UserName);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserName = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.UserName);
            
            CreateTable(
                "dbo.Moves",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BoardId = c.Int(nullable: false),
                        CurrentPos = c.Int(nullable: false),
                        Roll = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Boards", t => t.BoardId, cascadeDelete: true)
                .Index(t => t.BoardId);
            
            CreateTable(
                "dbo.Properties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BoardId = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        NumHotels = c.Int(nullable: false),
                        NumHouses = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Boards", t => t.BoardId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserName, cascadeDelete: true)
                .Index(t => t.BoardId)
                .Index(t => t.UserName);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Properties", "UserName", "dbo.Users");
            DropForeignKey("dbo.Properties", "BoardId", "dbo.Boards");
            DropForeignKey("dbo.Moves", "BoardId", "dbo.Boards");
            DropForeignKey("dbo.BoardUsers", "UserName", "dbo.Users");
            DropForeignKey("dbo.BoardUsers", "BoardId", "dbo.Boards");
            DropIndex("dbo.Properties", new[] { "UserName" });
            DropIndex("dbo.Properties", new[] { "BoardId" });
            DropIndex("dbo.Moves", new[] { "BoardId" });
            DropIndex("dbo.BoardUsers", new[] { "UserName" });
            DropIndex("dbo.BoardUsers", new[] { "BoardId" });
            DropTable("dbo.Properties");
            DropTable("dbo.Moves");
            DropTable("dbo.Users");
            DropTable("dbo.BoardUsers");
            DropTable("dbo.Boards");
        }
    }
}
