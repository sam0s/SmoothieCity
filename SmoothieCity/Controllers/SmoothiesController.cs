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
                    SqlCommand test = new SqlCommand("SELECT TOP 500 [dbo].OrderItems.SmoothieID, [dbo].[Order].CustomerID FROM[dbo].OrderItems INNER JOIN[dbo].[Order] ON[dbo].[OrderItems].[OrderID] = [dbo].[Order].[OrderID] WHERE[dbo].[Order].[CustomerID] = '" + usr.Id + "'", con);

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
                    SqlCommand test = new SqlCommand("SELECT TOP 2 [OrderID] FROM[dbo].[Order] WHERE CustomerID = '" + usr.Id + "'", con);
                    object result = test.ExecuteScalar();
                    result = (result == DBNull.Value) ? null : result;
                    orderId = (result == null) ? -1 : Convert.ToInt32(result);

                    //Debug.WriteLine("\n *%*%*%**%*%*%*%*%*%*% SQL attempted id: %**%*%*%*%*%*%*%*% \n" + orderId + "\n *%*%*%**%*%*%*%*%*%*% SQL %**%*%*%*%*%*%*%*% \n");
                    //123Pa$$word.
                    if (orderId == -1)
                    {
                        Order o = new Order();
                        o.CustomerID = usr.Id;
                        o.PickUpTime = "default";
                        o.OrderTime = "default";
                        o.SpecialInstructions = "default";

                        
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
                ViewModel mymodel = new ViewModel();
                mymodel.Smoothies = await _context.Smoothies.ToListAsync();
                ViewBag.addedId = smoothieId;

                OrderItems oi = new OrderItems();
                oi.OrderID = orderId;
                oi.SmoothieID = (int)smoothieId;

                if (ModelState.IsValid)
                {
                    _context.Add(oi);
                    await _context.SaveChangesAsync();
                    return View(mymodel);
                }

                return View(mymodel);


            }
            else
            {
                Debug.WriteLine("\n USER \n nosiree");
                Debug.WriteLine("\n USER \n");
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

        // POST: Smoothies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // POST: Smoothies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
    }
}
