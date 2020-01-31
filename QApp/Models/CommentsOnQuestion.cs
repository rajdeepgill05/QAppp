namespace QApp.Models
{
    public class CommentsOnQuestion :Comment
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public virtual Question Question { get; set; }
    }
}