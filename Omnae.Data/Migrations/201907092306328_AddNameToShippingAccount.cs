namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNameToShippingAccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShippingAccounts", "Name", c => c.String(nullable: true, maxLength: 100));

            Sql(@"
                    UPDATE [dbo].[ShippingAccounts]
                       SET [Name] = '' + isNull([Carrier], '') + CASE
									                    WHEN [CarrierType] = 0 THEN ''
									                    WHEN [CarrierType] = 1 THEN ' - Air'
									                    WHEN [CarrierType] = 2 THEN ' - Ocean'
									                    WHEN [CarrierType] = 3 THEN ' - Land'
								                    END");

            AlterColumn("dbo.ShippingAccounts", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShippingAccounts", "Name");
        }
    }
}
