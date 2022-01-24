using myauth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Net;
using Microsoft.AspNet.Identity;
using System.Net.Mail;

namespace ProjectWOrk.Controllers
{
    [HandleError]
    public class AdminController : Controller
    {
        
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin
        [Authorize]
        public ActionResult Accueil()
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index2", "default");
            }
            var clientsCount = db.Clients.ToList().Count;
            var propCount = db.Proprietaires.ToList().Count;
            var users = db.Users.ToList().Count;
            ViewBag.clientsCount = clientsCount.ToString();
            ViewBag.propCount = propCount.ToString();
            ViewBag.allCount = users.ToString();

            int notifCount = 0;
            List<string[]> notifs = new List<string[]>();
            
            int procount = db.Produits.Where(x => x.IsVerified.Equals(false)).ToList().Count;
            if (procount != 0)
            {
                notifCount++;
                string[] obj = new string[] { "New " + procount + " products are waiting to be verified", "/Admin/OwnerProductVerication" };
                notifs.Add(obj);
            }
            if (db.ClientActivites.Where(e => (DateTime.Compare(e.Date, db.Admins.FirstOrDefault().Last_connection)) >= 0).ToList().Count != 0)
            {
                notifCount++;
                string[] obj = new string[] { "New Activities has been done by some Clients", "/Admin/ClientHistory" };
                notifs.Add(obj);
            }
            if (db.PropActivites.Where(e => (DateTime.Compare(e.Date, db.Admins.FirstOrDefault().Last_connection)) >= 0).ToList().Count != 0)
            {
                notifCount++;
                string[] obj = new string[] { "New Activities has been done by some Owners", "/Admin/OwnerHistory" };
                notifs.Add(obj);
            }
            if (db.Produits.Where(e => (DateTime.Compare(e.DateAddition, db.Admins.FirstOrDefault().Last_connection)) >= 0).ToList().Count != 0)
            {
                notifCount++;
                string[] obj = new string[] { "New Commands/parches has been launched", "/Admin/ViewProducts" };
                notifs.Add(obj);
            }

            Session["NotificationCount"] = notifCount.ToString();
            Session["Notification"] = notifs;

            ViewBag.prodsCount = db.Produits.Count();

