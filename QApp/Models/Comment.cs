using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QApp.Models
{
    public class Comment
    {
        public string Text { get; set; }
        public DateTime Created { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}