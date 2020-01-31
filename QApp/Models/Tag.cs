using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QApp.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public virtual ICollection<QuestionTag> QuestionTags { get; set; }
    }
}