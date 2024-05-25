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
using Microsoft.AspNetCore.Authorization;

namespace KhumaloCraft.Controllers
{
    public class OrdersController : Controller
    {
        private readonly KhumaloCraftContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public OrdersController(KhumaloCraftContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("AdminIndex");
                }
                else
                {
                    return RedirectToAction("UserIndex");
                }
            }
            else
            {
                return Challenge(); // Sign in
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminIndex(OrderStatus? statusFilter)
        {
            // Start the query with the necessary includes
            var query = _context.Orders
                                .Include(o => o.User)
                                .Include(o => o.OrderItems)
                                .ThenInclude(oi => oi.MyWork) // Include related Product if needed
                                .AsQueryable();

            // Apply the status filter if provided
            if (statusFilter.HasValue)
            {
                query = query.Where(o => o.OrderStatus == statusFilter.Value);
            }

            // Store the current filter in ViewData to use in the view
            ViewData["CurrentFilter"] = statusFilter;

            // Execute the query and get the list of orders
            var orders = await query.ToListAsync();

            // Return the view with the list of orders
            return View(orders);
        }

        public async Task<IActionResult> UserIndex()
        {
            var user = await _userManager.GetUserAsync(User);


            var userOrders = await _context.Orders
                .Where(o => o.UserID == user.Id)
                .ToListAsync();
            return View(userOrders);
        }

        public async Task<IActionResult> OrderHistory()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var orders = await _context.Orders
                                       .Where(o => o.UserID == user.Id && o.OrderStatus != OrderStatus.Pending)
                                       .ToListAsync();
            return View(orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeStatus(int orderID, OrderStatus orderStatus)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var order = await _context.Orders.FindAsync(orderID);
            if (order == null)
            {
                return NotFound();
            }

            order.OrderStatus = orderStatus;
            _context.Update(order);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(AdminIndex));
        }



        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.OrderID == id);
            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderID,UserOrderNumber,UserID,TotalPrice,CreatedDate,ModifiedDate,Address,OrderStatus")] Orders orders)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orders);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", orders.UserID);
            return View(orders);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders = await _context.Orders.FindAsync(id);
            if (orders == null)
            {
                return NotFound();
            }
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", orders.UserID);
            return View(orders);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderID,UserOrderNumber,UserID,TotalPrice,CreatedDate,ModifiedDate,Address,OrderStatus")] Orders orders)
        {
            if (id != orders.OrderID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orders);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrdersExists(orders.OrderID))
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
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", orders.UserID);
            return View(orders);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.OrderID == id);
            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orders = await _context.Orders.FindAsync(id);
            if (orders != null)
            {
                _context.Orders.Remove(orders);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrdersExists(int id)
        {
            return _context.Orders.Any(e => e.OrderID == id);
        }
    }
}
