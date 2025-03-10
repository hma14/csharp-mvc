namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_StripeQbo_Table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StripeQboes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QboId = c.String(),
                        StripeInvoiceId = c.String(),
                        QboInvoiceId = c.String(),
                        QboInvoiceNumber = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.StripeQboes");
        }
    }
}
