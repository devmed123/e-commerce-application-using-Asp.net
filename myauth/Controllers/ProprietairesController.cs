
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
   
      
        public ActionResult Dashbord2()
        {
            
            if (!User.IsInRole("Prop"))
            {
                return RedirectToAction("Index2","default");
            }
            String a = User.Identity.GetUserId().ToString();
            return View(db.Commandes.Where(e => e.Produit.Proprietaire.Id == a).Include(e=>e.Produit).Include(e=>e.Client).Include(e => e.Client.User).OrderByDescending(e=>e.Date).ToList());
        }
        public ActionResult Index2(int ? i)
        {
            if (!User.IsInRole("Prop"))
            {
                return RedirectToAction("Index2", "default");
            }
            String a = User.Identity.GetUserId().ToString();
            return View(db.Produits.Where(e => e.Proprietaire.Id == a).ToList().ToPagedList(i ?? 1, 12));
        }

        [HttpPost, ActionName("Index2")]
        public ActionResult Index2Post(int? i)
        {
            int id = int.Parse(Request["id"]);
            Produit produit = db.Produits.Find(id);
            String a = User.Identity.GetUserId().ToString();
            if (!User.IsInRole("Prop") && !(produit.Proprietaire.Id == a))
            {
                return RedirectToAction("Index2", "default");
            }
            foreach (var obj in db.Commandes.Where(e => e.Produit.IdProduit == id).ToList())
                db.Commandes.Remove(obj);
            foreach (var obj in db.Commentaires.Where(e => e.Produit.IdProduit == id).ToList())
                db.Commentaires.Remove(obj);
            foreach (var obj in db.ImageProduits.Where(e => e.Produit.IdProduit == id).ToList())
                db.ImageProduits.Remove(obj);

            db.Produits.Remove(produit);
            db.SaveChanges();
            
            return View(db.Produits.Where(e => e.Proprietaire.Id == a).ToList().ToPagedList(i ?? 1, 12));
        }


        public ActionResult details_product(int? id)
        {
            if (!User.IsInRole("Prop"))
            {
                return RedirectToAction("Index2", "default");
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
                return RedirectToAction("Index2", "default");
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
                return RedirectToAction("Index2", "default");
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
                return RedirectToAction("Index2", "default");
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
                PropActivite pa = new PropActivite();
                pa.Date = DateTime.Now;
                pa.Proprietaire = db.Proprietaires.Where(e => e.Id == a).First();
                pa.Produit = produit;
                pa.Name = "ajout";
                db.PropActivites.Add(pa);
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
                return RedirectToAction("Index2", "default");
            }
            ViewBag.Categories = new SelectList(db.Categories.ToList(), "IdCategorie", "Nom");

            return View();
        }
        // GET: Produits1/Edit/5
        public ActionResult Edition(int? id)
        {
           
            
            Produit produit = db.Produits.Find(id);
            String a = User.Identity.GetUserId().ToString();
            if (!User.IsInRole("Prop") && !(produit.Proprietaire.Id == a))
            {
                return RedirectToAction("Index2", "default");
            }
            if (produit == null)
            {
                return HttpNotFound();
            }
            ViewBag.Categories = new SelectList(db.Categories.ToList(), "IdCategorie", "Nom");
          
            return View(produit);
        }

        // POST: Produits1/Edit/5
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edition([Bind(Include = "IdProduit,Prix,DateAddition,Description,DateLastModification,IsVerified")] Produit produit)
        {
            String a = User.Identity.GetUserId().ToString();
            if (!User.IsInRole("Prop") && !(produit.Proprietaire.Id == a))
            {
                return RedirectToAction("Index2", "default");
            }
            if (ModelState.IsValid)
            {
                db.Entry(produit).State = EntityState.Modified;
                PropActivite pa = new PropActivite();
                pa.Date = DateTime.Now;
                pa.Proprietaire = db.Proprietaires.Where(e => e.Id == a).First();
                pa.Produit = produit;
                pa.Name = "modfifier";
                db.PropActivites.Add(pa);
                db.SaveChanges();
                return RedirectToAction("Index2");
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
                return RedirectToAction("Index2", "default");
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
                return RedirectToAction("Index2", "default");
            }
            db.Produits.Remove(produit);
            db.SaveChanges();
            return RedirectToAction("Index2");
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

