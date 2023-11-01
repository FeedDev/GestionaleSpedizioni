using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace GestionaleSpedizioni.Models
{
    public class Spedizione
    {
        [Display(Name = "Id spedizione")]
        [Required]


        public int IdSpedizione { get; set; }
        [Display(Name = "Data spedizione")]
        [Required]

        public DateTime DataSpedizione { get; set; }
        [Required]

        public decimal Peso { get; set; }
        [Required]

        public string Citta { get; set; }
        [Display(Name ="Indirizzo destinatario")]
        [Required]

        public string IndirizzoDestinatario { get; set; }
        [Display(Name = "Nome destinatario")]
        [Required]

        public string NomeDestinatario { get; set; }
        [Display(Name = "Costo spedizione")]
        [Required]

        public decimal CostoSpedizione { get; set; }

        [Display(Name = "Data consegna")]
        [Required]

        public DateTime DataConsegna { get; set; }
        
        public int IdCliente { get; set; }
    }
}