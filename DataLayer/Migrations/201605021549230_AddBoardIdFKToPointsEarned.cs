namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBoardIdFKToPointsEarned : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PointsEarneds", "Board_Id", "dbo.Boards");
            DropIndex("dbo.PointsEarneds", new[] { "Board_Id" });
            RenameColumn(table: "dbo.PointsEarneds", name: "Board_Id", newName: "BoardId");
            AlterColumn("dbo.PointsEarneds", "BoardId", c => c.Int(nullable: false));
            CreateIndex("dbo.PointsEarneds", "BoardId");
            AddForeignKey("dbo.PointsEarneds", "BoardId", "dbo.Boards", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PointsEarneds", "BoardId", "dbo.Boards");
            DropIndex("dbo.PointsEarneds", new[] { "BoardId" });
            AlterColumn("dbo.PointsEarneds", "BoardId", c => c.Int());
            RenameColumn(table: "dbo.PointsEarneds", name: "BoardId", newName: "Board_Id");
            CreateIndex("dbo.PointsEarneds", "Board_Id");
            AddForeignKey("dbo.PointsEarneds", "Board_Id", "dbo.Boards", "Id");
        }
    }
}
