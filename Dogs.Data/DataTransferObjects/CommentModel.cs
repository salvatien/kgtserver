using System;
using System.ComponentModel.DataAnnotations;

namespace Dogs.Data.DataTransferObjects
{
    public class CommentModel
    {
        public int CommentId { get; set; }
        [Required]
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public int TrainingId { get; set; }
        //set if its a dog training comment, not set for training comment
        public int? DogId { get; set; }
    }
}