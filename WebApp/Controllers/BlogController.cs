using System;
using ElseForty.Models;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;
using System.Reflection;
using ElseForty.Repositories;
using ElseForty.Filters;
using Microsoft.AspNetCore.Authorization;

namespace ElseForty.Controllers
{

 
    public class BlogController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBlogRepo blogRepo;

        public BlogController(ILogger<HomeController> logger, IBlogRepo blogRepo)
        {
            _logger = logger;
            this.blogRepo = blogRepo;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<BlogPostModel> items = await blogRepo.GetAll();

            return View(items);
        }

        [HttpGet]
        public async Task<IActionResult> Read(string id)
        {
            BlogPostModel result = await blogRepo.Get(id);
            return result == null ? NotFound("Not found") : View(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult New(BlogPostModel model, bool cleanValidation = false)
        {
            if (cleanValidation) ModelState.Clear();
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> New_Post(BlogPostModel model)
        {
            if (ModelState.IsValid)
            {
                await blogRepo.Add(model);
                return RedirectToAction("Index", "Blog");
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            BlogPostModel result = await blogRepo.Get(id);
            return result == null ? NotFound("Not found") : View(result);
        }


        [HttpPost]
        public async Task<IActionResult> Edit_Post(BlogPostModel model)
        {
            if (ModelState.IsValid)
            {
                await blogRepo.Update(model);
                return RedirectToAction("Read", "Blog", new { id = model.id });
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var post = await blogRepo.Get(id);
            if (post != null)
            {
                await blogRepo.Delete(id);
                return RedirectToAction("Index", "Blog");
            }
            else
            {
                return NotFound("Not found");
            }
        }
    }
}

