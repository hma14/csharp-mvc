namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_RevisionNumber_BidRequestRevision : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BidRequestRevisions", "RevisionNumber", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BidRequestRevisions", "RevisionNumber");
        }
    }
}
