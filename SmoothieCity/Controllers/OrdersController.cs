using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SmoothieCity.Models;

namespace SmoothieCity.Controllers
{
    public class OrdersController : Controller
    {
        private readonly SmoothieCityContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly String _dbCon;

        public OrdersController(SmoothieCityContext context, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _dbCon = configuration.GetConnectionString("DefaultConnection");
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            if (_signInManager.IsSignedIn(User) && User.IsInRole("Manager"))
            {
                var smoothieCityContext = _context.Order.Include(o => o.Customer);
                return View(await smoothieCityContext.ToListAsync());
            }
            return View("~/Views/RoleManager/AcessDenied.cshtml");
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (_signInManager.IsSignedIn(User) && User.IsInRole("Manager"))
            {
                if (id == null)
                {
                    return NotFound();
                }

                var order = await _context.Order
                    .Include(o => o.Customer)
                    .FirstOrDefaultAsync(m => m.OrderID == id);
                if (order == null)
                {
                    return NotFound();
                }
                var uid = order.CustomerID;
                var oid = order.OrderID;

                

                    //try to populate cart
                    List<int> cartItems = new List<int>();
                    using (var con = new SqlConnection(_dbCon))
                    {
                        con.Open();
                        SqlCommand test = new SqlCommand("SELECT TOP 500 [dbo].OrderItems.SmoothieID, [dbo].[Order].CustomerID FROM[dbo].OrderItems INNER JOIN[dbo].[Order] ON[dbo].[OrderItems].[OrderID] = [dbo].[Order].[OrderID] WHERE [dbo].[Order].[CustomerID] = '" + uid + "'AND [dbo].[Order].[OrderID] = " + oid, con);
                        using (SqlDataReader reader = test.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Debug.WriteLine((int)reader[0]);
                                cartItems.Add((int)reader[0]);
                            }
                        }
                    }

                    ViewBag.CartData = cartItems;


                ViewModel a = new ViewModel()
                {
                    Smoothies = await _context.Smoothies.ToListAsync(),
                    Orders = await _context.Order.ToListAsync(),
                    Passable = order
                };

                return View(a);
            }
            return View("~/Views/RoleManager/AcessDenied.cshtml");
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["CustomerID"] = new SelectList(_context.Customer, "CustomerID", "CustomerID");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderID,SpecialInstructions,OrderTime,PickUpTime,CustomerID")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerID"] = new SelectList(_context.Customer, "CustomerID", "CustomerID", order.CustomerID);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["CustomerID"] = new SelectList(_context.Customer, "CustomerID", "CustomerID", order.CustomerID);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderID,SpecialInstructions,OrderTime,PickUpTime,CustomerID")] Order order)
        {
            if (id != order.OrderID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderID))
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
            ViewData["CustomerID"] = new SelectList(_context.Customer, "CustomerID", "CustomerID", order.CustomerID);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(m => m.OrderID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Order.FindAsync(id);
            _context.Order.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.OrderID == id);
        }
    }
}
