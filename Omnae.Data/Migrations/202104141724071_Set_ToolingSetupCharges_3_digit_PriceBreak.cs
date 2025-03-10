namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Set_ToolingSetupCharges_3_digit_PriceBreak1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PriceBreaks", "ToolingSetupCharges", c => c.Decimal(precision: 18, scale: 3));
            AlterColumn("dbo.PriceBreaks", "CustomerToolingSetupCharges", c => c.Decimal(precision: 18, scale: 3));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PriceBreaks", "CustomerToolingSetupCharges", c => c.Decimal(precision: 9, scale: 3));
            AlterColumn("dbo.PriceBreaks", "ToolingSetupCharges", c => c.Decimal(precision: 9, scale: 3));
        }
    }
}
