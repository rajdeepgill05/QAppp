namespace QApp.Models
{
    public class CommentOnAnswer :Comment
    {
        public int Id { get; set; }
        public  int AnswerId { get; set; }
        public virtual Answer Answer { get; set; }
    }
}