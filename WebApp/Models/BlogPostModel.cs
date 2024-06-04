using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElseForty.Models
{
    public class BlogPostModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public string? id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string? title { get; set; } 

        [Required]
        public string? content { get; set; }  

        [Required]
        public string? author { get; set; } 

        [Required]
        public DateTime? creationDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime? modificationDate { get; set; } = DateTime.Now;

        public string? resolution { get; set; }

        public DateTime? resolutionTime { get; set; }
    }
}

