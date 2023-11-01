using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace GestionaleSpedizioni.Models
{
    public class Cliente
    {
        [Display(Name = "Id cliente")]

        public int IdCliente { get; set; }
        [Required]

        public string Nome { get; set; }
        [Required]

        public string Cognome { get; set; }
        [Required]

        public string Indirizzo { get; set; }

        [Display(Name = "Codice fiscale")]

        public string CodiceFiscale { get; set; }

        [Display(Name = "Partita iva")]

        public string PartitaIva { get; set; }
        public bool Privato { get; set; }
        public List<Spedizione> ListaSpedizioni { get; set; } = new List<Spedizione>();

    }
}