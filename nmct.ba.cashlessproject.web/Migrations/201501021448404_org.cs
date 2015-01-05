namespace nmct.ba.cashlessproject.web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class org : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Organisations", "Login", c => c.String(nullable: false));
            AlterColumn("dbo.Organisations", "Password", c => c.String(nullable: false));
            AlterColumn("dbo.Organisations", "DbName", c => c.String(nullable: false));
            AlterColumn("dbo.Organisations", "DbLogin", c => c.String(nullable: false));
            AlterColumn("dbo.Organisations", "DbPassword", c => c.String(nullable: false));
            AlterColumn("dbo.Organisations", "OrganisationName", c => c.String(nullable: false));
            AlterColumn("dbo.Organisations", "Address", c => c.String(nullable: false));
            AlterColumn("dbo.Organisations", "Email", c => c.String(nullable: false));
            AlterColumn("dbo.Organisations", "Phone", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Organisations", "Phone", c => c.String());
            AlterColumn("dbo.Organisations", "Email", c => c.String());
            AlterColumn("dbo.Organisations", "Address", c => c.String());
            AlterColumn("dbo.Organisations", "OrganisationName", c => c.String());
            AlterColumn("dbo.Organisations", "DbPassword", c => c.String());
            AlterColumn("dbo.Organisations", "DbLogin", c => c.String());
            AlterColumn("dbo.Organisations", "DbName", c => c.String());
            AlterColumn("dbo.Organisations", "Password", c => c.String());
            AlterColumn("dbo.Organisations", "Login", c => c.String());
        }
    }
}
