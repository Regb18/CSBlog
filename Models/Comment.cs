using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace CSBlog.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Comment")]
        [StringLength(4000, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        public string? Body { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Updated { get; set; }

        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        public string? UpdateReason { get; set; }

        // Navigation Properties

        // foreign key 1-to-1 1 comment belongs to 1 blog post
        public int BlogPostId { get; set; }

        // BlogPost and author will only be included if we use .Include to add them, otherwise they have to be null since we don't need them included
        // Need the BlogPost object so the BlogPostId knows what to connect to in the database
        public virtual BlogPost? BlogPost { get; set; }


        // foreign key 1-to-many 1 blog user can have many comments
        [Required]
        public string? AuthorId { get; set; }
        public virtual BlogUser? Author { get; set; }
    }
}
