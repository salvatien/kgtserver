using System;
using System.Collections.Generic;
using System.Text;

namespace Dogs.ViewModels.Data.Models
{
    public class CommentModel
    {
        public int CommentId { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public DateTime Date { get; set; }
        public string Content { get; set; }
        public int TrainingId { get; set; }
        //set if its a dog training comment, not set for training comment
        public int? DogId { get; set; }
    }
}