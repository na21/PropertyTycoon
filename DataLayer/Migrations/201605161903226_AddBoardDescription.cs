namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBoardDescription : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Boards", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Boards", "Description");
        }
    }
}
