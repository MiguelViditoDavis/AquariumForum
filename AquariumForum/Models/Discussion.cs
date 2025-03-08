using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace AquariumForum.Models
{
    public class Discussion
    {
        public int DiscussionId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public string ImageFilename { get; set; }
        public DateTime CreateDate { get; set; }

        // Set [Required] for UserId
        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }

        // Navigation Property (No [Required] here, EF Core handles it)
        public IdentityUser User { get; set; }

        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
