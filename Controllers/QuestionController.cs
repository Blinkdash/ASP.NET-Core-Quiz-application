using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Web6.Models;
using Web6.Models.Interfaces;

namespace Web6.Controllers
{
    public class QuestionController : Controller
    {
        private readonly IQuestionRepository repo;
        public QuestionController(IQuestionRepository repo)
        {
            this.repo = repo;
        }

        // GET: /Question/All
        public IActionResult All(string sort = null)
        {
            var list = repo.GetAllQuestions();
            list = sort switch
            {
                "Text" => list.OrderBy(q => q.Text),
                "Answer" => list.OrderBy(q => q.Answer),
                "Category" => list.OrderBy(q =>
                    repo.GetCategories()
                        .FirstOrDefault(c => c.Id == q.CategoryId)?.Title),
                _ => list.OrderBy(q => q.Id),
            };

            ViewData["Sort"] = sort;
            ViewBag.Categories = repo.GetCategories().ToList();
            return View(list.ToList());
        }

        // GET: /Question/Add
        [HttpGet]
        public IActionResult Add()
        {
            var categories = repo.GetCategories().ToList();

            ViewBag.NoCategories = !categories.Any();

            ViewBag.Categories = categories
                .Select(c => new SelectListItem(c.Title, c.Id.ToString()))
                .ToList();

            return View(new Question());
        }

        // POST: /Question/Add
        [HttpPost]
        public IActionResult Add(Question question)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = repo.GetCategories()
                                         .Select(c => new SelectListItem(c.Title, c.Id.ToString()))
                                         .ToList();
                return View(question);
            }

            if (!repo.AddQuestion(question))
            {
                ModelState.AddModelError("", "Такой вопрос уже существует.");
                ViewBag.Categories = repo.GetCategories()
                                         .Select(c => new SelectListItem(c.Title, c.Id.ToString()))
                                         .ToList();
                return View(question);
            }

