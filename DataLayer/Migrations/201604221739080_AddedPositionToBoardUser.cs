namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPositionToBoardUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BoardUsers", "Position", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BoardUsers", "Position");
        }
    }
}
