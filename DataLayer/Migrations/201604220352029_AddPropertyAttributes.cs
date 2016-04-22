namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPropertyAttributes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Properties", "Position", c => c.Int(nullable: false));
            AddColumn("dbo.Properties", "Rent", c => c.Int(nullable: false));
            AddColumn("dbo.Properties", "Price", c => c.Int(nullable: false));
            AddColumn("dbo.Properties", "Group", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Properties", "Group");
            DropColumn("dbo.Properties", "Price");
            DropColumn("dbo.Properties", "Rent");
            DropColumn("dbo.Properties", "Position");
        }
    }
}
