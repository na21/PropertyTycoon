namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPointsEarned : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Boards", "Status", c => c.String());
            AddColumn("dbo.Boards", "minSkillRange", c => c.Int(nullable: false));
            AddColumn("dbo.Boards", "maxSkillRange", c => c.Int(nullable: false));
            AddColumn("dbo.Boards", "Winner_UserName", c => c.String(maxLength: 128));
            AddColumn("dbo.Users", "SkillPoints", c => c.Int(nullable: false));
            CreateIndex("dbo.Boards", "Winner_UserName");
            AddForeignKey("dbo.Boards", "Winner_UserName", "dbo.Users", "UserName");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Boards", "Winner_UserName", "dbo.Users");
            DropIndex("dbo.Boards", new[] { "Winner_UserName" });
            DropColumn("dbo.Users", "SkillPoints");
            DropColumn("dbo.Boards", "Winner_UserName");
            DropColumn("dbo.Boards", "maxSkillRange");
            DropColumn("dbo.Boards", "minSkillRange");
            DropColumn("dbo.Boards", "Status");
        }
    }
}
