namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedDescriptionToMove : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Moves", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Moves", "Description");
        }
    }
}
