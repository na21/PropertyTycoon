namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddActiveBoardPlayerToBoard : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Boards", "ActiveBoardPlayer_UserName", c => c.String(maxLength: 128));
            CreateIndex("dbo.Boards", "ActiveBoardPlayer_UserName");
            AddForeignKey("dbo.Boards", "ActiveBoardPlayer_UserName", "dbo.Users", "UserName");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Boards", "ActiveBoardPlayer_UserName", "dbo.Users");
            DropIndex("dbo.Boards", new[] { "ActiveBoardPlayer_UserName" });
            DropColumn("dbo.Boards", "ActiveBoardPlayer_UserName");
        }
    }
}
