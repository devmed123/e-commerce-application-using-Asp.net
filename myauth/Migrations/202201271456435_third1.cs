namespace myauth.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class third1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Admins", "Last_connection", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Admins", "Last_connection");
        }
    }
}
