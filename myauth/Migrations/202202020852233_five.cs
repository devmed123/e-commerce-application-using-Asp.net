namespace myauth.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class five : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Produits", "Quantite", c => c.Int(nullable: false));
            DropColumn("dbo.Commandes", "Quantite");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Commandes", "Quantite", c => c.Int(nullable: false));
            DropColumn("dbo.Produits", "Quantite");
        }
    }
}
