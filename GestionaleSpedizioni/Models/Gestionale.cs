using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionaleSpedizioni.Models
{
    
    public class Gestionale
    {
        public static List<LoginModel> ListaLogin { get; set; } = new List<LoginModel>(); 
        //{
        //    new LoginModel() { IdLogin=1, Username= "mario", Password ="123"}
        //};
        public static List<Cliente> ListaClienti { get; set; } = new List<Cliente>();

        public static List<Spedizione> ListaTotSpedizioni { get; set; } = new List<Spedizione>();
    }
}