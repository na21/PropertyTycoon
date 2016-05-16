namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBadges : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Badges",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(nullable: false, maxLength: 128),
                        Date = c.DateTime(nullable: false),
                        Name = c.String(nullable: false),
                        Desc = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserName, cascadeDelete: true)
                .Index(t => t.UserName);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Badges", "UserName", "dbo.Users");
            DropIndex("dbo.Badges", new[] { "UserName" });
            DropTable("dbo.Badges");
        }
    }
}
