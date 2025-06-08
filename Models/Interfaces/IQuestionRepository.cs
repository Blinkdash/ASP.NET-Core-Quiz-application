using System.Collections.Generic;
using Web6.Models;

namespace Web6.Models.Interfaces;

public interface IQuestionRepository
{
    IEnumerable<Question> GetAllQuestions();
    bool AddQuestion(Question obj);
    bool RemoveQuestion(Question obj);
    bool UpdateQuestion(Question obj);

    IEnumerable<Category> GetCategories();
    bool AddCategory(Category category);
    bool RemoveCategory(Category category);
    bool UpdateCategory(Category category);
}
