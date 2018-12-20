using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DogsServer.Models
{
    [Table("DogTrainingComments")]
    public class DogTrainingComment
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int DogTrainingCommentId { get; set; }
        [Required]
        public int TrainingId { get; set; }
        [Required]
        public int DogId { get; set; }
        public virtual DogTraining DogTraining { get; set; }
        [Required]
        public int AuthorId { get; set; }
        public virtual Guide Author { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string Content { get; set; }
    }
}