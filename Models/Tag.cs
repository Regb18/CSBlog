﻿using System.ComponentModel.DataAnnotations;

namespace CSBlog.Models
{
    public class Tag
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Tag Name")]
        [StringLength(40, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        public string? Name { get; set; }


        // Navigation Properties
        public virtual ICollection<BlogPost> BlogPosts { get; set; } = new HashSet<BlogPost>();
    }
}
