namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Docs_BidRequestRevision : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Documents", name: "BidRequestRevision_Id", newName: "BidRequestRevisionId");
            RenameIndex(table: "dbo.Documents", name: "IX_BidRequestRevision_Id", newName: "IX_BidRequestRevisionId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Documents", name: "IX_BidRequestRevisionId", newName: "IX_BidRequestRevision_Id");
            RenameColumn(table: "dbo.Documents", name: "BidRequestRevisionId", newName: "BidRequestRevision_Id");
        }
    }
}
