namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedisEnterpriseTaskData : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TaskDatas", "isEnterprise", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TaskDatas", "isEnterprise");
        }
    }
}
