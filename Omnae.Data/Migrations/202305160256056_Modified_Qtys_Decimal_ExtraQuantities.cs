namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modified_Qtys_Decimal_ExtraQuantities : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ExtraQuantities", "Qty1", c => c.Decimal(precision: 18, scale: 3));
            AlterColumn("dbo.ExtraQuantities", "Qty2", c => c.Decimal(precision: 18, scale: 3));
            AlterColumn("dbo.ExtraQuantities", "Qty3", c => c.Decimal(precision: 18, scale: 3));
            AlterColumn("dbo.ExtraQuantities", "Qty4", c => c.Decimal(precision: 18, scale: 3));
            AlterColumn("dbo.ExtraQuantities", "Qty5", c => c.Decimal(precision: 18, scale: 3));
            AlterColumn("dbo.ExtraQuantities", "Qty6", c => c.Decimal(precision: 18, scale: 3));
            AlterColumn("dbo.ExtraQuantities", "Qty7", c => c.Decimal(precision: 18, scale: 3));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ExtraQuantities", "Qty7", c => c.Int());
            AlterColumn("dbo.ExtraQuantities", "Qty6", c => c.Int());
            AlterColumn("dbo.ExtraQuantities", "Qty5", c => c.Int());
            AlterColumn("dbo.ExtraQuantities", "Qty4", c => c.Int());
            AlterColumn("dbo.ExtraQuantities", "Qty3", c => c.Int());
            AlterColumn("dbo.ExtraQuantities", "Qty2", c => c.Int());
            AlterColumn("dbo.ExtraQuantities", "Qty1", c => c.Int());
        }
    }
}
