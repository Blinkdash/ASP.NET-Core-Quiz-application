using System.Collections.Generic;

namespace Web6.Models;

public class Category
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }

    public List<Question> Questions { get; set; } = new();
}
