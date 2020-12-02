using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmoothieCity.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace SmoothieCity.Controllers
{
    public class SmoothiesController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly String _dbCon;

        private readonly SmoothieCityContext _context;

        public SmoothiesController(SmoothieCityContext context, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _dbCon = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IActionResult> Remove(int idd)
        {
            if (_signInManager.IsSignedIn(User))
            {
                //Get user
                var usr = await _userManager.FindByNameAsync(User.Identity.Name);

                //get order id
                int oid = -1;
                using (var con = new SqlConnection(_dbCon))
                {
                    con.Open();
                    SqlCommand test = new SqlCommand("SELECT TOP 2 [OrderID] FROM[dbo].[Order] WHERE CustomerID = '" + usr.Id + "' AND [dbo].[Order].[Submitted] = 0", con);
                    object result = test.ExecuteScalar();
                    result = (result == DBNull.Value) ? null : result;
                    oid = (result == null) ? -1 : Convert.ToInt32(result);
                }

                using (var con = new SqlConnection(_dbCon))
                {
                    con.Open();
                    SqlCommand rem = new SqlCommand("DELETE TOP(1) FROM [OrderItems] WHERE OrderID = " + oid + " and SmoothieId =" + idd, con);
                    rem.ExecuteNonQuery();
                }
                return RedirectToAction("Cart", "Smoothies");

            }

            return View("~/Views/RoleManager/AcessDenied.cshtml");
        }
            
        
            public async Task<IActionResult> Cart()
        {
            if (_signInManager.IsSignedIn(User))
            {
                //Get user
                var usr = await _userManager.FindByNameAsync(User.Identity.Name);

                //try to populate cart
                List<int> cartItems = new List<int>();
                using (var con = new SqlConnection(_dbCon))
                {
                    con.Open();
                    SqlCommand test = new SqlCommand("SELECT TOP 500 [dbo].OrderItems.SmoothieID, [dbo].[Order].CustomerID FROM[dbo].OrderItems INNER JOIN[dbo].[Order] ON[dbo].[OrderItems].[OrderID] = [dbo].[Order].[OrderID] WHERE [dbo].[Order].[CustomerID] = '" + usr.Id + "'AND [dbo].[Order].[Submitted] = 0", con);
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

                //very useful piece of code to get Order ID based on signed in user. Should be turned into a function probably.
                using (var con = new SqlConnection(_dbCon))
                {
                    con.Open();
                    SqlCommand test = new SqlCommand("SELECT TOP 2 [OrderID] FROM[dbo].[Order] WHERE CustomerID = '" + usr.Id + "' AND [dbo].[Order].[Submitted] = 0", con);
                    object result = test.ExecuteScalar();
                    result = (result == DBNull.Value) ? null : result;
                    ViewBag.orderId = (result == null) ? -1 : Convert.ToInt32(result);
                }
                ViewModel a = new ViewModel(){
                    Smoothies = await _context.Smoothies.ToListAsync(),
                    Orders = await _context.Order.ToListAsync()
                };
                return View(a);
            }
            return View(await _context.Smoothies.ToListAsync());
        }

        public async Task<IActionResult> AddToCart(int? smoothieId)
        {
            if (_signInManager.IsSignedIn(User))
            {
                //Get user
                var usr = await _userManager.FindByNameAsync(User.Identity.Name);

                //Connects to DB and sees if the user has an active order, if not it makes a new one for that user.
                int orderId = -1;
                using (var con = new SqlConnection(_dbCon))
                {
                    con.Open();
                    SqlCommand test = new SqlCommand("SELECT TOP 2 [OrderID] FROM[dbo].[Order] WHERE CustomerID = '" + usr.Id + "' AND [dbo].[Order].[Submitted] = 0", con);
                    object result = test.ExecuteScalar();
                    result = (result == DBNull.Value) ? null : result;
                    orderId = (result == null) ? -1 : Convert.ToInt32(result);

                    //Debug.WriteLine("\n *%*%*%**%*%*%*%*%*%*% SQL attempted id: %**%*%*%*%*%*%*%*% \n" + orderId + "\n *%*%*%**%*%*%*%*%*%*% SQL %**%*%*%*%*%*%*%*% \n");
                    //123Pa$$word.
                    if (orderId == -1)
                    {
                        Order o = new Order() {
                            CustomerID = usr.Id,
                            PickUpTime = DateTime.Now.AddMinutes(15),
                            OrderTime = "default",
                            Submitted = false,
                            SpecialInstructions = "default"
                        };


                        if (ModelState.IsValid)
                        {
                            _context.Add(o);
                            await _context.SaveChangesAsync();
                        }

                        result = test.ExecuteScalar();
                        result = (result == DBNull.Value) ? null : result;
                        orderId = (result == null) ? -1 : Convert.ToInt32(result);

                        //Debug.WriteLine("\n *%*%*%**%*%*%*%*%*%*% SQL fresh id: %**%*%*%*%*%*%*%*% \n" + orderId + "\n *%*%*%**%*%*%*%*%*%*% SQL %**%*%*%*%*%*%*%*% \n");

                    }

                }
                ViewModel mymodel = new ViewModel() {
                    Smoothies = await _context.Smoothies.ToListAsync()
                };

                ViewBag.addedId = smoothieId;

                OrderItems oi = new OrderItems() {
                    OrderID = orderId,
                SmoothieID = (int)smoothieId
            };

                if (ModelState.IsValid)
                {
                    _context.Add(oi);
                    await _context.SaveChangesAsync();
                    return View(mymodel);
                }

                return View(mymodel);


            }

            return Redirect("/Identity/Account/Login");

        }

        // GET: Smoothies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Smoothies.ToListAsync());
        }

        // GET: Smoothies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (_signInManager.IsSignedIn(User) && User.IsInRole("Manager"))
            {
                if (id == null)
                {
                    return NotFound();
                }

                var smoothies = await _context.Smoothies
                    .FirstOrDefaultAsync(m => m.SmoothieId == id);
                if (smoothies == null)
                {
                    return NotFound();
                }

                return View(smoothies);
            }
            return View("~/Views/RoleManager/AcessDenied.cshtml");
        }

        // GET: Smoothies/Create
        public IActionResult Create()
        {
            if (_signInManager.IsSignedIn(User) && User.IsInRole("Manager"))
            {
                return View();
            }
            return View("~/Views/RoleManager/AcessDenied.cshtml");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SmoothieId,SmoothieName,SmoothieCalories,SmoothiePrice,SmoothieIngredients,SmoothieImage,")] Smoothies smoothies)
        {
            if (ModelState.IsValid)
            {
                _context.Add(smoothies);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(smoothies);
        }

        // GET: Smoothies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (_signInManager.IsSignedIn(User) && User.IsInRole("Manager"))
            {
                if (id == null)
                {
                    return NotFound();
                }

                var smoothies = await _context.Smoothies.FindAsync(id);
                if (smoothies == null)
                {
                    return NotFound();
                }
                return View(smoothies);
            }
            return View("~/Views/RoleManager/AcessDenied.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitOrder(int id)
        {
            using (var con = new SqlConnection(_dbCon))
            {
                con.Open();
                SqlCommand test = new SqlCommand("UPDATE [Order] SET Submitted = 1 WHERE OrderID = " + id, con);
                object result = test.ExecuteNonQuery();
            }
            return View("~/Views/Home/Index.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SmoothieId,SmoothieName,SmoothieCalories,SmoothiePrice,SmoothieIngredients,SmoothieImage")] Smoothies smoothies)
        {
            if (id != smoothies.SmoothieId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(smoothies);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SmoothiesExists(smoothies.SmoothieId))
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
            return View(smoothies);
        }

        // GET: Smoothies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var smoothies = await _context.Smoothies
                .FirstOrDefaultAsync(m => m.SmoothieId == id);
            if (smoothies == null)
            {
                return NotFound();
            }

            return View(smoothies);
        }

        // POST: Smoothies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var smoothies = await _context.Smoothies.FindAsync(id);
            _context.Smoothies.Remove(smoothies);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SmoothiesExists(int id)
        {
            return _context.Smoothies.Any(e => e.SmoothieId == id);
        }


        // finalize order initial
        public async Task<IActionResult> Finalize(int? id)
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

            if (_signInManager.IsSignedIn(User))
            {
                var usr = await _userManager.FindByNameAsync(User.Identity.Name);
                if ((usr.Id).Equals(order.CustomerID))
                {
                    ViewData["CustomerID"] = new SelectList(_context.Customer, "CustomerID", "CustomerID", order.CustomerID);
                    return View(order);
                }
                else
                {
                    return View("~/Views/RoleManager/AcessDenied.cshtml");
                }

            }

            return NotFound();
        }

        // finalize order post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Finalize(int id, [Bind("OrderID,SpecialInstructions,OrderTime,PickUpTime,CustomerID")] Order order)
        {
            if (_signInManager.IsSignedIn(User))
            {
                var usr = await _userManager.FindByNameAsync(User.Identity.Name);

                order.Submitted = true;
                DateTime time = DateTime.Now.ToLocalTime();
                order.OrderTime = DateTime.Now.ToLocalTime().ToString();


                int n = order.PickUpTime.Hour;
                
                if (n > 17 || n<7){
                    ViewBag.errormsg = "Enter valid time, between (7am - 7pm)";
                    return View();
                }

                order.CustomerID = usr.Id;
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
                        if (!(_context.Order.Any(e => e.OrderID == id)))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return View("Views/Smoothies/Confirmation.cshtml");
                }
                ViewData["CustomerID"] = new SelectList(_context.Customer, "CustomerID", "CustomerID", order.CustomerID);
                return View(order);
                
            }
            return NotFound();
        }
       
    }

}