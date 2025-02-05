using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace AquariumForum.Models
{
    public class Discussion
    {
        public int DiscussionId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImageFilename { get; set; }
        public DateTime CreateDate { get; set; }

        // Navigation Property: A discussion can have many comments
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
