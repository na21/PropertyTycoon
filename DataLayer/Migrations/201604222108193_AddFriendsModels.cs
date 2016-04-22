namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFriendsModels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Friends",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName1 = c.String(maxLength: 128),
                        UserName2 = c.String(maxLength: 128),
                        User_UserName = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserName1)
                .ForeignKey("dbo.Users", t => t.UserName2)
                .ForeignKey("dbo.Users", t => t.User_UserName)
                .Index(t => t.UserName1)
                .Index(t => t.UserName2)
                .Index(t => t.User_UserName);
            
            AddColumn("dbo.Properties", "Mortgaged", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Friends", "User_UserName", "dbo.Users");
            DropForeignKey("dbo.Friends", "UserName2", "dbo.Users");
            DropForeignKey("dbo.Friends", "UserName1", "dbo.Users");
            DropIndex("dbo.Friends", new[] { "User_UserName" });
            DropIndex("dbo.Friends", new[] { "UserName2" });
            DropIndex("dbo.Friends", new[] { "UserName1" });
            DropColumn("dbo.Properties", "Mortgaged");
            DropTable("dbo.Friends");
        }
    }
}
