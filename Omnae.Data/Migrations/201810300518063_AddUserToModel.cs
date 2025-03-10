namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserToModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Documents", "CreatedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.Documents", "ModifiedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.Products", "CreatedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.Products", "ModifiedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.PartRevisions", "CreatedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.PartRevisions", "ModifiedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.PriceBreaks", "CreatedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.PriceBreaks", "ModifiedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.TaskDatas", "CreatedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.TaskDatas", "ModifiedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.Orders", "CreatedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.Orders", "ModifiedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.RFQBids", "CreatedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.RFQBids", "ModifiedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.OrderStateTrackings", "CreatedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.OrderStateTrackings", "ModifiedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.ProductStateTrackings", "CreatedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.ProductStateTrackings", "ModifiedByUserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Documents", "CreatedByUserId");
            CreateIndex("dbo.Documents", "ModifiedByUserId");
            CreateIndex("dbo.Products", "CreatedByUserId");
            CreateIndex("dbo.Products", "ModifiedByUserId");
            CreateIndex("dbo.PartRevisions", "CreatedByUserId");
            CreateIndex("dbo.PartRevisions", "ModifiedByUserId");
            CreateIndex("dbo.PriceBreaks", "CreatedByUserId");
            CreateIndex("dbo.PriceBreaks", "ModifiedByUserId");
            CreateIndex("dbo.TaskDatas", "CreatedByUserId");
            CreateIndex("dbo.TaskDatas", "ModifiedByUserId");
            CreateIndex("dbo.Orders", "CreatedByUserId");
            CreateIndex("dbo.Orders", "ModifiedByUserId");
            CreateIndex("dbo.RFQBids", "CreatedByUserId");
            CreateIndex("dbo.RFQBids", "ModifiedByUserId");
            CreateIndex("dbo.OrderStateTrackings", "CreatedByUserId");
            CreateIndex("dbo.OrderStateTrackings", "ModifiedByUserId");
            CreateIndex("dbo.ProductStateTrackings", "CreatedByUserId");
            CreateIndex("dbo.ProductStateTrackings", "ModifiedByUserId");
            AddForeignKey("dbo.Documents", "CreatedByUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Documents", "ModifiedByUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Products", "CreatedByUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Products", "ModifiedByUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.PartRevisions", "CreatedByUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.PartRevisions", "ModifiedByUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.PriceBreaks", "CreatedByUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.PriceBreaks", "ModifiedByUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.TaskDatas", "CreatedByUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.TaskDatas", "ModifiedByUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Orders", "CreatedByUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Orders", "ModifiedByUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.RFQBids", "CreatedByUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.RFQBids", "ModifiedByUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.OrderStateTrackings", "CreatedByUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.OrderStateTrackings", "ModifiedByUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.ProductStateTrackings", "CreatedByUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.ProductStateTrackings", "ModifiedByUserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductStateTrackings", "ModifiedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ProductStateTrackings", "CreatedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.OrderStateTrackings", "ModifiedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.OrderStateTrackings", "CreatedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.RFQBids", "ModifiedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.RFQBids", "CreatedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Orders", "ModifiedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Orders", "CreatedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TaskDatas", "ModifiedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TaskDatas", "CreatedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.PriceBreaks", "ModifiedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.PriceBreaks", "CreatedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.PartRevisions", "ModifiedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.PartRevisions", "CreatedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Products", "ModifiedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Products", "CreatedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Documents", "ModifiedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Documents", "CreatedByUserId", "dbo.AspNetUsers");
            DropIndex("dbo.ProductStateTrackings", new[] { "ModifiedByUserId" });
            DropIndex("dbo.ProductStateTrackings", new[] { "CreatedByUserId" });
            DropIndex("dbo.OrderStateTrackings", new[] { "ModifiedByUserId" });
            DropIndex("dbo.OrderStateTrackings", new[] { "CreatedByUserId" });
            DropIndex("dbo.RFQBids", new[] { "ModifiedByUserId" });
            DropIndex("dbo.RFQBids", new[] { "CreatedByUserId" });
            DropIndex("dbo.Orders", new[] { "ModifiedByUserId" });
            DropIndex("dbo.Orders", new[] { "CreatedByUserId" });
            DropIndex("dbo.TaskDatas", new[] { "ModifiedByUserId" });
            DropIndex("dbo.TaskDatas", new[] { "CreatedByUserId" });
            DropIndex("dbo.PriceBreaks", new[] { "ModifiedByUserId" });
            DropIndex("dbo.PriceBreaks", new[] { "CreatedByUserId" });
            DropIndex("dbo.PartRevisions", new[] { "ModifiedByUserId" });
            DropIndex("dbo.PartRevisions", new[] { "CreatedByUserId" });
            DropIndex("dbo.Products", new[] { "ModifiedByUserId" });
            DropIndex("dbo.Products", new[] { "CreatedByUserId" });
            DropIndex("dbo.Documents", new[] { "ModifiedByUserId" });
            DropIndex("dbo.Documents", new[] { "CreatedByUserId" });
            DropColumn("dbo.ProductStateTrackings", "ModifiedByUserId");
            DropColumn("dbo.ProductStateTrackings", "CreatedByUserId");
            DropColumn("dbo.OrderStateTrackings", "ModifiedByUserId");
            DropColumn("dbo.OrderStateTrackings", "CreatedByUserId");
            DropColumn("dbo.RFQBids", "ModifiedByUserId");
            DropColumn("dbo.RFQBids", "CreatedByUserId");
            DropColumn("dbo.Orders", "ModifiedByUserId");
            DropColumn("dbo.Orders", "CreatedByUserId");
            DropColumn("dbo.TaskDatas", "ModifiedByUserId");
            DropColumn("dbo.TaskDatas", "CreatedByUserId");
            DropColumn("dbo.PriceBreaks", "ModifiedByUserId");
            DropColumn("dbo.PriceBreaks", "CreatedByUserId");
            DropColumn("dbo.PartRevisions", "ModifiedByUserId");
            DropColumn("dbo.PartRevisions", "CreatedByUserId");
            DropColumn("dbo.Products", "ModifiedByUserId");
            DropColumn("dbo.Products", "CreatedByUserId");
            DropColumn("dbo.Documents", "ModifiedByUserId");
            DropColumn("dbo.Documents", "CreatedByUserId");
        }
    }
}
