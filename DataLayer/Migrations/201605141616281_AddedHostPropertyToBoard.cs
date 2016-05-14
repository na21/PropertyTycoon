namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedHostPropertyToBoard : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Boards", "Host_UserName", c => c.String(maxLength: 128));
            CreateIndex("dbo.Boards", "Host_UserName");
            AddForeignKey("dbo.Boards", "Host_UserName", "dbo.Users", "UserName");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Boards", "Host_UserName", "dbo.Users");
            DropIndex("dbo.Boards", new[] { "Host_UserName" });
            DropColumn("dbo.Boards", "Host_UserName");
        }
    }
}
