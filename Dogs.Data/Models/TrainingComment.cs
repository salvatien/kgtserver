using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dogs.Data.Models
{
    [Table("TrainingComments")]
    public class TrainingComment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int TrainingCommentId { get; set; }
        [Required]
        public int TrainingId { get; set; }
        public virtual Training Training { get; set; }
        [Required]
        public int AuthorId { get; set; }
        public virtual Guide Author { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string Content { get; set; }
    }
}
