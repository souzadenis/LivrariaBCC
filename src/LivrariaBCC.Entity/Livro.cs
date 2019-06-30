using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LivrariaBCC.Entity
{
    public class Livro
    {
        public int Id { get; set; }
        public string ISBN { get; set; }
        public string Autor { get; set; }
        public string Nome { get; set; }
        public double Preco { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataPublicacao { get; set; }
        public string ImagemCapa { get; set; }
    }
}