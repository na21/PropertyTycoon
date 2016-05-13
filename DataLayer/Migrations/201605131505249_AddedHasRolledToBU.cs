namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedHasRolledToBU : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BoardUsers", "HasRolled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BoardUsers", "HasRolled");
        }
    }
}
