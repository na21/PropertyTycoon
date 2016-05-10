namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIsFirstMove : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Moves", "IsFirstMove", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Moves", "IsFirstMove");
        }
    }
}
