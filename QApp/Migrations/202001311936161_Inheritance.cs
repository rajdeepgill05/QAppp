namespace QApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Inheritance : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CommentOnAnswers", "Text", c => c.String());
            AddColumn("dbo.CommentOnAnswers", "Created", c => c.DateTime(nullable: false));
            AddColumn("dbo.CommentOnAnswers", "UserId", c => c.String(maxLength: 128));
            AddColumn("dbo.CommentsOnQuestions", "Text", c => c.String());
            AddColumn("dbo.CommentsOnQuestions", "Created", c => c.DateTime(nullable: false));
            AddColumn("dbo.CommentsOnQuestions", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.CommentOnAnswers", "UserId");
            CreateIndex("dbo.CommentsOnQuestions", "UserId");
            AddForeignKey("dbo.CommentsOnQuestions", "UserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.CommentOnAnswers", "UserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CommentOnAnswers", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.CommentsOnQuestions", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.CommentsOnQuestions", new[] { "UserId" });
            DropIndex("dbo.CommentOnAnswers", new[] { "UserId" });
            DropColumn("dbo.CommentsOnQuestions", "UserId");
            DropColumn("dbo.CommentsOnQuestions", "Created");
            DropColumn("dbo.CommentsOnQuestions", "Text");
            DropColumn("dbo.CommentOnAnswers", "UserId");
            DropColumn("dbo.CommentOnAnswers", "Created");
            DropColumn("dbo.CommentOnAnswers", "Text");
        }
    }
}