            return RedirectToAction(nameof(All));
        }


        // GET: /Question/Edit
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var question = repo.GetAllQuestions().FirstOrDefault(q => q.Id == id);
            if (question == null) return NotFound();

            ViewBag.Categories = repo.GetCategories()
                                     .Select(c => new SelectListItem(
                                         c.Title,
                                         c.Id.ToString(),
                                         c.Id == question.CategoryId))
                                     .ToList();
            return View(question);
        }

        // POST: /Question/Edit
        [HttpPost]
        public IActionResult Edit(Question question)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = repo.GetCategories()
                                         .Select(c => new SelectListItem(
                                             c.Title,
                                             c.Id.ToString(),
                                             c.Id == question.CategoryId))
                                         .ToList();
                return View(question);
            }

            if (!repo.UpdateQuestion(question))
            {
                ModelState.AddModelError("", "Не удалось обновить вопрос.");
                ViewBag.Categories = repo.GetCategories()
                                         .Select(c => new SelectListItem(
                                             c.Title,
                                             c.Id.ToString(),
                                             c.Id == question.CategoryId))
                                         .ToList();
                return View(question);
            }

            return RedirectToAction(nameof(All));
        }

        // GET: /Question/Remove
        public IActionResult Remove(int id)
        {
            var q = repo.GetAllQuestions().FirstOrDefault(x => x.Id == id);
            if (q != null)
                repo.RemoveQuestion(q);
            return RedirectToAction(nameof(All));
        }

        // GET: /Question/Details
        public IActionResult Details(int id)
        {
            var q = repo.GetAllQuestions().FirstOrDefault(x => x.Id == id);
            if (q == null) return NotFound();
            // подгружаем список категорий для ViewBag
            ViewBag.Categories = repo.GetCategories().ToList();
            return View(q);
        }


        // GET: /Question/Stat
        public IActionResult Stat()
        {
            var list = repo.GetAllQuestions().ToList();
            ViewData["Count"] = list.Count;

            if (list.Any())
            {
                ViewData["Id"] = new[] { list.Min(q => q.Id), list.Max(q => q.Id) };
                ViewData["CategoryId"] = new[] { list.Min(q => q.CategoryId), list.Max(q => q.CategoryId) };
                ViewData["Answer"] = list.Select(q => q.Answer.ToString()).Distinct().ToList();
                ViewData["Text"] = list.Select(q => q.Text).Distinct().ToList();
                ViewData["Comment"] = list.Select(q => q.Comment).Distinct().ToList();
            }
            else
            {
                ViewData["Id"] = new[] { 0, 0 };
                ViewData["CategoryId"] = new[] { 0, 0 };
                ViewData["Answer"] = new List<string>();
                ViewData["Text"] = new List<string>();
                ViewData["Comment"] = new List<string>();
            }

            return View(list);
        }

        // GET: /Question/Export?n=...&sort=...
        [HttpGet]
        public IActionResult Export(int n = 0, string sort = null)
        {
            ViewBag.N = n;
            ViewBag.Sort = sort;
            return View();
        }

        [HttpPost, ActionName("Export")]
        public IActionResult ExportPost(int n, string sort)
        {
            var list = repo.GetAllQuestions();
            list = sort switch
            {
                "Text" => list.OrderBy(q => q.Text),
                "Answer" => list.OrderBy(q => q.Answer),
                "Category" => list.OrderBy(q =>
                                  repo.GetCategories()
                                      .FirstOrDefault(c => c.Id == q.CategoryId)?.Title),
                _ => list.OrderBy(q => q.Id),
            };
            if (n > 0) list = list.Take(n);

            var exportData = list.Select(q => new {
                q.Id,
                q.Text,
                q.Answer,
                q.Comment,
                q.CategoryId,
                CategoryTitle = repo.GetCategories()
                                    .FirstOrDefault(c => c.Id == q.CategoryId)?.Title
            }).ToList();

            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(exportData, jsonOptions);

            var utf8WithBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
            byte[] fileBytes = utf8WithBom.GetBytes(json);
            var result = new FileContentResult(fileBytes, "application/json")
            {
                FileDownloadName = "questions.json"
            };
            Response.Headers["Content-Type"] = "application/json; charset=utf-8";
            Response.Headers["Content-Disposition"] =
                "attachment; filename=\"questions.json\"; filename*=UTF-8''questions.json";

            return result;
        }





        // GET: /Question/Test
        [HttpGet]
        public IActionResult Test(int n = 0, int CategoryId = 0)
        {
            ViewBag.Categories = repo.GetCategories()
                .Select(c => new SelectListItem(
                    c.Title, c.Id.ToString(), c.Id == CategoryId))
                .ToList();
            ViewBag.N = n;
            ViewBag.CategoryId = CategoryId;

            var list = new List<Question>();
            if (CategoryId > 0 || n > 0)
            {
                var all = repo.GetAllQuestions();
                if (CategoryId > 0)
                    all = all.Where(q => q.CategoryId == CategoryId);
                if (n > 0)
                    all = all.Take(n);
                list = all.ToList();
            }

            return View(list);
        }

        // POST: /Question/Test
        [HttpPost, ActionName("Test")]
        public IActionResult RunTest(int n = 0, int CategoryId = 0)
        {
            var answerKeys = Request.Form.Keys.Where(k => k.StartsWith("answer_"));
            int correct = 0, wrong = 0;
            foreach (var key in answerKeys)
            {
                if (!int.TryParse(key["answer_".Length..], out var id)) continue;
                bool userAns = Request.Form[key] == "True";
                var q = repo.GetAllQuestions().FirstOrDefault(x => x.Id == id);
                if (q != null)
                {
                    if (q.Answer == userAns) correct++;
                    else wrong++;
                }
            }

            ViewBag.Correct = correct;
            ViewBag.Wrong = wrong;
            ViewBag.Total = correct + wrong;
            return View("TestResult");
        }
    }
}
