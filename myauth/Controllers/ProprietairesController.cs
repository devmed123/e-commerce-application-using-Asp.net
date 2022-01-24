
using myauth.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using Microsoft.AspNet.Identity;
using System.Net.Mail;

namespace myauth.Controllers
{   [Authorize]
    public class ProprietairesController : Controller
    {    
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Contact(int id)
        {

            if (!User.IsInRole("Prop"))
            {
                return RedirectToAction("Index", "Home");
            }
           
            return View(db.Commandes.Where(e=>e.IdCommande==id).First());
        }
        [HttpPost]
        public ActionResult Contact(string subject, string message ,int IdCommande)
        {
            Commande c = db.Commandes.Where(e => e.IdCommande == IdCommande).Include(e=>e.Client.User).Include(e => e.Produit).First();
        
                if (ModelState.IsValid)
                {  

                    
                    var senderEmail = new MailAddress("bouzitmed8@gmail.com");
                    var receiverEmail = new MailAddress("bouzitmed9@gmail.com");
                    var password = "28012001/2001";
                    var sub = subject;
                    var body = "k,k,k,";
                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = true,
                        Credentials = new NetworkCredential(senderEmail.Address, password),
                        
                    };
                    using (var mess = new MailMessage(senderEmail, receiverEmail)
                    {
                        Subject = subject,
                        Body = body
                    })
                    {
                        smtp.Send(mess);
                    }
                    return View("Index2");
                }

            return View();
        }
        public ActionResult Dashbord2()
        {
            
            if (!User.IsInRole("Prop"))
            {
                return RedirectToAction("Index","Home");
            }
            String a = User.Identity.GetUserId().ToString();
            return View(db.Commandes.Where(e => e.Produit.Proprietaire.Id == a).Include(e=>e.Produit).Include(e=>e.Client).Include(e => e.Client.User).OrderByDescending(e=>e.Date).ToList());
        }
        public ActionResult Index2(int ? i)
        {
            if (!User.IsInRole("Prop"))
            {
                return RedirectToAction("Index", "Home");
            }
            String a = User.Identity.GetUserId().ToString();
            return View(db.Produits.Where(e => e.Proprietaire.Id == a).ToList().ToPagedList(i ?? 1, 12));
        }
       
       
        public ActionResult details_product(int? id)
        {
            if (!User.IsInRole("Prop"))
            {
                return RedirectToAction("Index", "Home");
            }
           
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            Produit produit = db.Produits.Find(id);
            String a = User.Identity.GetUserId().ToString();
            if (produit == null )
            {
                return HttpNotFound();
            }
            if ( db.Produits.Where(e=>e.IdProduit==id).Include(e=>e.Proprietaire).First().Proprietaire.Id != a)
            {
                return HttpNotFound();
            }


            ViewBag.c = db.ImageProduits.Where(x => x.Produit.IdProduit == id).ToList().Count;
            ViewBag.comments = db.Commentaires.Where(x => x.Produit.IdProduit == id).Include(e=>e.Client.User).ToList();
            return View(produit);
        }
       
        public ActionResult RenderImage(int id, int t)
        {
            if (!User.IsInRole("Prop"))
            {
                return RedirectToAction("Index", "Home");
            }
            var images = db.ImageProduits.Where(x => x.Produit.IdProduit == id).ToList();
            byte[] ph = images[t].Image;
            return File(ph, "image/png");
        }
        // GET: Produits1/Create
        public ActionResult Create()
        {
            if (!User.IsInRole("Prop"))
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Categories = new SelectList(db.Categories.ToList(), "IdCategorie", "Nom");
            return View();
        }
        
        // POST: Produits1/Create
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdProduit,Prix,DateAddition,Description,DateLastModification,IsVerified,Titre")] Produit produit, IEnumerable<String>  photos, IEnumerable<int> Categories)
        {
            if (!User.IsInRole("Prop"))
            {
                return RedirectToAction("Index", "Home");
            }
            String a = User.Identity.GetUserId().ToString();
            if (ModelState.IsValid)
            {
                produit.Proprietaire = db.Proprietaires.Where(e=>e.Id ==a).First();
                produit.DateAddition = DateTime.Now;
                produit.DateLastModification = DateTime.Now;
                foreach (var file in photos)
                {
                    ImageConverter imgCon = new ImageConverter();
                    ImageProduit im = new ImageProduit();
                    Image img = Image.FromFile("C:\\Users\\merye\\Pictures\\images\\" + file);
                    byte[] bytes = (byte[])(new ImageConverter()).ConvertTo(img, typeof(byte[]));
                    im.Image = bytes;
                    im.Produit = produit;
                    db.ImageProduits.Add(im);
                }

                db.Produits.Add(produit);
                
                db.SaveChanges();
                foreach (int x in Categories)
                {
                    ProductCategory productCategories = new ProductCategory
                    {
                        IdCategorie = x,
                        produit = produit
                    };
                    db.ProductCategories.Add(productCategories);
                    db.SaveChanges();
                }
                return RedirectToAction("index2");
            }

            return View(produit);
        }

        public ActionResult edit2(int? id)
        {
            if (!User.IsInRole("Prop"))
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Categories = new SelectList(db.Categories.ToList(), "IdCategorie", "Nom");

            return View();
        }
        // GET: Produits1/Edit/5
        public ActionResult Edit(int? id)
        {
           
            
            Produit produit = db.Produits.Find(id);
            String a = User.Identity.GetUserId().ToString();
            if (!User.IsInRole("Prop") && !(produit.Proprietaire.Id == a))
            {
                return RedirectToAction("Index", "Home");
            }
            if (produit == null)
            {
                return HttpNotFound();
            }
            return View(produit);
        }

        // POST: Produits1/Edit/5
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdProduit,Prix,DateAddition,Description,DateLastModification,IsVerified")] Produit produit)
        {
            String a = User.Identity.GetUserId().ToString();
            if (!User.IsInRole("Prop") && !(produit.Proprietaire.Id == a))
            {
                return RedirectToAction("Index", "Home");
            }
            if (ModelState.IsValid)
            {
                db.Entry(produit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(produit);
        }

        // GET: Produits1/Delete/5
        public ActionResult Delete(int? id)
        {
           
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Produit produit = db.Produits.Find(id);
            String a = User.Identity.GetUserId().ToString();
            if (!User.IsInRole("Prop") && !(produit.Proprietaire.Id == a))
            {
                return RedirectToAction("Index", "Home");
            }
            if (produit == null)
            {
                return HttpNotFound();
            }
            return View(produit);
        }

        // POST: Produits1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
           
            Produit produit = db.Produits.Find(id);
            String a = User.Identity.GetUserId().ToString();
            if (!User.IsInRole("Prop") && !(produit.Proprietaire.Id == a))
            {
                return RedirectToAction("Index", "Home");
            }
            db.Produits.Remove(produit);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

