namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRoleData : DbMigration
    {
        public override void Up()
        {
            Sql(@"
                IF NOT EXISTS (SELECT 1 FROM [dbo].[AspNetRoles] WHERE id = 'SystemAdmin')
				BEGIN
                    INSERT INTO [dbo].[AspNetRoles] ([Id],[Name]) VALUES ('SystemAdmin', 'Administrator');
                END
                IF NOT EXISTS (SELECT 1 FROM [dbo].[AspNetRoles] WHERE id = 'CompanyAdmin')
				BEGIN
                    INSERT INTO [dbo].[AspNetRoles] ([Id],[Name]) VALUES ('CompanyAdmin', 'Company Administrator');
                END
                IF NOT EXISTS (SELECT 1 FROM [dbo].[AspNetRoles] WHERE id = 'CompanyUser')
				BEGIN
                    INSERT INTO [dbo].[AspNetRoles] ([Id],[Name]) VALUES ('CompanyUser', 'Company User');
                END");
        }
        
        public override void Down()
        {
            Sql(@"
                DELETE FROM [dbo].[AspNetRoles] WHERE [Id] = 'SystemAdmin';
                DELETE FROM [dbo].[AspNetRoles] WHERE [Id] = 'CompanyAdmin';
                DELETE FROM [dbo].[AspNetRoles] WHERE [Id] = 'CompanyUser';");
        }
    }
}
