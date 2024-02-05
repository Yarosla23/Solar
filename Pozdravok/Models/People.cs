using System.ComponentModel.DataAnnotations;

namespace Pozdravok.Models
{
    public class People
    {
        [Key]
        public int Id { get; set; }

        [Display (Name = "Введите имя")]
        [Required (ErrorMessage = "Вы забыли ввести имя")]
        public string Name { get; set; }

        [Display (Name = "Введите фамилию")]
        [Required(ErrorMessage = "Вы забыли ввести фамилию")]
        public string Surname { get; set; }

        [Display(Name = "Фото")]
        public byte[]? Photo { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Вы забыли указать дату")]
        [Display(Name = "Укажите дату")]
        public DateTime BirthDate { get; set; }

        [Display(Name = "Заметки")]
        [StringLength( 1500, ErrorMessage = "введите текст больше 5 символов")]
        public string? Note { get; set; } 
    }
}

