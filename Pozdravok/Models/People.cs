using System.ComponentModel.DataAnnotations;

namespace Pozdravok.Models
{
    public class People
    {
        [Key]
        public int Id { get; set; }
   
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Photo { get; set; } 
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; } 
        public string Note { get; set; }
    }
}

