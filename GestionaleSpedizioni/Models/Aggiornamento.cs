using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GestionaleSpedizioni.Models
{
    public class Aggiornamento
    {
        [Display(Name = "Id aggiornamento")]
        public int IdAggiornamento { get; set; }

        public string Stato { get; set; }
        [Required]

        public string Luogo { get; set; }

        public string Descrizione { get; set; }


        [Display(Name ="Data aggiornamento")]
        public DateTime DataAggiornamento { get; set; }
        [Display(Name ="Id identificativo addetto")]
        public int IdLogin { get; set; }
        public int IdSpedizione { get; set; }

    }
}