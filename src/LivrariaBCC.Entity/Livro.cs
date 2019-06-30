using System;
using System.ComponentModel.DataAnnotations;

namespace LivrariaBCC.Entity
{
    public class Livro
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(17, MinimumLength = 17, ErrorMessage = "ISBN deve conter 17 caracteres")]
        public string ISBN { get; set; }
        [Required]
        public string Autor { get; set; }
        [Required]
        public string Nome { get; set; }
        [Required]
        public double Preco { get; set; }
        [Required]
        public DateTime DataPublicacao { get; set; }
        public string ImagemCapa { get; set; }
    }
}