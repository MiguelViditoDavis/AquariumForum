using System;

namespace AquariumForum.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string Content { get; set; }
        public DateTime CreateDate { get; set; }

        // Foreign Key
        public int DiscussionId { get; set; }

        // Make the navigation property nullable to avoid validation issues
        public Discussion? Discussion { get; set; }
    }
}
