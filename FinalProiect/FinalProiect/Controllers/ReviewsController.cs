using FinalProiect.Models;
using Incercare2Proiect.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Incercare2Proiect.Controllers
{
    public class ReviewsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Reviews
        public ActionResult Index()
        {
            return View();
        }

        [HttpDelete]
        [Authorize(Roles = "User,Colaborator,Admin")]
        public ActionResult Delete(int id)
        {
            Review rev = db.Reviews.Find(id);
            Product prod = db.Products.Find(rev.ProductId);
            
            if (rev.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                prod.Numar = prod.Numar - rev.Nota; prod.Suma = prod.Suma - 1;
                if (prod.Suma == 0)
                    prod.Medie = 0;
                else
                    prod.Medie = prod.Numar / prod.Suma;
                db.Reviews.Remove(rev);
                db.SaveChanges();
                return Redirect("/Products/Show/" + rev.ProductId);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari";
                return RedirectToAction("Index", "Products");
            }
        }

       /*[HttpPost]
        public ActionResult New(Review rev)
        {
            rev.Date = DateTime.Now;
            try
            {
                db.Reviews.Add(rev);
                db.SaveChanges();
                return Redirect("/Products/Show/" + rev.ProductId);
            }

            catch (Exception e)
            {
                return Redirect("/Products/Show/" + rev.ProductId);
            }

        }*/
        [Authorize(Roles = "User,Colaborator,Admin")]
        public ActionResult Edit(int id)
        {
            Review rev = db.Reviews.Find(id);
            if (rev.UserId == User.Identity.GetUserId()||User.IsInRole("Admin"))
            {
                return View(rev);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari";
                return RedirectToAction("Index", "Products");
            }
        }

        [HttpPut]
        [Authorize(Roles = "User,Colaborator,Admin")]
        public ActionResult Edit(int id, Review requestReview)
        {
            try
            {
                Review rev = db.Reviews.Find(id);
                if (rev.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
                {
                    if (TryUpdateModel(rev))
                    {
                        rev.Continut = requestReview.Continut;
                        db.SaveChanges();
                    }
                    return Redirect("/Products/Show/" + rev.ProductId);
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa faceti modificari";
                    return RedirectToAction("Index", "Products");
                }
            }
            catch (Exception e)
            {
                return View(requestReview);
            }

        }
    }
}