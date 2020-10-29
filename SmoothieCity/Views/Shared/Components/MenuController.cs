using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmoothieCity.Models;



namespace SmoothieCity.ViewComponents
{
    public class MenuController : ViewComponent
    {

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly String _dbCon;

        private readonly SmoothieCityContext _context;

        public MenuController(SmoothieCityContext context, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _dbCon = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            int total = 0;
            
            if (_signInManager.IsSignedIn((System.Security.Claims.ClaimsPrincipal)User))
            {
                var usr = await _userManager.FindByNameAsync(User.Identity.Name);
                List<int> cartItems = new List<int>();

                //this finds total items in cart. Might be a better way to do this still.

                using (var con = new SqlConnection(_dbCon))
                {
                    con.Open();
                    SqlCommand test = new SqlCommand("SELECT TOP 500 [dbo].OrderItems.SmoothieID, [dbo].[Order].CustomerID FROM[dbo].OrderItems INNER JOIN[dbo].[Order] ON[dbo].[OrderItems].[OrderID] = [dbo].[Order].[OrderID] WHERE[dbo].[Order].[CustomerID] = '" + usr.Id + "'", con);

                    using (SqlDataReader reader = test.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            total++;
                        }
                    }
                }
            }
            ViewBag.cartTotal = total;
            return View("~/Views/Shared/_LoginPartial.cshtml");
        }

    }
}