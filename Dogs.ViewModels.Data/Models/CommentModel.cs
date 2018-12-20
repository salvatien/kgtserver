using System;
using System.Collections.Generic;
using System.Text;

namespace Dogs.ViewModels.Data.Models
{
    class CommentModel
    {
        public int AuthorId { get; set; }
        public DateTime Date { get; set; }
        public string Content { get; set; }
    }
}
