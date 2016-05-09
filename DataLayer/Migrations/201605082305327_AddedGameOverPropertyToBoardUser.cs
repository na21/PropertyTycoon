namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedGameOverPropertyToBoardUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BoardUsers", "GameOver", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BoardUsers", "GameOver");
        }
    }
}
