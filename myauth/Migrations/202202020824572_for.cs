namespace myauth.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _for : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Proprietaires", "Issociete", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Proprietaires", "Issociete");
        }
    }
}
