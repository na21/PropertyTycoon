namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedUserNameToMove : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Moves", "UserName", c => c.String(maxLength: 128));
            CreateIndex("dbo.Moves", "UserName");
            AddForeignKey("dbo.Moves", "UserName", "dbo.Users", "UserName");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Moves", "UserName", "dbo.Users");
            DropIndex("dbo.Moves", new[] { "UserName" });
            DropColumn("dbo.Moves", "UserName");
        }
    }
}
