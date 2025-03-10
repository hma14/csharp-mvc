namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_ProcessType_ApprovedCapability : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ApprovedCapabilities", "ProcessType", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ApprovedCapabilities", "ProcessType");
        }
    }
}
