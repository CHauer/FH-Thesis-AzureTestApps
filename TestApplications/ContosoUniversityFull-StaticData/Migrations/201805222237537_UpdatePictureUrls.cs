namespace ContosoUniversityFull.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePictureUrls : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Picture", "StoragePath", c => c.String());
            AddColumn("dbo.Picture", "OriginalUrl", c => c.String());
            AddColumn("dbo.Picture", "PictureUrl", c => c.String());
            AddColumn("dbo.Picture", "ThumbnailUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Picture", "ThumbnailUrl");
            DropColumn("dbo.Picture", "PictureUrl");
            DropColumn("dbo.Picture", "OriginalUrl");
            DropColumn("dbo.Picture", "StoragePath");
        }
    }
}
