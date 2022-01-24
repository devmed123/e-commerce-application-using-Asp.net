namespace myauth.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sec : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Proprietaires", "IsFavorite", c => c.Boolean(nullable: false));
            AddColumn("dbo.Proprietaires", "IsNoir", c => c.Boolean(nullable: false));
            DropColumn("dbo.Proprietaires", "NumTel");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Proprietaires", "NumTel", c => c.String(nullable: false));
            DropColumn("dbo.Proprietaires", "IsNoir");
            DropColumn("dbo.Proprietaires", "IsFavorite");
        }
    }
}
