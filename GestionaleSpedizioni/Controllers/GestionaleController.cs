using GestionaleSpedizioni.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace GestionaleSpedizioni.Controllers
{
    [Authorize]
    public class GestionaleController : Controller
    {

        public static SqlConnection ConnessioneDB()
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["ConnessioneDB"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            return sqlConnection;
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Dashboard");

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LoginModel l)
        {
            try
            {
                SqlConnection sqlConnection = ConnessioneDB();
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Login", sqlConnection);

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                if(sqlDataReader.HasRows)
                {
                    while(sqlDataReader.Read())
                    {
                        if(l.Username== sqlDataReader["Username"].ToString() && l.Password == sqlDataReader["Password"].ToString())
                        {
                            FormsAuthentication.SetAuthCookie(l.Username, false);

                            HttpCookie cookiePropID = new HttpCookie("USER_COOKIE");
                            cookiePropID.Values["ID"] = sqlDataReader["IdLogin"].ToString();
                            Response.Cookies.Add(cookiePropID);

                            TempData["Successo"] = $"Benvenuto {l.Username}";

                            sqlConnection.Close();
                            return Redirect(FormsAuthentication.DefaultUrl);

                        }
                        else
                        {
                            ViewBag.Errore = "Username o password errate";
                        }
                    }
                }
                
                sqlConnection.Close();

            } catch
            {
                return View();
            }
            return View();
        }

        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return Redirect(FormsAuthentication.LoginUrl);

        }

        public ActionResult Dashboard()
        {
            List<Cliente> clienti = new List<Cliente>();
            try
            {
                SqlConnection sqlConnection = ConnessioneDB();
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Cliente", sqlConnection);

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                if(sqlDataReader.HasRows)
                {
                    while(sqlDataReader.Read())
                    {
                        Cliente c = new Cliente();
                        c.IdCliente = Convert.ToInt32(sqlDataReader["IdCliente"]);
                        c.Nome = sqlDataReader["Nome"].ToString();
                        c.Cognome = sqlDataReader["Cognome"].ToString();
                        c.Indirizzo = sqlDataReader["Indirizzo"].ToString();
                        c.CodiceFiscale = sqlDataReader["CodiceFiscale"].ToString();
                        c.PartitaIva = sqlDataReader["PartitaIva"].ToString();
                        c.Privato = Convert.ToBoolean(sqlDataReader["Privato"]);
                        clienti.Add(c);
                    }
                }

                sqlConnection.Close();
                return View(clienti);

            }
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
        }

        public ActionResult SpedizioniDashboard()
        {
            List<Spedizione> spedizioni = new List<Spedizione>();
            try
            {
                SqlConnection sqlConnection = ConnessioneDB();
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Spedizione", sqlConnection);

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        Spedizione sp = new Spedizione();
                        sp.IdSpedizione = Convert.ToInt32(sqlDataReader["IdSpedizione"]);
                        sp.DataSpedizione = Convert.ToDateTime(sqlDataReader["DataSpedizione"]);
                        sp.Peso = Convert.ToDecimal(sqlDataReader["Peso"]);
                        sp.Citta = sqlDataReader["Citta"].ToString();
                        sp.IndirizzoDestinatario = sqlDataReader["IndirizzoDestinatario"].ToString();
                        sp.NomeDestinatario = sqlDataReader["NomeDestinatario"].ToString();
                        sp.CostoSpedizione = Convert.ToDecimal(sqlDataReader["CostoSpedizione"]);
                        sp.DataConsegna = Convert.ToDateTime(sqlDataReader["DataConsegna"]);
                        sp.IdCliente = Convert.ToInt32(sqlDataReader["IdCliente"]);

                        spedizioni.Add(sp);
                    }
                }

                sqlConnection.Close();
                sqlConnection.Open();

                sqlCommand = new SqlCommand("SELECT * FROM AGGIORNAMENTO", sqlConnection);

                sqlDataReader = sqlCommand.ExecuteReader();

                List<Aggiornamento> aggiornamenti = new List<Aggiornamento>();
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        Aggiornamento agg = new Aggiornamento();
                        agg.IdAggiornamento = Convert.ToInt32(sqlDataReader["IdAggiornamento"]);
                        agg.Stato = sqlDataReader["Stato"].ToString();
                        agg.Luogo = sqlDataReader["Luogo"].ToString();
                        agg.Descrizione = sqlDataReader["Descrizione"].ToString();
                        agg.DataAggiornamento = Convert.ToDateTime(sqlDataReader["DataAggiornamento"]);
                        agg.IdSpedizione = Convert.ToInt32(sqlDataReader["IdSpedizione"]);
                        agg.IdLogin = Convert.ToInt32(sqlDataReader["IdLogin"]);

                        aggiornamenti.Add(agg);
                    }
                }
                sqlConnection.Close();

                sqlConnection.Open();

                ViewBag.ListaAggiornamenti = aggiornamenti;

                sqlCommand = new SqlCommand("SELECT COUNT(*) AS NumeroTotSpedizioni, Citta FROM Spedizione GROUP BY Citta", sqlConnection);

                sqlDataReader = sqlCommand.ExecuteReader();
                List<Spedizione> ListaCitta = new List<Spedizione>();
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        Spedizione sp = new Spedizione();
                        sp.IdSpedizione = Convert.ToInt32(sqlDataReader["NumeroTotSpedizioni"]);
                        sp.Citta = sqlDataReader["Citta"].ToString();

                        ListaCitta.Add(sp);
                    }
                }
                ViewBag.ListaCitta = ListaCitta;
                return View(spedizioni);

            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
        }

        [AllowAnonymous]
        public ActionResult RicercaSpedizioni()
        {
            return View();
        }
        [AllowAnonymous]

        [HttpPost]
        public ActionResult RicercaSpedizioni(string CF, int NumeroSpedizione)
        {
            List<Cliente> clienti = new List<Cliente>();
            try
            {
                SqlConnection sqlConnection = ConnessioneDB();
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Cliente WHERE PartitaIva=@cf OR CodiceFiscale=@cf", sqlConnection);
                sqlCommand.Parameters.AddWithValue("cf", CF);

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                int idCliente = 0;
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        idCliente = Convert.ToInt32(sqlDataReader["IdCliente"]);
                    }
                }

                sqlConnection.Close();

                sqlConnection.Open();


                SqlCommand sqlCommand2 = new SqlCommand("SELECT * FROM Spedizione WHERE IdCliente = @idcliente AND IdSpedizione = @idspedizione", sqlConnection);
                sqlCommand2.Parameters.AddWithValue("idcliente", idCliente);
                sqlCommand2.Parameters.AddWithValue("idspedizione", NumeroSpedizione);

                SqlDataReader sqlDataReader2 = sqlCommand2.ExecuteReader();

                int idSpedizione = 0;
                if (sqlDataReader2.HasRows)
                {
                    while (sqlDataReader2.Read())
                    {
                        idSpedizione = Convert.ToInt32(sqlDataReader2["IdSpedizione"]);
                    }
                }
                else
                {
                    ViewBag.Errore = "Spedizione non trovata";
                    return View();
                }

                sqlConnection.Close();

                sqlConnection.Open();


                SqlCommand sqlCommand3 = new SqlCommand("SELECT * FROM Aggiornamento WHERE IdSpedizione=@idspedizione", sqlConnection);
                sqlCommand3.Parameters.AddWithValue("idspedizione", idSpedizione);

                SqlDataReader sqlDataReader3 = sqlCommand3.ExecuteReader();

                List<Aggiornamento> aggiornamenti = new List<Aggiornamento>();
                if(sqlDataReader3.HasRows)
                {
                    while(sqlDataReader3.Read()) {
                        
                            Aggiornamento agg = new Aggiornamento();
                            agg.IdAggiornamento = Convert.ToInt32(sqlDataReader3["IdAggiornamento"]);
                            agg.Stato = sqlDataReader3["Stato"].ToString();
                            agg.Luogo = sqlDataReader3["Luogo"].ToString();
                            agg.Descrizione = sqlDataReader3["Descrizione"].ToString();
                            agg.DataAggiornamento = Convert.ToDateTime(sqlDataReader3["DataAggiornamento"]);
                            agg.IdSpedizione = Convert.ToInt32(sqlDataReader3["IdSpedizione"]);
                            agg.IdLogin = Convert.ToInt32(sqlDataReader3["IdLogin"]);

                            aggiornamenti.Add(agg);
                        
                    }
                }
                else
                {
                    ViewBag.Errore = "Nessun aggiornamento per la spedizione";
                    return View();
                }

                TempData["ListaAggiornamenti"] = aggiornamenti;
                sqlConnection.Close();
                return RedirectToAction("AggiornamentiSpedizione");

            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
        }

        [AllowAnonymous]
        public ActionResult AggiornamentiSpedizione()
        {
            List<Aggiornamento> list = new List<Aggiornamento>();
            List<Aggiornamento> list2 = new List<Aggiornamento>();

            list = (List<Aggiornamento>)TempData["ListaAggiornamenti"];



            if (list != null)
            {
                for (int i = list.Count() - 1; i >= 0; i--)
                {
                    list2.Add(list[i]);
                }
                return View(list2);
            }
            return RedirectToAction("RicercaSpedizioni");

            //if (TempData["ListaAggiornamenti"] != null)
            //{
                
            //    return View(TempData["ListaAggiornamenti"]);
            //}
            //return RedirectToAction("RicercaSpedizioni");
            
        }

        public ActionResult CreateCliente()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateCliente(Cliente c)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    SqlConnection sqlConnection = ConnessioneDB();
                    sqlConnection.Open();

                    SqlCommand sqlCommand = new SqlCommand("INSERT INTO CLIENTE VALUES(@nome, @cognome, @indirizzo, @codicefiscale, @partitaiva, @privato)", sqlConnection);
                    
                    sqlCommand.Parameters.AddWithValue("nome", c.Nome);
                    sqlCommand.Parameters.AddWithValue("cognome", c.Cognome);
                    sqlCommand.Parameters.AddWithValue("indirizzo", c.Indirizzo);

                    if(c.CodiceFiscale != null && c.PartitaIva==null)
                    {
                        sqlCommand.Parameters.AddWithValue("codicefiscale", c.CodiceFiscale);
                        sqlCommand.Parameters.AddWithValue("partitaiva", "");
                    }
                    else if(c.CodiceFiscale == null && c.PartitaIva != null)
                    {
                        sqlCommand.Parameters.AddWithValue("codicefiscale", "");
                        sqlCommand.Parameters.AddWithValue("partitaiva", c.PartitaIva);

                    }
                    sqlCommand.Parameters.AddWithValue("privato", c.Privato);

                    int righeInserite = sqlCommand.ExecuteNonQuery();

                    if (righeInserite > 0)
                    {
                        TempData["Successo"] = "Cliente aggiunto correttamente";
                    }

                    sqlConnection.Close();
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    ViewBag.Errore = "Non hai inserito i campi correttamente";
                    return View();
                }
               

            }
            catch(Exception ex)
            {
                ViewBag.Errore = ex.Message;
                return View();
            }
        }

        public ActionResult EditCliente(int id)
        {
            try
            {
                SqlConnection sqlConnection = ConnessioneDB();
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Cliente WHERE IdCliente=@idcliente", sqlConnection);
                sqlCommand.Parameters.AddWithValue("idcliente", id);

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                Cliente c = new Cliente();

                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        c.IdCliente = Convert.ToInt32(sqlDataReader["IdCliente"]);
                        c.Nome = sqlDataReader["Nome"].ToString();
                        c.Cognome = sqlDataReader["Cognome"].ToString();
                        c.Indirizzo = sqlDataReader["Indirizzo"].ToString();
                        c.CodiceFiscale = sqlDataReader["CodiceFiscale"].ToString();
                        c.PartitaIva = sqlDataReader["PartitaIva"].ToString();
                        c.Privato = Convert.ToBoolean(sqlDataReader["Privato"]);
                    }
                }

                sqlConnection.Close();
                return View(c);

            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public ActionResult EditCliente(Cliente c)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    SqlConnection sqlConnection = ConnessioneDB();
                    sqlConnection.Open();

                    SqlCommand sqlCommand = new SqlCommand("UPDATE CLIENTE SET Nome=@nome, Cognome=@cognome, Indirizzo=@indirizzo, CodiceFiscale=@codicefiscale, PartitaIva=@partitaiva, Privato=@privato WHERE IdCliente=@idcliente", sqlConnection);
                    sqlCommand.Parameters.AddWithValue("idcliente", c.IdCliente);

                    sqlCommand.Parameters.AddWithValue("nome", c.Nome);
                    sqlCommand.Parameters.AddWithValue("cognome", c.Cognome);
                    sqlCommand.Parameters.AddWithValue("indirizzo", c.Indirizzo);

                    if (c.CodiceFiscale != null && c.PartitaIva == null)
                    {
                        sqlCommand.Parameters.AddWithValue("codicefiscale", c.CodiceFiscale);
                        sqlCommand.Parameters.AddWithValue("partitaiva", "");
                    }
                    else if (c.CodiceFiscale == null && c.PartitaIva != null)
                    {
                        sqlCommand.Parameters.AddWithValue("codicefiscale", "");
                        sqlCommand.Parameters.AddWithValue("partitaiva", c.PartitaIva);

                    }
                    sqlCommand.Parameters.AddWithValue("privato", c.Privato);

                    int righeInserite = sqlCommand.ExecuteNonQuery();

                    if (righeInserite > 0)
                    {
                        TempData["ModificaCorretta"] = "Cliente modificato correttamente";
                    }

                    sqlConnection.Close();
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    return View();
                }


            }
            catch (Exception ex)
            {
                ViewBag.Errore = ex.Message;
                return View();
            }
        }
            
        public ActionResult DetailsCliente(int id)
        {
            try
            {
                SqlConnection sqlConnection = ConnessioneDB();
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Cliente WHERE IdCliente=@idcliente", sqlConnection);
                sqlCommand.Parameters.AddWithValue("idcliente", id);

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                Cliente c = new Cliente();

                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        c.IdCliente = Convert.ToInt32(sqlDataReader["IdCliente"]);
                        c.Nome = sqlDataReader["Nome"].ToString();
                        c.Cognome = sqlDataReader["Cognome"].ToString();
                        c.Indirizzo = sqlDataReader["Indirizzo"].ToString();
                        c.CodiceFiscale = sqlDataReader["CodiceFiscale"].ToString();
                        c.PartitaIva = sqlDataReader["PartitaIva"].ToString();
                        c.Privato = Convert.ToBoolean(sqlDataReader["Privato"]);
                    }
                }
                sqlConnection.Close();
                sqlConnection.Open();
                sqlCommand = new SqlCommand("SELECT * FROM SPEDIZIONE WHERE IdCliente=@idcliente", sqlConnection);
                sqlCommand.Parameters.AddWithValue("idcliente", id);

                sqlDataReader = sqlCommand.ExecuteReader();

                List<Spedizione> spedizioni = new List<Spedizione>();

                if(sqlDataReader.HasRows)
                {
                    while(sqlDataReader.Read())
                    {
                        Spedizione sp = new Spedizione();
                        sp.IdSpedizione = Convert.ToInt32(sqlDataReader["IdSpedizione"]);
                        sp.DataSpedizione = Convert.ToDateTime(sqlDataReader["DataSpedizione"]);
                        sp.Peso = Convert.ToDecimal(sqlDataReader["Peso"]);
                        sp.Citta = sqlDataReader["Citta"].ToString();
                        sp.IndirizzoDestinatario = sqlDataReader["IndirizzoDestinatario"].ToString();
                        sp.NomeDestinatario = sqlDataReader["NomeDestinatario"].ToString();
                        sp.CostoSpedizione= Convert.ToDecimal(sqlDataReader["CostoSpedizione"]);
                        sp.DataConsegna = Convert.ToDateTime(sqlDataReader["DataConsegna"]);
                        sp.IdCliente = Convert.ToInt32(sqlDataReader["IdCliente"]);

                        spedizioni.Add(sp);


                    }
                }
                ViewBag.ListaSpedizioni = spedizioni;
                sqlConnection.Close();
                return View(c);

            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
        }

        public ActionResult DeleteCliente(int id)
        {

            try
            {
                SqlConnection sqlConnection = ConnessioneDB();
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Cliente WHERE IdCliente=@idcliente", sqlConnection);
                sqlCommand.Parameters.AddWithValue("idcliente", id);

                Cliente c = new Cliente();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        c.IdCliente = Convert.ToInt32(sqlDataReader["IdCliente"]);
                        c.Nome = sqlDataReader["Nome"].ToString();
                        c.Cognome = sqlDataReader["Cognome"].ToString();
                    }
                }

                sqlConnection.Close();
                return View(c);

            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
        }
        [HttpPost]
        [ActionName("DeleteCliente")]
        public ActionResult ConfermDeleteCliente(int id, Cliente c)
        {
            try
            {
                    SqlConnection sqlConnection = ConnessioneDB();
                    sqlConnection.Open();

                    SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Spedizione WHERE IdCliente=@idcliente", sqlConnection);
                    sqlCommand.Parameters.AddWithValue("idcliente", id);

                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                    if(sqlDataReader.HasRows)
                    {
                        ViewBag.Errore = "Impossibile eliminare il cliente perchè ha effettuato delle spedizioni";
                        sqlConnection.Close();
                        return View(c);
                    }

                    sqlConnection.Close();

                    sqlConnection.Open();

                    sqlCommand = new SqlCommand("DELETE CLIENTE WHERE IdCliente=@idcliente", sqlConnection);
                    sqlCommand.Parameters.AddWithValue("idcliente", id);

                    int righeInserite = sqlCommand.ExecuteNonQuery();

                    if (righeInserite > 0)
                    {
                        TempData["Successo"] = "Cliente cancellato correttamente";
                    }

                    
                    sqlConnection.Close();
                    return RedirectToAction("Dashboard");


            }
            catch (Exception ex)
            {
                ViewBag.Errore = ex.Message;
                return View();
            }
        }

        //DATI PER LA SPEDIZIONE
        public ActionResult CreateSpedizione()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateSpedizione(int IdCliente, Spedizione sp)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SqlConnection sqlConnection = ConnessioneDB();
                    sqlConnection.Open();

                    SqlCommand sqlCommand = new SqlCommand("INSERT INTO SPEDIZIONE VALUES(@dataspedizione, @peso, @citta, @indirizzodestinatario, @nomedestinatario, @costospedizione, @dataconsegna, @idcliente)", sqlConnection);
                    sqlCommand.Parameters.AddWithValue("idcliente", IdCliente);

                    sqlCommand.Parameters.AddWithValue("dataspedizione", sp.DataSpedizione);
                    sqlCommand.Parameters.AddWithValue("peso", sp.Peso);
                    sqlCommand.Parameters.AddWithValue("citta", sp.Citta);
                    sqlCommand.Parameters.AddWithValue("indirizzodestinatario", sp.IndirizzoDestinatario);
                    sqlCommand.Parameters.AddWithValue("nomedestinatario", sp.NomeDestinatario);
                    sqlCommand.Parameters.AddWithValue("costospedizione", sp.CostoSpedizione);
                    sqlCommand.Parameters.AddWithValue("dataconsegna", sp.DataConsegna);


                    int righeInserite = sqlCommand.ExecuteNonQuery();

                    if (righeInserite > 0)
                    {
                        TempData["InserimentoCorretto"] = "Spedizione aggiunta correttamente";
                    }

                    sqlConnection.Close();
                    TempData["Successo"] = "Spedizione aggiunta correttamente";
                    return RedirectToAction("DetailsCliente", new { id = IdCliente });
                }
                else
                {
                    ViewBag.Errore = "Errore nella compilazione dei campi";
                    return View();
                }


            }
            catch (Exception ex)
            {
                ViewBag.Errore = ex.Message;
                return View();
            }
        }

        //AGGIORNAMENTI
        [HttpGet]
        public ActionResult MostraAggiornamenti(int IdCliente, int IdSpedizione)
        {
            List<Aggiornamento> aggiornamenti = new List<Aggiornamento>();
            try
            {
                SqlConnection sqlConnection = ConnessioneDB();
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand("SELECT * FROM AGGIORNAMENTO WHERE IdSpedizione=@idspedizione", sqlConnection);
                sqlCommand.Parameters.AddWithValue("idspedizione", IdSpedizione);

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        Aggiornamento agg = new Aggiornamento();
                        agg.IdAggiornamento = Convert.ToInt32(sqlDataReader["IdAggiornamento"]);
                        agg.Stato = sqlDataReader["Stato"].ToString();
                        agg.Luogo = sqlDataReader["Luogo"].ToString();
                        agg.Descrizione = sqlDataReader["Descrizione"].ToString();
                        agg.DataAggiornamento = Convert.ToDateTime(sqlDataReader["DataAggiornamento"]);
                        agg.IdSpedizione = Convert.ToInt32(sqlDataReader["IdSpedizione"]);
                        agg.IdLogin = Convert.ToInt32(sqlDataReader["IdLogin"]);

                        aggiornamenti.Add(agg);
                    }
                }

                sqlConnection.Close();
                return View(aggiornamenti);

            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
        }

        [HttpGet]
        public ActionResult CreateAggiornamento()
        {
            //dropdown
            List<SelectListItem> lista = new List<SelectListItem>();
            SelectListItem item1 = new SelectListItem
            {
                Text = "In consegna",
                Value = "In consegna",
                Selected = true
            };
            SelectListItem item2 = new SelectListItem
            {
                Text = "In transito",
                Value = "In transito"
            }; SelectListItem item3 = new SelectListItem
            {
                Text = "Consegnato",
                Value = "Consegnato"
            };
            SelectListItem item4 = new SelectListItem
            {
                Text = "Non consegnato",
                Value = "Non consegnato"
            };
            lista.Add(item1);
            lista.Add(item2);
            lista.Add(item3);
            lista.Add(item4);

            ViewBag.Stato = lista;     
    
            return View();
        }

        [HttpPost]
        public ActionResult CreateAggiornamento(int IdCliente, int IdSpedizione, Aggiornamento agg)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SqlConnection sqlConnection = ConnessioneDB();
                    sqlConnection.Open();

                    SqlCommand sqlCommand = new SqlCommand("INSERT INTO AGGIORNAMENTO VALUES(@stato, @luogo, @descrizione, @dataaggiornamento, @idlogin, @idspedizione)", sqlConnection);
                    sqlCommand.Parameters.AddWithValue("idspedizione", IdSpedizione);

                    sqlCommand.Parameters.AddWithValue("stato", agg.Stato);
                    sqlCommand.Parameters.AddWithValue("luogo", agg.Luogo);
                    sqlCommand.Parameters.AddWithValue("descrizione", agg.Descrizione);
                    sqlCommand.Parameters.AddWithValue("dataaggiornamento", DateTime.Now);
                    sqlCommand.Parameters.AddWithValue("idlogin", Request.Cookies["USER_COOKIE"]["ID"]);





                    int righeInserite = sqlCommand.ExecuteNonQuery();

                    if (righeInserite > 0)
                    {
                        TempData["InserimentoCorretto"] = "Spedizione aggiunta correttamente";
                    }

                    sqlConnection.Close();
                    TempData["Successo"] = "Aggiornamento aggiunto correttamente";
                    return RedirectToAction("MostraAggiornamenti", new { idCliente = IdCliente, idSpedizione = IdSpedizione });
                }
                else
                {
                    ViewBag.Errore = "Errore nell'inserimento dei campi";
                    return View();
                }


            }
            catch (Exception ex)
            {
                ViewBag.Errore = ex.Message;
                return View();
            }

        }
    }
}