using System;
using ElseForty.Models;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ElseForty.Repositories;
using ElseForty.Filters;
using Microsoft.AspNetCore.Authorization;

namespace ElseForty.Controllers
{
    public class BugController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration configuration;
        private readonly IBugRepo bugRepo;

        public string? bugResolvedEventEndpoint { get; set; }
        public string? startOrchestrationClientEndPoint { get; set; }
        public BugController(ILogger<HomeController> logger, IConfiguration configuration, IBugRepo bugRepo)
        {
            _logger = logger;
            this.configuration = configuration;
            this.bugRepo = bugRepo;
            bugResolvedEventEndpoint = configuration["raiseResolvedEventEndpoint"];
            startOrchestrationClientEndPoint = configuration["starterEndPoint"];
        }

        [HttpGet]
        public async Task<IActionResult> ViewBugReport(string id)
        {
            var bugReport = await bugRepo.Get(id);
            return View(bugReport);
        }

        [HttpGet]
        public IActionResult Report(BugModel model, bool cleanValidation = false)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("List");
                }
            }

            if (cleanValidation) ModelState.Clear();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Report(BugModel model)
        {
            if (ModelState.IsValid)
            {
                model.creationTime = DateTime.Now;
                using (var client = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(model);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(startOrchestrationClientEndPoint, content);
                    response.EnsureSuccessStatusCode();
                }
                return RedirectToAction("Index", "Home");
            }
            return View("Report");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Resolve(string id)
        {
            var bugReport = await bugRepo.Get(id);
            return View(bugReport);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Resolve(BugModel model)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(model);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(bugResolvedEventEndpoint, content);
                    response.EnsureSuccessStatusCode();
                }
                return RedirectToAction("Index", "Home");
            }
            return View("Resolve");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var bugs = await bugRepo.GetAll();
            return View(bugs);
        }
    }
}

