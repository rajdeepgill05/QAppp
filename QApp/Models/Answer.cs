using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QApp.Models
{
    public class Answer
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public int  QuestionId { get; set; }
        public virtual Question Question { get; set; }
        public int Votes { get; set; }
        public virtual ICollection<CommentOnAnswer> Comments { get; set; }
    }
}