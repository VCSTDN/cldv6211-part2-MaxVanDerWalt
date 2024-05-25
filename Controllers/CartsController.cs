using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KhumaloCraft.Data;
using KhumaloCraft.Models;
using Microsoft.AspNetCore.Identity;
using KhumaloCraft.ModelViews;


namespace KhumaloCraft.Controllers
{
    public class CartsController : Controller
    {
        private readonly KhumaloCraftContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        public CartsController(KhumaloCraftContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int MyWorkID)
        {
            var user = await _userManager.GetUserAsync(User); // Get the current user
            if (user == null) return Challenge();

            var item = await _context.MyWorkModel.FindAsync(MyWorkID); // Get the work item
            if (item == null) return NotFound("Product is not avaliable");
            
            var cart = await _context.Cart.Include(c => c.CartItems).SingleOrDefaultAsync(c => c.UserID == user.Id); // Get the cart
            if (cart == null) // If the cart is empty, create  a new cart
                cart = new Cart
                {
                    UserID = user.Id
                };
                _context.Cart.Add(cart); 

           //check to see if item is already in cart
           if (cart != null)
            {
                foreach(var product in cart.CartItems)
                {
                    if (product.MyWorkID == MyWorkID)
                    {
                        return RedirectToAction("ViewCart");
                    }
                }
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.MyWorkID == MyWorkID); // Check if the item is already in the cart
            if (cartItem == null)
            {
                cart.CartItems.Add(new CartItems
                {
                    MyWorkID = MyWorkID
                });
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("ViewCart");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int WorkId)
        {
            var item = await _context.CartItems.Include(ci => ci.MyWork).FirstOrDefaultAsync(ci => ci.CartItemsID == WorkId);

            if (item == null)
            {
                return NotFound();
            }
            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();

            return RedirectToAction("ViewCart");
        }

        public async Task<IActionResult> ViewCart()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();
            var cart = await _context.Cart
                                     .Include(c => c.CartItems)
                                     .ThenInclude(ci => ci.MyWork)
                                     .FirstOrDefaultAsync(c => c.UserID == user.Id);

            if (cart == null)
            {
                return View(new CartsModelView());
            }

            var model = new CartsModelView
            {
               CartItems = cart.CartItems.Select(ci => new CartItemsModelView
               {
                   CartItemsID = ci.CartItemsID,
                   WorkName = ci.MyWork.ProductName,
                   Price = ci.MyWork.Price,
                   ImagePath = ci.MyWork.ImagePath

               }).ToList(),
               CartTotal = cart.CartItems.Sum(ci => ci.MyWork.Price)
            };
            return View("ViewCart", model);
        }

        // GET: Carts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.CartID == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Carts/Create
        public IActionResult Create()
        {
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CartID,UserID")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", cart.UserID);
            return View(cart);
        }

        // GET: Carts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", cart.UserID);
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CartID,UserID")] Cart cart)
        {
            if (id != cart.CartID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.CartID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", cart.UserID);
            return View(cart);
        }

        // GET: Carts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.CartID == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cart = await _context.Cart.FindAsync(id);
            if (cart != null)
            {
                _context.Cart.Remove(cart);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(int id)
        {
            return _context.Cart.Any(e => e.CartID == id);
        }
    }
}