            var admin = db.Admins.FirstOrDefault();
            admin.Last_connection = DateTime.Now;
            db.Entry(admin).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            var favoritProps = db.Proprietaires.Where(e => e.IsFavorite == true).Include(x => x.Produits).Include(x => x.User).ToList();
            return View(favoritProps);
        }
        [Authorize]
        public ActionResult Owner()
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index2", "default");
            }
            return View();
        }
        [Authorize]
        public ActionResult OwnerProfils()
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index2", "default");
            }
            var owners = db.Proprietaires.Include(e => e.User).ToList();
            return View(owners);
        }
        

        [HttpPost]
        [Authorize]
        public ActionResult OwnerProfils(string Search)
        {

            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index2", "default");
            }
            if (Request["propIdFav"] != null)
            {
                var propId = int.Parse(Request["propIdFav"]);
                var prop = db.Proprietaires.Find(propId);
                if (!prop.IsFavorite)
                {
                    prop.IsFavorite = true;
                    db.Entry(prop).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.hasBeenAdded = "An Owner has been added to Favorite";
                }
            }
            if (Request["propIdDefav"] != null)
            {
                var propId = int.Parse(Request["propIdDefav"]);
                var prop = db.Proprietaires.Find(propId);
                if (prop.IsFavorite)
                {
                    prop.IsFavorite = false;
                    db.Entry(prop).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.hasBeenAdded = "An Owner has been removed from Favorite";
                }
            }
            if (Request["propIdBlack"] != null)
            {
                var propId = int.Parse(Request["propIdBlack"]);
                var prop = db.Proprietaires.Find(propId);
                if (!prop.IsNoir)
                {
                    prop.IsNoir = true;
                    db.Entry(prop).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.hasBeenAdded = "An Owner has been added to BlackList";
                }
            }
            if (Request["propIdDeblack"] != null)
            {
                var propId = int.Parse(Request["propIdDeblack"]);
                var prop = db.Proprietaires.Find(propId);
                if (!prop.IsFavorite)
                {
                    prop.IsNoir = false;
                    db.Entry(prop).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.hasBeenAdded = "An Owner has been removed from Favorite";
                }
            }

            int a;

            if (Request["button"] != null)
            {
                if (Request["button"].ToString() == "Black List" && (Search.Equals("true") || Search.Equals("false")))
                {
                    bool flt;
                    if (Search.Equals("true"))
                        flt = true;
                    else
                        flt = false;
                    var props = db.Proprietaires.Where(x => x.IsNoir.Equals(flt)).Include(e => e.User).ToList();
                    return View(props);
                }
                else if (Request["button"].ToString() == "Name Prop" && !String.IsNullOrEmpty(Search))
                {
                    var activities = db.Proprietaires.Where(x => x.User.UserName.Contains(Search)).Include(e => e.User).ToList();
                    return View(activities);
                }
                else if (Request["button"].ToString() == "Id Prop" && !String.IsNullOrEmpty(Search) && Int32.TryParse(Search, out a))
                {
                    int b = Int32.Parse(Search);
                    var props = db.Proprietaires.Where(x => x.IdPropritaire.Equals(b)).Include(e => e.User).ToList();
                    return View(props);
                }
                else if (Request["button"].ToString() == "Favorit List" && (Search.Equals("true") || Search.Equals("false")))
                {
                    bool flt;
                    if (Search.Equals("true"))
                        flt = true;
                    else
                        flt = false;
                    var props = db.Proprietaires.Where(x => x.IsFavorite.Equals(flt)).Include(e => e.User).ToList();
                    return View(props);
                }
                else
                {
                    return View(db.Proprietaires.Include(e => e.User).ToList());
                }
            }
            return View(db.Proprietaires.Include(e => e.User).ToList());

        }
        [Authorize]
        public ActionResult OwnerHistory()
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index2", "default");
            }
            var activities = db.PropActivites.Include(e=>e.Proprietaire.User).Include(e => e.Proprietaire).Include(e => e.Produit).ToList();
            

           

            return View(activities);
        }
        [HttpPost]
        [Authorize]
        public ActionResult OwnerHistory(string Search)
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index2", "default");
            }
            int a;

            if (Request["button"].ToString() == "Prop" && !String.IsNullOrEmpty(Search))
            {
                //line changed
                var activities = db.PropActivites.Where(x => x.Proprietaire.User.UserName.Contains(Search)).Include(e => e.Proprietaire.User).Include(e => e.Proprietaire).Include(e => e.Produit).ToList();
                return View(activities);
            }
            else if(Request["button"].ToString() == "Product" && !String.IsNullOrEmpty(Search))
            {
                var activities = db.PropActivites.Where(x => x.Produit.Description.Contains(Search)).Include(e => e.Proprietaire.User).Include(e => e.Proprietaire).Include(e => e.Produit).ToList();
                return View(activities);
            }
            else if (Request["button"].ToString() == "PropId" && !String.IsNullOrEmpty(Search) && Int32.TryParse(Search, out a))
            {
                int b = Int32.Parse(Search);
                var activities = db.PropActivites.Where(x => x.Proprietaire.IdPropritaire.Equals(b)).Include(e => e.Proprietaire.User).Include(e => e.Proprietaire).Include(e => e.Produit).ToList();
                return View(activities);
            }
            else if (Request["button"].ToString() == "ProductId" && !String.IsNullOrEmpty(Search) && Int32.TryParse(Search, out a))
            {
                int b = Int32.Parse(Search);
                var activities = db.PropActivites.Where(x => x.Produit.IdProduit.Equals(b)).Include(e => e.Proprietaire.User).Include(e => e.Proprietaire).Include(e => e.Produit).ToList();
                return View(activities);
            }
            else
            {
                return View(db.PropActivites.Include(e => e.Proprietaire.User).Include(e => e.Proprietaire).Include(e => e.Produit).ToList());
            }

        }
        [Authorize]
        public ActionResult OwnerProductVerication()
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index2", "default");
            }
            var produits = db.Produits.Where(x => x.IsVerified.Equals(false)).Include(e=>e.Proprietaire).ToList();

            if(Session["ProductVerified"]!= null)
            {
                ViewBag.showSuccess = Session["ProductVerified"];
                Session["ProductVerified"] = null;
            }

            return View(produits);
        }
        [HttpPost]
        [Authorize]
        public ActionResult OwnerProductVerication(string Search)
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index2", "default");
            }
            int a;
            Decimal b;

            if (Request["button"] != null)
            {
                if (Request["button"].ToString() == "Prix" && !String.IsNullOrEmpty(Search) && Decimal.TryParse(Search, out b))
                {
                    Decimal prix = Decimal.Parse(Search);

                    var produits = db.Produits.Where(x => x.IsVerified == false).Where(x => x.Prix == prix).Include(e => e.Proprietaire).ToList();
                    return View(produits);
                }
                else if (Request["button"].ToString() == "Description" && !String.IsNullOrEmpty(Search))
                {

                    var produits = db.Produits.Where(x => x.IsVerified == false).Where(x => x.Description.Contains(Search)).Include(e => e.Proprietaire).ToList();
                    return View(produits);
                }
                else if (Request["button"].ToString() == "Id Prop" && !String.IsNullOrEmpty(Search) && Int32.TryParse(Search, out a))
                {
                    int id = Int32.Parse(Search);
                    //line changed
                    var produits = db.Produits.Where(x => x.IsVerified == false).Where(t => t.Proprietaire.IdPropritaire == id).Include(e => e.Proprietaire).ToList();

                    return View(produits);
                }
                else if (Request["button"].ToString() == "Product Id" && !String.IsNullOrEmpty(Search) && Int32.TryParse(Search, out a))
                {
                    int id = Int32.Parse(Search);
                    var produits = db.Produits.Where(x => x.IsVerified == false).Where(x => x.IdProduit == id).Include(e => e.Proprietaire).ToList();
                    return View(produits);
                }
                else
                {
                    return View(db.Produits.Where(x => x.IsVerified == false).Include(e => e.Proprietaire).ToList());
                }
            }
            return View(db.Produits.Where(x => x.IsVerified == false).Include(e => e.Proprietaire).ToList());


        }
        [Authorize]
        public ActionResult VerificationSuccess(int id)
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index2", "default");
            }
            var produit = db.Produits.Include(e => e.Proprietaire.User).FirstOrDefault(e=>e.IdProduit == id);
            ViewBag.c = db.ImageProduits.Where(x => x.Produit.IdProduit == id).ToList().Count;
            //ViewBag.c = 4;
            return View(produit);
        }
        [HttpPost, ActionName("VerificationSuccess")]
        [Authorize]
        public ActionResult VerificationSuccessPost(int id)

        {
            var produit = db.Produits.Find(id);
            produit.IsVerified = true;
            db.Entry(produit).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            Session["ProductVerified"] = " One Product has been Verified";

            return RedirectToAction("OwnerProductVerication");
        }
        [Authorize]
        public ActionResult RenderImage(int id, int t)
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index2", "default");
            }
            //line changed
            var images = db.ImageProduits.Where(x => x.Produit.IdProduit == id).ToList();
            if (!(images.Count() == 0))
            {
                byte[] ph = images[t-1].Image;
                
                return File(ph, "image/png");

            }
            return null;
        }

        [Authorize]
        public ActionResult VerificationDelete(int id)
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index2", "default");
            }
            var produit = db.Produits.Include(e => e.Proprietaire).Include(e=>e.Proprietaire.User).FirstOrDefault(e => e.IdProduit == id);
            ViewBag.c = db.ImageProduits.Where(x => x.Produit.IdProduit == id).ToList().Count;

            return View(produit);
        }
        [HttpPost, ActionName("VerificationDelete")]
        [Authorize]
        public ActionResult VerificationDeletePost(int id)
        {
            var produit = db.Produits.Find(id);
            foreach (var obj in db.Commandes.Where(e => e.Produit.IdProduit == id).ToList())
                db.Commandes.Remove(obj);
            foreach (var obj in db.Commentaires.Where(e => e.Produit.IdProduit == id).ToList())
                db.Commentaires.Remove(obj);
            foreach (var obj in db.ImageProduits.Where(e => e.Produit.IdProduit == id).ToList())
                db.ImageProduits.Remove(obj);

            db.Produits.Remove(produit);
            db.SaveChanges();

            Session["ProductVerified"] = " One Product has been Deleted";

            return RedirectToAction("OwnerProductVerication");
        }
        [Authorize]
        public ActionResult ClientHistory()
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index2", "default");
            }
            var activities = db.ClientActivites.Include(e=>e.Client.User).ToList();

            return View(activities);
        }
        [HttpPost]
        [Authorize]
        public ActionResult ClientHistory(string Search)
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index2", "default");
            }
            int a;

            if (Request["button"].ToString() == "Action" && !String.IsNullOrEmpty(Search))
            {
                var activities = db.ClientActivites.Where(x => x.Name.Contains(Search)).ToList();
                return View(activities);
            }
            else if (Request["button"].ToString() == "Id Client" && !String.IsNullOrEmpty(Search) && Int32.TryParse(Search, out a))
            {
                int id = Int32.Parse(Search);
                var activities = db.ClientActivites.Where(x => x.Client.IdClient.Equals(id)).ToList();
                return View(activities);
            }
            else if (Request["button"].ToString() == "Id Commande" && !String.IsNullOrEmpty(Search) && Int32.TryParse(Search, out a))
            {
                int id = Int32.Parse(Search);
                var activities = db.ClientActivites.Where(x => x.Commande.IdCommande.Equals(id)).ToList();
                return View(activities);
            }
            else
            {
                return View(db.ClientActivites.ToList());
            }

        }
        [Authorize]
        public ActionResult Client()
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index2", "default");
            }
            var clients = db.Clients.Include(e=>e.User).ToList();

            return View(clients);
        }
        [HttpPost]
        [Authorize]
        public ActionResult Client(string Search)
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index2", "default");
            }
            int a;

            if (Request["button"].ToString() == "Id Client" && !String.IsNullOrEmpty(Search) && Int32.TryParse(Search, out a))
            {
                int id = Int32.Parse(Search);
                var clients = db.Clients.Where(x => x.IdClient == id).ToList();
                return View(clients);
            }

            else if (Request["button"].ToString() == "Nom" && !String.IsNullOrEmpty(Search))
            {
                //line changed
                var clients = db.Clients.Where(x => x.User.UserName.Contains(Search)).ToList();
                return View(clients);
            }

            //else if (Request["button"].ToString() == "Prenom" && !String.IsNullOrEmpty(Search))
            //{
            //    //line changed
            //    var clients = db.Clients.Where(x => x.Prenom.Contains(Search)).ToList();
            //    return View(clients);
            //}
            else if (Request["button"].ToString() == "Email" && !String.IsNullOrEmpty(Search))
            {
                //line changed
                var clients = db.Clients.Where(x => x.User.Email.Contains(Search)).ToList();
                return View(clients);
            }
            else if (Request["button"].ToString() == "Password" && !String.IsNullOrEmpty(Search))
            {
                //line changed
                var clients = db.Clients.Where(x => x.User.PasswordHash.Contains(Search)).ToList();
                return View(clients);
            }
            else
            {
                return View(db.Clients.ToList());
            }


        }
        [Authorize]
        public ActionResult CommandeDetails(int id)
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index2", "default");
            }
            var command = db.Commandes.Include(e => e.Client.User).Include(e => e.Produit.ProductCategories.Select(x => x.Categorie)).Include(e => e.Produit.Proprietaire.User).FirstOrDefault(e => e.IdCommande == id);
            ViewBag.count = command.Produit.ProductCategories.Select(e => e.Categorie).ToList().Count();
            
            return View(command);
        }
        [Authorize]
        public ActionResult ViewProducts()
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index2", "default");
            }
            var produits = db.Produits.Include(e => e.Proprietaire.User).ToList();

            return View(produits);
        }
        [HttpPost]
        [Authorize]
        public ActionResult ViewProducts(string Search)
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index2", "default");
            }
            int a;
            Decimal b;

            if (Request["button"].ToString() == "Prix" && !String.IsNullOrEmpty(Search) && Decimal.TryParse(Search, out b))
            {
                Decimal prix = Decimal.Parse(Search);

                var produits = db.Produits.Where(x => x.Prix == prix).Include(e => e.Proprietaire.User).ToList();
                return View(produits);
            }
            else if (Request["button"].ToString() == "Titre" && !String.IsNullOrEmpty(Search))
            {
                var activities = db.Produits.Where(x => x.Titre.Contains(Search)).Include(e => e.Proprietaire.User).ToList();
                return View(activities);
            }
            else if (Request["button"].ToString() == "Produit Id" && !String.IsNullOrEmpty(Search) && Int32.TryParse(Search, out a))
            {
                int id = Int32.Parse(Search);
                var activities = db.Produits.Where(x => x.IdProduit.Equals(id)).Include(e => e.Proprietaire.User).ToList();
                return View(activities);
            }
            else if (Request["button"].ToString() == "Propritaire Id" && !String.IsNullOrEmpty(Search) && Int32.TryParse(Search, out a))
            {
                int id = Int32.Parse(Search);
                var activities = db.Produits.Where(x => x.Proprietaire.IdPropritaire.Equals(id)).Include(e => e.Proprietaire.User).ToList();
                return View(activities);
            }
            else if (Request["button"].ToString() == "Propritaire Nom" && !String.IsNullOrEmpty(Search))
            {
                var activities = db.Produits.Where(x => x.Proprietaire.User.UserName.Contains(Search)).Include(e => e.Proprietaire.User).ToList();
                return View(activities);
            }
            else
            {
                return View(db.Produits.Include(e => e.Proprietaire.User).ToList());
            }

        }
        [Authorize]
        public ActionResult ProductDetails(int id)
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index2", "default");
            }
            var command = db.Produits.Include(e => e.ProductCategories.Select(x => x.Categorie)).Include(e => e.Proprietaire.User).FirstOrDefault(e => e.IdProduit == id);
            ViewBag.c = db.ImageProduits.Where(x => x.Produit.IdProduit == id).ToList().Count;

            return View(command);
        }
    }

}