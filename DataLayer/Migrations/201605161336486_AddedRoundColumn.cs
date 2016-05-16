namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedRoundColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Boards", "MaximumRounds", c => c.Int(nullable: false));
            AddColumn("dbo.BoardUsers", "Rounds", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BoardUsers", "Rounds");
            DropColumn("dbo.Boards", "MaximumRounds");
        }
    }
}
