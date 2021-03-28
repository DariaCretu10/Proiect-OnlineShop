using FinalProiect.Models;
using Incercare2Proiect.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Incercare2Proiect.Controllers
{
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private int _perPage = 6;
        // GET: Products

        public ActionResult Index(int? orderBy)
        {
            // aducem produsele din memeorie in variabila products
            IEnumerable<Product> products = db.Products.Where( a => a.Accept == true).Include("Category").Include("User").OrderBy(async => async.Pret);
            ViewBag.Products = products;
            // sortam produsele in functie de alegerea noastra
            if (orderBy == 1)
            {
                products = db.Products.Include("Category").Include("User").OrderBy(a => a.Pret);
            }
            else if (orderBy == 2)
            {
                products = db.Products.Include("Category").Include("User").OrderByDescending(a => a.Pret);
            }
            else if (orderBy == 3)
            {
                products = db.Products.Include("Category").Include("User").OrderBy(a => a.Nume);
            }
            else if (orderBy == 4) // if (orderBy == 4)
            {
                products = db.Products.Include("Category").Include("User").OrderByDescending(a => a.Nume);
            }
            else 
            {
                products = db.Products.Include("Category").Include("User").OrderByDescending(a => a.Medie);
            }
            //var products = db.Products.Include("Category").Include("User").OrderBy(a => a.Pret);
            var produse = db.Products.Where(a => a.Accept == true).Include("Category").Include("User").OrderBy(a => a.Pret);
            ViewBag.Products = produse;
            var search = "";

            if ( Request.Params.Get("search") != null)
            {
                // alegem produsele care contin ce am introdus in search
                search = Request.Params.Get("search").Trim();
                List<int> productIds = db.Products.Where(
                    at => at.Nume.Contains(search)
                    || at.Descriere.Contains(search)
                    ).Select(a => a.ProductId).ToList();

                List<int> reviewIds = db.Reviews.Where(c => c.Continut.Contains(search))
                    .Select(rev => rev.ProductId).ToList();

                List<int> mergeIds = productIds.Union(reviewIds).ToList();

                // alegem produsele in functie de ce am aleg la sortare
                if (orderBy == 1)
                {
                    products = db.Products.Where(product => mergeIds.Contains(product.ProductId)).Include("Category").Include("User").OrderBy(async => async.Pret);
                }
                else if (orderBy == 2)
                {
                    products = db.Products.Where(product => mergeIds.Contains(product.ProductId)).Include("Category").Include("User").OrderByDescending(async => async.Pret);
                }
                else if (orderBy == 3)
                {
                    products = db.Products.Where(product => mergeIds.Contains(product.ProductId)).Include("Category").Include("User").OrderBy(async => async.Nume);
                }
                else  if (orderBy == 4)
                {
                    products = db.Products.Where(product => mergeIds.Contains(product.ProductId)).Include("Category").Include("User").OrderByDescending(async => async.Nume);
                }

                else 
                {
                    products = db.Products.Where(product => mergeIds.Contains(product.ProductId)).Include("Category").Include("User").OrderByDescending(async => async.Medie);
                }

                //products = db.Products.Where(product => mergeIds.Contains(product.ProductId)).Include("Category").Include("User").OrderBy(async => async.Pret);
            }
            // numaram produsele de afisat
            var totalItems = products.Count();
            var currentPage = Convert.ToInt32(Request.Params.Get("page"));

            var offset = 0;

            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * this._perPage;
            }

            var paginatedProducts = products.Skip(offset).Take(this._perPage).OrderByDescending(async => async.Nume);
            if (orderBy == 1)
            {
                paginatedProducts = products.Skip(offset).Take(this._perPage).OrderBy(async => async.Pret);
            }
            else if (orderBy == 2)
            {
                paginatedProducts = products.Skip(offset).Take(this._perPage).OrderByDescending(async => async.Pret);
            }
            else if (orderBy == 3)
            {
                paginatedProducts = products.Skip(offset).Take(this._perPage).OrderBy(async => async.Nume);
            }
            else
            {
                paginatedProducts = products.Skip(offset).Take(this._perPage).OrderByDescending(async => async.Medie);
            }
            /*
            else //if (orderBy == 4)
            {
                paginatedProducts = products.Skip(offset).Take(this._perPage).OrderByDescending(async => async.Nume);
            }
            */

            // var paginatedProducts = products.Skip(offset).Take(this._perPage);

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            ViewBag.perPage = this._perPage;
            ViewBag.total = totalItems;
            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)this._perPage);

            ViewBag.Products = paginatedProducts;

            ViewBag.OrderBy = orderBy;

            return View();
        }

        //GET : Products(In asteptare)
        public ActionResult Waiting()
        {
            var products = db.Products.Where(a => a.Accept == false).Include("Category").Include("User").OrderBy(async => async.Pret);
            ViewBag.Products = products;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            return View();
        }

        // GET: Upload
        [Authorize(Roles = "Admin, Colaborator")]
        public ActionResult SaveImages()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Colaborator")]
        public ActionResult SaveImages(HttpPostedFileBase file)
        {

            var path = "";
            if (file != null)
            {
                if (file.ContentLength > 0)
                {
                    if (Path.GetExtension(file.FileName).ToLower() == ".jpg"
                    || Path.GetExtension(file.FileName).ToLower() == ".jpeg"
                    || Path.GetExtension(file.FileName).ToLower() == ".png"
                    || Path.GetExtension(file.FileName).ToLower() == ".gif")
                    {
                        path = Path.Combine(Server.MapPath("~/Content/UploadedImages"), file.FileName);
                        file.SaveAs(path);
                        return RedirectToAction("New");

                    }
                    else
                    {
                        return View();
                    }
                }
                else
                { return View(); }

            }
            else
            {
                return View();
            }


        }

        public ActionResult Show(int id)
        {
            
            Product product = db.Products.Find(id);
            ViewBag.afisareButoane = false;

            if (User.IsInRole("Colaborator") || User.IsInRole("Admin"))
            {
                ViewBag.afisareButoane = true;
            }

            ViewBag.esteAdmin = User.IsInRole("Admin");
            ViewBag.utilizatorCurent = User.Identity.GetUserId();
            /////////
            var rol = false;
            if (User.IsInRole("Admin") || User.IsInRole("Colaborator") || User.IsInRole("User"))
                rol = true;
            ViewBag.rol = rol;
            ViewBag.afisareButoane1 = false;
            if (User.IsInRole("Admin"))
            {
                ViewBag.afisareButoane1 = true;
            }
            ViewBag.utilizatorCurent1 = User.Identity.GetUserId();
            return View(product);
        }

        [HttpPost]
        public ActionResult Show(Review rev)
        {
            
            rev.Date = DateTime.Now;
            rev.UserId = User.Identity.GetUserId();
            Product prod = db.Products.Find(rev.ProductId);
            try
            {
                if (ModelState.IsValid)
                {
                    
                    db.Reviews.Add(rev);
                    prod.Suma = prod.Suma + 1;
                    prod.Numar = prod.Numar + rev.Nota;
                    prod.Medie = prod.Numar / prod.Suma;
                    db.SaveChanges();
                    return Redirect("/Products/Show/" + rev.ProductId);
                }
                
                else
                {
                    Product a = db.Products.Find(rev.ProductId);
                    return View(a);
                }

            }

            catch (Exception e)
            {
                Product a = db.Products.Find(rev.ReviewId);
                return View(a);
            }

        }

        //Adauga
        [Authorize(Roles = "Admin,Colaborator")]
        public ActionResult New()
        {
            Product product = new Product();
            product.Categ = GetAllCategories();
            product.UserId = User.Identity.GetUserId();
            
            return View(product);
                
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Colaborator")]
        public ActionResult New(Product product)
        {
            product.UserId = User.Identity.GetUserId();
            product.Accept = true;
            if (User.IsInRole("Admin"))
                product.Accept = true;
            else
                product.Accept = false;
            try
            {
                if (ModelState.IsValid)
                {
                    db.Products.Add(product);
                    db.SaveChanges();
                    TempData["message"] = "Produsul a fost adaugat";
                    return RedirectToAction("Index");
                }
                else
                {
                    product.Categ = GetAllCategories();
                    return View(product); }
                
            }
            catch (Exception e)
            {
                product.Categ = GetAllCategories();
                return View(product);
            }
        }

        //Edit
        [Authorize(Roles = "Colaborator,Admin")]
        public ActionResult Edit(int id)
        {
            Product product = db.Products.Find(id);
            product.Categ = GetAllCategories();
            if (product.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                return View(product);
            }

            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui articol care nu va apartine";
                return RedirectToAction("Index");
            }

        }


        [HttpPut]
        [Authorize(Roles = "Colaborator,Admin")]
        public ActionResult Edit(int id, Product requestProduct)
        {
            requestProduct.Categ = GetAllCategories();
            try
            {

                if (ModelState.IsValid)
                {
                    Product product = db.Products.Find(id);
                    if (product.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
                    {
                        if (TryUpdateModel(product))
                        {
                            //product = requestProduct;
                            product.Nume = requestProduct.Nume;
                            product.Pret = requestProduct.Pret;
                            product.Descriere = requestProduct.Descriere;
                            product.CategoryId = requestProduct.CategoryId;
                            db.SaveChanges();
                            TempData["message"] = "Produsul a fost modificat";

                        }
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui articol care nu va apartine";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    requestProduct.Categ = GetAllCategories();
                    return View(requestProduct); }
                    
               
            }
            catch (Exception e)
            {
                requestProduct.Categ = GetAllCategories();
                return View(requestProduct);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Colaborator,Admin")]
        public ActionResult Delete(int id)
        {
            Product product = db.Products.Find(id);
            if (product.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                db.Products.Remove(product);
                db.SaveChanges();
                TempData["message"] = "Produsul a fost sters";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un articol care nu va apartine";
                return RedirectToAction("Index");
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public ActionResult Acceptare(int id)
        {
            Product product = db.Products.Find(id);
            if (User.IsInRole("Admin"))
            {
                product.Accept = true;
                db.SaveChanges();
                TempData["message"] = "Produsul a fost adaugat";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un articol care nu va apartine";
                return RedirectToAction("Index");
            }
        }
        
        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            // generam o lista goala
            var selectList = new List<SelectListItem>();

            // extragem toate categoriile din baza de date
            var categories = from cat in db.Categories
                             select cat;

            // iteram prin categorii
            foreach (var category in categories)
            {
                // adaugam in lista elementele necesare pentru dropdown
                selectList.Add(new SelectListItem
                {
                    Value = category.CategoryId.ToString(),
                    Text = category.CategoryName.ToString()
                });
            }
            /*
            foreach (var category in categories)
            {
                var listItem = new SelectListItem();
                listItem.Value = category.CategoryId.ToString();
                listItem.Text = category.CategoryName.ToString();

                selectList.Add(listItem);
            }*/

            // returnam lista de categorii
            return selectList;
        }
    }
}