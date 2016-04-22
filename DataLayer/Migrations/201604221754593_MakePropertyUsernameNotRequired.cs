namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakePropertyUsernameNotRequired : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Properties", "UserName", "dbo.Users");
            DropIndex("dbo.Properties", new[] { "UserName" });
            AlterColumn("dbo.Properties", "UserName", c => c.String(maxLength: 128));
            CreateIndex("dbo.Properties", "UserName");
            AddForeignKey("dbo.Properties", "UserName", "dbo.Users", "UserName");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Properties", "UserName", "dbo.Users");
            DropIndex("dbo.Properties", new[] { "UserName" });
            AlterColumn("dbo.Properties", "UserName", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Properties", "UserName");
            AddForeignKey("dbo.Properties", "UserName", "dbo.Users", "UserName", cascadeDelete: true);
        }
    }
}
