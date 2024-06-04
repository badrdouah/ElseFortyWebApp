using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElseForty.Models
{
    public class BugModel
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public string? id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string? name { get; set; }

        [EmailAddress]
        [Required]
        public string? email { get; set; }

        [Required]
        public string? software { get; set; }

        [Required]
        public string? subject { get; set; }

        [Required]
        public string? content { get; set; }

        public string? status { get; set; } = "Unresolved";
 
        public string? resolution { get; set; }

        public DateTime? resolutionTime { get; set; } = DateTime.MinValue;

        public DateTime? creationTime { get; set; } = DateTime.Now;
    }

}

