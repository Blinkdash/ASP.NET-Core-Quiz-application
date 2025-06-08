using System.Collections.Generic;
using System.Linq;
using Web6.Models;
using Web6.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Web6.Data
{
    public class EFRepository : IQuestionRepository
    {
        private readonly QuestionContext ctx;
        public EFRepository(QuestionContext context)
        {
            ctx = context;
        }

        // --- Вопросы ---
        public IEnumerable<Question> GetAllQuestions()
            => ctx.Questions
                  .Include(q => q.Category)
                  .AsNoTracking()
                  .ToList();

        public bool AddQuestion(Question q)
        {
            if (ctx.Questions.Any(x =>
                  x.Text == q.Text
               && x.Answer == q.Answer
               && x.Comment == q.Comment
               && x.CategoryId == q.CategoryId))
                return false;

            ctx.Questions.Add(q);
            ctx.SaveChanges();
            return true;
        }

        public bool RemoveQuestion(Question q)
        {
            var ex = ctx.Questions.Find(q.Id);
            if (ex == null) return false;
            ctx.Questions.Remove(ex);
            ctx.SaveChanges();
            return true;
        }

        public bool UpdateQuestion(Question q)
        {
            var ex = ctx.Questions.Find(q.Id);
            if (ex == null) return false;

            ex.Text = q.Text;
            ex.Answer = q.Answer;
            ex.Comment = q.Comment;
            ex.CategoryId = q.CategoryId;
            ctx.SaveChanges();
            return true;
        }

        // --- Категории ---
        public IEnumerable<Category> GetCategories()
            => ctx.Categories
                  .Include(c => c.Questions)
                  .AsNoTracking()
                  .ToList();

        public bool AddCategory(Category c)
        {
            if (ctx.Categories.Any(x =>
                  x.Title == c.Title
               && x.Description == c.Description))
                return false;

            ctx.Categories.Add(c);
            ctx.SaveChanges();
            return true;
        }

        public bool RemoveCategory(Category c)
        {
            var ex = ctx.Categories.Find(c.Id);
            if (ex == null) return false;
            ctx.Categories.Remove(ex);
            ctx.SaveChanges();
            return true;
        }

        public bool UpdateCategory(Category c)
        {
            var ex = ctx.Categories.Find(c.Id);
            if (ex == null) return false;
            ex.Title = c.Title;
            ex.Description = c.Description;
            ctx.SaveChanges();
            return true;
        }
    }
}
