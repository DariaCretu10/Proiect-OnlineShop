using FinalProiect.Models;
using FinalProiect.ViewModels;
using Incercare2Proiect.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinalProiect.Controllers
{
    [Authorize(Roles = "Admin, Colaborator, User")]
    public class ShoppingCartController : Controller
    {
        ApplicationDbContext storeDB = new ApplicationDbContext();

        // GET: ShoppingCart
        public ActionResult Index()
        {
           

            var CartId = User.Identity.GetUserName();
            var cart = ShoppingCart.GetCart(CartId);

            // Set up our ViewModel
            var viewModel = new ShoppingCartViewModel
            {
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetTotal()
            };
            // Return the view
            return View(viewModel);
        }
        //
        // GET: /Store/AddToCart/5
        public ActionResult AddToCart(int id)
        {
            // Retrieve the album from the database
            var addedProduct = storeDB.Products
                .Single(product => product.ProductId == id);

            // Add it to the shopping cart
            var CartId = User.Identity.GetUserName();
            var cart = ShoppingCart.GetCart(CartId);

            cart.AddToCart(addedProduct);

            // Go back to the main store page for more shopping
            return RedirectToAction("Index");
        }
        //
        // AJAX: /ShoppingCart/RemoveFromCart/5
        [HttpPost]
        public ActionResult RemoveFromCart(int id)
        {
            // Remove the item from the cart
            var CartId = User.Identity.GetUserName();
            var cart = ShoppingCart.GetCart(CartId);

            // Get the name of the album to display confirmation
            string productName = storeDB.Carts
                .Single(item => item.RecordId == id).Product.Nume;

            // Remove from cart
            int itemCount = cart.RemoveFromCart(id);

            // Display the confirmation message
            var results = new ShoppingCartRemoveViewModel
            {
                Message = Server.HtmlEncode(productName) +
                    " has been removed from your shopping cart.",
                CartTotal = cart.GetTotal(),
                CartCount = cart.GetCount(),
                ItemCount = itemCount,
                DeleteId = id
            };
            return RedirectToAction("Index");
        }
      
    }
}