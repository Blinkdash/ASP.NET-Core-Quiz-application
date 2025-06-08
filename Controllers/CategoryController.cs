using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Web6.Models;
using Web6.Models.Interfaces;

namespace Web6.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IQuestionRepository repo;
        public CategoryController(IQuestionRepository repo)
        {
            this.repo = repo;
        }

        // GET: /Category/All?sort=null
        public IActionResult All(string sort = null)
        {
            var list = repo.GetCategories();
            list = sort switch
            {
                "Title" => list.OrderBy(c => c.Title),
                "Description" => list.OrderBy(c => c.Description),
                _ => list.OrderBy(c => c.Id),
            };
            ViewData["Sort"] = sort;
            return View(list.ToList());
        }

        [HttpGet]
        public IActionResult Add()
            => View(new Category());

        [HttpPost]
        public IActionResult Add(Category category)
        {
            if (!ModelState.IsValid)
                return View(category);

            if (!repo.AddCategory(category))
            {
                ModelState.AddModelError("", "Такая категория уже существует.");
                return View(category);
            }

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var cat = repo.GetCategories().FirstOrDefault(c => c.Id == id);
            if (cat == null) return NotFound();
            return View(cat);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (!ModelState.IsValid)
                return View(category);

            if (!repo.UpdateCategory(category))
            {
                ModelState.AddModelError("", "Не удалось обновить категорию.");
                return View(category);
            }

            return RedirectToAction(nameof(All));
        }

        public IActionResult Remove(int id)
        {
            var cat = repo.GetCategories().FirstOrDefault(c => c.Id == id);
            if (cat != null)
                repo.RemoveCategory(cat);
            return RedirectToAction(nameof(All));
        }

        public IActionResult Details(int id)
        {
            var cat = repo.GetCategories().FirstOrDefault(c => c.Id == id);
            if (cat == null) return NotFound();
            return View(cat);
        }
        // GET: /Category/Stat
        public IActionResult Stat()
        {
            var list = repo.GetCategories().ToList();

            ViewData["Count"] = list.Count;
            if (list.Any())
            {
                ViewData["Id"] = new[] { list.Min(c => c.Id), list.Max(c => c.Id) };
                ViewData["Title"] = list.Select(c => c.Title).Distinct().ToList();
                ViewData["Description"] = list.Select(c => c.Description).Distinct().ToList();
            }
            else
            {
                ViewData["Id"] = new[] { 0, 0 };
                ViewData["Title"] = new List<string>();
                ViewData["Description"] = new List<string>();
            }

            return View(list);
        }

        // GET: /Category/Export
        [HttpGet]
        public IActionResult Export(int n = 0, string sort = null)
        {
            ViewBag.N = n;
            ViewBag.Sort = sort;
            return View();
        }

        // POST: /Category/Export
        [HttpPost, ActionName("Export")]
        public IActionResult ExportPost(int n, string sort)
        {
            var list = repo.GetCategories();

            list = sort switch
            {
                "Title" => list.OrderBy(c => c.Title),
                "Description" => list.OrderBy(c => c.Description),
                _ => list.OrderBy(c => c.Id),
            };
            if (n > 0)
                list = list.Take(n);

            var exportData = list.Select(c => new {
                c.Id,
                c.Title,
                c.Description
            }).ToList();

            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(exportData, options);
            byte[] bytes = Encoding.UTF8.GetBytes(json);

            return File(bytes, "application/json", "categories.json");
        }
    }
}
