using System.Collections.Generic;
using System.Linq;
using Web6.Models;
using Web6.Models.Interfaces;

namespace Web6.Data
{
    public class MemoryRepository : IQuestionRepository
    {
        private readonly List<Question> questions = new();
        private readonly List<Category> categories = new();

        // --- Вопросы ---

        public IEnumerable<Question> GetAllQuestions() => questions;

        public bool AddQuestion(Question q)
        {
            if (q.Id == 0)
                q.Id = questions.Any() ? questions.Max(x => x.Id) + 1 : 1;

            if (questions.Any(x =>
                  x.Id == q.Id
               || (x.Text == q.Text
                   && x.Answer == q.Answer
                   && x.Comment == q.Comment
                   && x.CategoryId == q.CategoryId)))
                return false;

            questions.Add(q);
            return true;
        }

        public bool RemoveQuestion(Question q) => questions.Remove(q);

        public bool UpdateQuestion(Question q)
        {
            var ex = questions.FirstOrDefault(x => x.Id == q.Id);
            if (ex == null) return false;
            ex.Text = q.Text;
            ex.Answer = q.Answer;
            ex.Comment = q.Comment;
            ex.CategoryId = q.CategoryId;
            ex.Category = categories.FirstOrDefault(c => c.Id == q.CategoryId);
            return true;
        }


        public IEnumerable<Category> GetCategories() => categories;

        public bool AddCategory(Category c)
        {
            if (c.Id == 0)
                c.Id = categories.Any() ? categories.Max(x => x.Id) + 1 : 1;

            if (categories.Any(x =>
                  x.Id == c.Id
               || (x.Title == c.Title && x.Description == c.Description)))
                return false;

            categories.Add(c);
            return true;
        }

        public bool RemoveCategory(Category c) => categories.Remove(c);

        public bool UpdateCategory(Category c)
        {
            var ex = categories.FirstOrDefault(x => x.Id == c.Id);
            if (ex == null) return false;
            ex.Title = c.Title;
            ex.Description = c.Description;
            return true;
        }
    }
}
