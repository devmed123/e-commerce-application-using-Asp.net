using myauth.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;

using System.Net.Http.Headers;
using System.Web.Helpers;

namespace myauth.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default
        ApplicationDbContext db = new ApplicationDbContext();
        
        public ActionResult Index2()
        {
            List<Produit> prod = new List<Produit>();
            double days = 10;
            var latest = DateTime.Now.AddDays(-days);

            while (prod.Count == 0) { 

            prod = db.Produits.Include(e => e.ProductCategories.Select(x => x.Categorie)).Where(e => e.DateAddition> latest).ToList();
            latest = latest.AddDays(-days);
            }


            List<SelectListItem> test = new List<SelectListItem>();

            var categories = db.Categories.ToList();
            ViewBag.categCount = categories;

            foreach (var categ in categories)
            {
                test.Add(new SelectListItem { Text = categ.Nom, Value = categ.IdCategorie.ToString() });
            }
            ViewBag.categ = test;

            return View(prod);
        }

     
       
        //home
        //public ActionResult RenderImage(int id)
        //{
        //    //line changed
        //    var images = db.ImageProduits.Where(x => x.Produit.IdProduit == id).ToList();
        //    if (!(images.Count() == 0))
        //    {
        //        byte[] ph = images[0].Image;


        //        return File(ph, "image/png");

        //    }
        //    return null;
        //}
        public ActionResult RenderImage(int id, int? t)
        {
            //line changed
            var images = db.ImageProduits.Where(x => x.Produit.IdProduit == id).ToList();
            if (!(images.Count() == 0))
            {
                byte[] ph;
                if (t != null)
                {
                     ph = images[(int)t - 1].Image;
                }
                else
                {
                     ph = images[0].Image;
                }


                return File(ph, "image/png");

            }
            return null;
        }



        public ActionResult Details2(int id)
        {
            // Produit prod = db.Produits.Find(id);
            ViewBag.count = db.ImageProduits.Where(e => e.Produit.IdProduit == id).ToList().Count();
            Produit prod = db.Produits.Include(e => e.Proprietaire.User).Include(e=>e.Commentaires.Select(d=>d.Client.User)).FirstOrDefault(e => e.IdProduit == id);

            var tt = db.Produits.Find(id).Proprietaire.User.Email;
         
            return View(prod);
        }
        
        [Authorize]
        [HttpPost]
        public ActionResult Details2(int IdProduit, string comment)
        {
            if (!User.IsInRole("client"))
            {
                return RedirectToAction("index2", "default");
            }
            Commentaire c = new Commentaire();

            c.Message = comment;
            string a = User.Identity.GetUserId().ToString();
            c.Client = db.Clients.Where(e => e.Id == a).First();

            c.Produit = db.Produits.Where(e => e.IdProduit == IdProduit).First();
            db.Commentaires.Add(c);
            db.SaveChanges();
            Produit prod = db.Produits.Include(e => e.Proprietaire.User).Include(e => e.Commentaires).FirstOrDefault(e => e.IdProduit == IdProduit);

            
            return View(prod);
        }
        [Authorize]
        public ActionResult Contact(int id)
        {
            if (!User.IsInRole("Client"))
            {
                return RedirectToAction("index2");
            }
            Produit prod = db.Produits.Include(e => e.Proprietaire.User).FirstOrDefault(e => e.IdProduit == id);
            if (prod == null)
            {
                return HttpNotFound();
            }
            if (Request["call"] == "call")
            {
                Commande com = new Commande()
                {
   
                    Date = DateTime.Now,
                    Produit = db.Produits.Find(id),
                    Client = db.Clients.Find(1),

                };

                db.Commandes.Add(com);
                db.SaveChanges();
            }




            return View( prod);
        }
        //contact
        [HttpPost,ActionName("Contact")]
        [Authorize]
        public ActionResult ContactPost(int id,string description)
        {
            if (!User.IsInRole("Client"))
            {
                return RedirectToAction("Index2", "Home");
            }
            Produit prod = db.Produits.Include(e => e.Proprietaire.User).FirstOrDefault(e => e.IdProduit == id);
            if (prod == null)
            {
                return HttpNotFound();
            }

          
            // Save it in database
            if (Request["call"] != null)
            {
                String a = User.Identity.GetUserId().ToString();
                if (Request["call"] == "call phone")
                {
                    Commande com = new Commande()
                    {

                        Date = DateTime.Now,
                        Produit = db.Produits.Find(id),

                        Client = db.Clients.Where(e => e.Id == a).First(),//current user change
                    Panier = db.Paniers.Find(1),
                    };
                    ClientActivite cltAct = new ClientActivite()
                    {
                        Name = "phone",
                        Date = DateTime.Now,
                        Commande = com,
                        Client = db.Clients.Find(1),
                    };
                    db.ClientActivites.Add(cltAct);
                    db.Commandes.Add(com);
                    db.SaveChanges();
                    ViewBag.Message = true;
                }
                else if (Request["call"] == "send email")
                {
                   
                    Commande com = new Commande()
                    {
                      
                        Date = DateTime.Now,
                        Produit = db.Produits.Find(id),
                        Client = db.Clients.Where(e => e.Id == a).First(),//current user change
                        Panier = db.Paniers.Find(1),
                    };
                    ClientActivite cltAct = new ClientActivite()
                    {
                        Name = "email",
                        Date = DateTime.Now,
                        Commande = com,
                        Client = db.Clients.Find(1),
                    };
                    db.ClientActivites.Add(cltAct);
                    db.Commandes.Add(com);
                    db.SaveChanges();
                    string subject = "A client is interested in your product";
                    string body = "Product " + db.Produits.Find(id).Titre + " <br/> A client has sent you this message <br/>" + description + "<br/> Contact him in this email :" + User.Identity.GetUserName();//current user change
                                                                                                                                                                                                                                                                    // WebMail.Send("mouad.aboutajedyne@gmail.com", subject, body, null, null, null, true, null, null, null, null, null, null);//db.Produits.Find(id).Proprietaire.User.Email
                    try {
                        WebMail.Send(db.Produits.Find(id).Proprietaire.User.Email, subject, body, null, null, null, true, null, null, null, null, null, null);
                        ViewBag.msg = " email sent ...";
                    }
                    catch
                    {
                        ViewBag.msg = " error  ...";
                    }

                   
                }
                else if (Request["call"] == "continue shopping")
                {
                    return View("Index2");
                }
            }
                
                //Return Success message
               


            return View(prod);
        }
        //detqils

        //public ActionResult Details(int id)
        //{
        //   // Produit prod = db.Produits.Find(id);
        //    ViewBag.count = db.ImageProduits.Where(e => e.Produit.IdProduit == id).ToList().Count();
        //    Produit prod = db.Produits.Include(e => e.Proprietaire.User).FirstOrDefault(e => e.IdProduit == id);


        //  //  var tesdt = db.Produits.ToList();
        ////    tesdt.Sort();

        //    var tt = db.Produits.Find(id).Proprietaire.User.Email;
        //    return View(prod) ;
        //}
        //[HttpPost,ActionName("Details")]
        //public ActionResult DetailsPost(int id)
        //{
        //    string subject = "hhhhhi";
        //    string body = "jjjjjjj";
        //    WebMail.Send("hajaramak01@gmail.com", subject, body, null, null, null, true, null, null, null, null, null ,null);

        //    if (Request["call"] == "call")
        //    {
        //        Commande com = new Commande()
        //        {
        //            Quantite = 1,
        //            Date = DateTime.Now,
        //            Produit = db.Produits.Find(id),
        //            Client = db.Clients.Find(1),
                   
        //        };

        //        db.Commandes.Add(com);
        //        db.SaveChanges();
        //    }
        //        //{
        //        //    EmailClass.GmailUsername = "amakhajar@gmail.com";
        //        //    EmailClass.GmailPassword = "nouveaumotdepasse";

        //        //    EmailClass mailer = new EmailClass();
        //        //    mailer.ToEmail = "mouad.aboutajedyne@gmail.com";
        //        //    mailer.Subject = "Verify your email id";
        //        //    mailer.Body = "Thanks for Registering your account.<br> please verify your email id by clicking the link <br> <a href='youraccount.com/verifycode=12323232'>verify</a>";
        //        //    mailer.IsHtml = true;
        //        //    mailer.Senda();
        //        //}


        //        Produit prod = db.Produits.Find(id);
        //    if (prod == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(prod);
        //}
        //public ActionResult Contacter()
        //{
        //    int id = 1;
        //    Produit num = db.Produits.Include(e => e.Proprietaire.User).FirstOrDefault(e => e.IdProduit == id);


        //    Panier p = db.Paniers.Find(1);

        //    Commande com = new Commande()
        //    {
        //        Quantite = 1,
        //        Date = DateTime.Now,
        //        Produit = db.Produits.Find(id),
        //        Client = db.Clients.Find(1),
        //        Panier = p
        //    };

        //    db.Commandes.Add(com);
        //    db.SaveChanges();
        //    return null;
          
        //}
        //[HttpPost,ActionName("Contacter")]
        //public ActionResult ContacterPost()
        //{
        //    int id = 1;
        //    Produit num = db.Produits.Include(e => e.Proprietaire.User).FirstOrDefault(e => e.IdProduit == id);


        //    Panier p = db.Paniers.Find(1);

        //    Commande com = new Commande()
        //    {
        //        Quantite = 1,
        //        Date = DateTime.Now,
        //        Produit = db.Produits.Find(id),
        //        Client = db.Clients.Find(1),
        //        Panier = p
        //    };

        //    db.Commandes.Add(com);
        //    db.SaveChanges();
        //    return null;
           
        //}

        //public ActionResult WishList()
        //{
        //    var Produit = db.Produits.ToList();
        //    return View(Produit);
        //}

        //public ActionResult List()
        //{
        //    var Produit = db.Produits.ToList();
        //    return View(Produit);
        //}

        ////shopping cart
        //public ActionResult Cart(int id)
        //{
        //  //  var produit = db.Produits.Find(id);
          
        //    Panier p = new Panier();


        //    Commande com = new Commande()
        //    {
        //        Quantite = 1,
        //        Date = DateTime.Now,
        //        Produit = db.Produits.Find(id),
        //        Client = db.Clients.Find(1)
        //    };
          
           
        //    db.Commandes.Add(com);
        //    db.Paniers.Add(p);
        //    db.SaveChanges();



        //    return View("List",p);
        //}

        //categorie
        public ActionResult Categ2(int id)
        {
            var cat = db.Categories.Include(e => e.ProductCategories).Include(e => e.ProductCategories.Select(x => x.produit)).Include(e => e.ProductCategories.Select(x => x.produit.ProductCategories.Select(t => t.Categorie))).FirstOrDefault(e => e.IdCategorie == id);
            List<SelectListItem> test = new List<SelectListItem>();

            var categories = db.Categories.ToList();
            ViewBag.categCount = categories;

            foreach (var categ in categories)
            {
                test.Add(new SelectListItem { Text = categ.Nom, Value = categ.IdCategorie.ToString() });
            }
            ViewBag.categ = test;

            return View(cat);
        }
        [HttpPost, ActionName("Categ2")]

        public ActionResult CategPost2(int id)
        {

            // var produit = db.Produits.Include(e => e.ProductCategories.Select(x => x.IdCategorie==id)).ToList();
            var cat = db.Categories.Include(e => e.ProductCategories).Include(e => e.ProductCategories.Select(x => x.produit)).Include(e => e.ProductCategories.Select(x => x.produit.ProductCategories.Select(t => t.Categorie))).FirstOrDefault(e => e.IdCategorie == id);
            List<SelectListItem> test = new List<SelectListItem>();

            var categories = db.Categories.ToList();
            ViewBag.categCount = categories;

            foreach (var categ in categories)
            {
                test.Add(new SelectListItem { Text = categ.Nom, Value = categ.IdCategorie.ToString() });
            }
            ViewBag.categ = test;

            return View(cat);
        }


        //public ActionResult Categ(int id)
        //{
        //    var cat = db.Categories.Include(e => e.ProductCategories).Include(e => e.ProductCategories.Select(x => x.produit)).Include(e => e.ProductCategories.Select(x => x.produit.ProductCategories.Select(t =>t.Categorie))).FirstOrDefault(e => e.IdCategorie == id);

        //    return View(cat);
        //}
        //[HttpPost,ActionName("Categ")]

        //public ActionResult CategPost(int id)
        //{

        //    // var produit = db.Produits.Include(e => e.ProductCategories.Select(x => x.IdCategorie==id)).ToList();
        //    var cat = db.Categories.Include(e => e.ProductCategories).Include(e => e.ProductCategories.Select(x => x.produit)).Include(e => e.ProductCategories.Select(x => x.produit.ProductCategories.Select(t => t.Categorie))).FirstOrDefault(e => e.IdCategorie == id);


        //    return View(cat);
        //}
        //public ActionResult Test()
        //{
        //    var prod = db.ProductCategories.ToList();
        //    return View(prod);
        //}

        public ActionResult Rechercher()
        {
            List<SelectListItem> test = new List<SelectListItem>();

            var categories = db.Categories.ToList();
            ViewBag.categCount = categories;

            foreach (var categ in categories)
            {
                test.Add(new SelectListItem { Text = categ.Nom, Value = categ.IdCategorie.ToString() });
            }
            ViewBag.categ = test;
            return View();
        }
        [HttpPost]

        public ActionResult Rechercher(string recherche, Categorie categorie)
        {
            List<Produit> result = new List<Produit>();
            if (categorie.IdCategorie != 0 && categorie!=null)
            {
                //.Where(a => a.Titre.Contains(recherche)).ToList();
                //result = db.Produits.Include(e => e.ProductCategories.Select(x => x.Categorie)).Select(t => t.ProductCategories).Where(a => a.IdCategorie == categorie.IdCategorie)).Select(p => p.produit).ToList();
                var resultids = db.Categories.Include(e => e.ProductCategories).Include(e => e.ProductCategories.Select(x => x.produit)).Include(e => e.ProductCategories.Select(x => x.produit.ProductCategories.Select(t => t.Categorie))).FirstOrDefault(e => e.IdCategorie == categorie.IdCategorie).ProductCategories.Select(a => a.IdProduit).ToList();
                foreach (var obj in resultids)
                {
                    if (db.Produits.Find(obj).Titre.Contains(recherche))
                        result.Add(db.Produits.Include(e => e.ProductCategories.Select(x => x.Categorie)).FirstOrDefault(e => e.IdProduit == obj));
                }
            }
            else
            {
                 result = db.Produits.Include(e => e.ProductCategories.Select(x => x.Categorie)).Where(a => a.Titre.Contains(recherche)).ToList();
            }

            //List<string> list = new List<string>();
            //var prod = db.Produits.Include(e => e.ProductCategories.Select(x => x.Categorie)).FirstOrDefault(t => t.IdProduit == 16).ProductCategories.ToList();
            //foreach (var obj in prod)
            //{
            //    list.Add(obj.Categorie.Nom);
            //}
            List<SelectListItem> test = new List<SelectListItem>();

            var categories = db.Categories.ToList();
            ViewBag.categCount = categories;

            foreach (var categ in categories)
            {
                test.Add(new SelectListItem { Text = categ.Nom, Value = categ.IdCategorie.ToString() });
            }
            ViewBag.categ = test;

            return View(result);
        }
       

        [HttpPost]
        public ActionResult getSelectedValue()
        {
            int selectedValue = Int32.Parse(Request.Form["LogOffTime"].ToString()); //this will get selected value
                                                                                    //  return View("Categ", selectedValue);
            
            return RedirectToAction("Categ2", new { id = selectedValue });
        }






    }
}