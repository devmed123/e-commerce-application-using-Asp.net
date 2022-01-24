namespace myauth.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class third : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Admins",
                c => new
                    {
                        IdAdmin = c.Int(nullable: false, identity: true),
                        Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.IdAdmin)
                .ForeignKey("dbo.AspNetUsers", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        IdCategorie = c.Int(nullable: false, identity: true),
                        Nom = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.IdCategorie);
            
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        IdClient = c.Int(nullable: false, identity: true),
                        Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.IdClient)
                .ForeignKey("dbo.AspNetUsers", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.ClientActivites",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Date = c.DateTime(nullable: false),
                        IdCommande = c.Int(nullable: false),
                        Client_IdClient = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.Client_IdClient)
                .ForeignKey("dbo.Commandes", t => t.IdCommande, cascadeDelete: true)
                .Index(t => t.IdCommande)
                .Index(t => t.Client_IdClient);
            
            CreateTable(
                "dbo.Commandes",
                c => new
                    {
                        IdCommande = c.Int(nullable: false, identity: true),
                        Quantite = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Client_IdClient = c.Int(),
                        Panier_IdPanier = c.Int(),
                        Produit_IdProduit = c.Int(),
                    })
                .PrimaryKey(t => t.IdCommande)
                .ForeignKey("dbo.Clients", t => t.Client_IdClient)
                .ForeignKey("dbo.Paniers", t => t.Panier_IdPanier)
                .ForeignKey("dbo.Produits", t => t.Produit_IdProduit)
                .Index(t => t.Client_IdClient)
                .Index(t => t.Panier_IdPanier)
                .Index(t => t.Produit_IdProduit);
            
            CreateTable(
                "dbo.Paniers",
                c => new
                    {
                        IdPanier = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.IdPanier);
            
            CreateTable(
                "dbo.Produits",
                c => new
                    {
                        IdProduit = c.Int(nullable: false, identity: true),
                        Prix = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateAddition = c.DateTime(nullable: false),
                        Description = c.String(nullable: false),
                        Titre = c.String(nullable: false),
                        DateLastModification = c.DateTime(nullable: false),
                        IsVerified = c.Boolean(nullable: false),
                        Proprietaire_IdPropritaire = c.Int(),
                    })
                .PrimaryKey(t => t.IdProduit)
                .ForeignKey("dbo.Proprietaires", t => t.Proprietaire_IdPropritaire)
                .Index(t => t.Proprietaire_IdPropritaire);
            
            CreateTable(
                "dbo.Commentaires",
                c => new
                    {
                        IdCommentaire = c.Int(nullable: false, identity: true),
                        Message = c.String(nullable: false),
                        Client_IdClient = c.Int(),
                        Produit_IdProduit = c.Int(),
                    })
                .PrimaryKey(t => t.IdCommentaire)
                .ForeignKey("dbo.Clients", t => t.Client_IdClient)
                .ForeignKey("dbo.Produits", t => t.Produit_IdProduit)
                .Index(t => t.Client_IdClient)
                .Index(t => t.Produit_IdProduit);
            
            CreateTable(
                "dbo.ImageProduits",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Image = c.Binary(nullable: false),
                        Produit_IdProduit = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Produits", t => t.Produit_IdProduit)
                .Index(t => t.Produit_IdProduit);
            
            CreateTable(
                "dbo.ProductCategories",
                c => new
                    {
                        IdProduit = c.Int(nullable: false),
                        IdCategorie = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.IdProduit, t.IdCategorie })
                .ForeignKey("dbo.Categories", t => t.IdCategorie, cascadeDelete: true)
                .ForeignKey("dbo.Produits", t => t.IdProduit, cascadeDelete: true)
                .Index(t => t.IdProduit)
                .Index(t => t.IdCategorie);
            
            CreateTable(
                "dbo.PropActivites",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Produit_IdProduit = c.Int(),
                        Proprietaire_IdPropritaire = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Produits", t => t.Produit_IdProduit)
                .ForeignKey("dbo.Proprietaires", t => t.Proprietaire_IdPropritaire)
                .Index(t => t.Produit_IdProduit)
                .Index(t => t.Proprietaire_IdPropritaire);
            
            CreateTable(
                "dbo.Proprietaires",
                c => new
                    {
                        IdPropritaire = c.Int(nullable: false, identity: true),
                        NumTel = c.String(nullable: false),
                        Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.IdPropritaire)
                .ForeignKey("dbo.AspNetUsers", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.ClientCategories",
                c => new
                    {
                        Client_IdClient = c.Int(nullable: false),
                        Categorie_IdCategorie = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Client_IdClient, t.Categorie_IdCategorie })
                .ForeignKey("dbo.Clients", t => t.Client_IdClient, cascadeDelete: true)
                .ForeignKey("dbo.Categories", t => t.Categorie_IdCategorie, cascadeDelete: true)
                .Index(t => t.Client_IdClient)
                .Index(t => t.Categorie_IdCategorie);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Clients", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ClientActivites", "IdCommande", "dbo.Commandes");
            DropForeignKey("dbo.Proprietaires", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.PropActivites", "Proprietaire_IdPropritaire", "dbo.Proprietaires");
            DropForeignKey("dbo.Produits", "Proprietaire_IdPropritaire", "dbo.Proprietaires");
            DropForeignKey("dbo.PropActivites", "Produit_IdProduit", "dbo.Produits");
            DropForeignKey("dbo.ProductCategories", "IdProduit", "dbo.Produits");
            DropForeignKey("dbo.ProductCategories", "IdCategorie", "dbo.Categories");
            DropForeignKey("dbo.ImageProduits", "Produit_IdProduit", "dbo.Produits");
            DropForeignKey("dbo.Commentaires", "Produit_IdProduit", "dbo.Produits");
            DropForeignKey("dbo.Commentaires", "Client_IdClient", "dbo.Clients");
            DropForeignKey("dbo.Commandes", "Produit_IdProduit", "dbo.Produits");
            DropForeignKey("dbo.Commandes", "Panier_IdPanier", "dbo.Paniers");
            DropForeignKey("dbo.Commandes", "Client_IdClient", "dbo.Clients");
            DropForeignKey("dbo.ClientActivites", "Client_IdClient", "dbo.Clients");
            DropForeignKey("dbo.ClientCategories", "Categorie_IdCategorie", "dbo.Categories");
            DropForeignKey("dbo.ClientCategories", "Client_IdClient", "dbo.Clients");
            DropForeignKey("dbo.Admins", "Id", "dbo.AspNetUsers");
            DropIndex("dbo.ClientCategories", new[] { "Categorie_IdCategorie" });
            DropIndex("dbo.ClientCategories", new[] { "Client_IdClient" });
            DropIndex("dbo.Proprietaires", new[] { "Id" });
            DropIndex("dbo.PropActivites", new[] { "Proprietaire_IdPropritaire" });
            DropIndex("dbo.PropActivites", new[] { "Produit_IdProduit" });
            DropIndex("dbo.ProductCategories", new[] { "IdCategorie" });
            DropIndex("dbo.ProductCategories", new[] { "IdProduit" });
            DropIndex("dbo.ImageProduits", new[] { "Produit_IdProduit" });
            DropIndex("dbo.Commentaires", new[] { "Produit_IdProduit" });
            DropIndex("dbo.Commentaires", new[] { "Client_IdClient" });
            DropIndex("dbo.Produits", new[] { "Proprietaire_IdPropritaire" });
            DropIndex("dbo.Commandes", new[] { "Produit_IdProduit" });
            DropIndex("dbo.Commandes", new[] { "Panier_IdPanier" });
            DropIndex("dbo.Commandes", new[] { "Client_IdClient" });
            DropIndex("dbo.ClientActivites", new[] { "Client_IdClient" });
            DropIndex("dbo.ClientActivites", new[] { "IdCommande" });
            DropIndex("dbo.Clients", new[] { "Id" });
            DropIndex("dbo.Admins", new[] { "Id" });
            DropTable("dbo.ClientCategories");
            DropTable("dbo.Proprietaires");
            DropTable("dbo.PropActivites");
            DropTable("dbo.ProductCategories");
            DropTable("dbo.ImageProduits");
            DropTable("dbo.Commentaires");
            DropTable("dbo.Produits");
            DropTable("dbo.Paniers");
            DropTable("dbo.Commandes");
            DropTable("dbo.ClientActivites");
            DropTable("dbo.Clients");
            DropTable("dbo.Categories");
            DropTable("dbo.Admins");
        }
    }
}
