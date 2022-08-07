using System.ComponentModel.DataAnnotations;

namespace Dogs.Data.DataTransferObjects
{
    public class IdNameModel
    {
        public int Id { get; set; }
        [Display(Name = "Przewodnik")]      //TODO check if this is used to display only Guide name
        public string Name { get; set; }
    }
}
