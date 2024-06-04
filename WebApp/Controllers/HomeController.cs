using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ElseForty.Models;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Microsoft.Azure.Cosmos;
using Microsoft.AspNetCore.Identity;

namespace ElseForty.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<IdentityUser> UserManager;
    private readonly RoleManager<IdentityRole> roleManager;
 

    public HomeController(ILogger<HomeController> logger,
        UserManager<IdentityUser > userManager, RoleManager<IdentityRole> roleManager )
    {
        _logger = logger;
        this.UserManager = userManager;
        this.roleManager = roleManager;
    }

    public  IActionResult Index( )
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

