namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPointsEarnedToBoard : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PointsEarneds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreatedAt = c.DateTime(nullable: false),
                        Points = c.Int(nullable: false),
                        Board_Id = c.Int(),
                        User_UserName = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Boards", t => t.Board_Id)
                .ForeignKey("dbo.Users", t => t.User_UserName)
                .Index(t => t.Board_Id)
                .Index(t => t.User_UserName);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PointsEarneds", "User_UserName", "dbo.Users");
            DropForeignKey("dbo.PointsEarneds", "Board_Id", "dbo.Boards");
            DropIndex("dbo.PointsEarneds", new[] { "User_UserName" });
            DropIndex("dbo.PointsEarneds", new[] { "Board_Id" });
            DropTable("dbo.PointsEarneds");
        }
    }
}
