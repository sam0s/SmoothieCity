using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SmoothieCity.Controllers
{
    public class RoleManagerController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public RoleManagerController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Manager"))
            {
                var roles = await _roleManager.Roles.ToListAsync();
                return View(roles);
            }
            return View("~/Views/RoleManager/AcessDenied.cshtml");

        }

        [HttpPost]
        public async Task<IActionResult> AddRole(string roleName)
        {
            if (User.IsInRole("Manager"))
            {
                if (roleName != null)
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName.Trim()));
                }
                return RedirectToAction("Index");
            }
            return View("~/Views/RoleManager/AcessDenied.cshtml");
        }
    }
}
