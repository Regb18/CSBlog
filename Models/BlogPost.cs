
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CSBlog.Models
{
    public class BlogPost
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        public string? Title { get; set; }

        [StringLength(600, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        public string? Abstract { get; set; }

        [Required]
        public string? Content { get; set; }

        // DataType.DateTime will show time also
        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Updated { get; set; }

        // make this required
        [Required]
        public string? Slug { get; set; }

        [Display(Name = "Deleted?")]
        public Boolean IsDeleted { get; set; }

        [Display(Name = "Published?")]
        public Boolean IsPublished { get; set; }


        public byte[]? ImageData { get; set; }
        public string? ImageType { get; set; }

        [NotMapped]
        public virtual IFormFile? ImageFile { get; set; }

        // navigation properties

        // foreign key Many-to-1 - 1 category can have many blog posts
        public int CategoryId { get; set; }

        [JsonIgnore]
        public virtual Category? Category { get; set; }

        // Many-to-Many
        [JsonIgnore]
        public virtual ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();

        // 1-to-Many 1 Blog post can have many comments
        [JsonIgnore]
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();

    }
}
