namespace Web6.Models;

public class Question
{
    public int Id { get; set; }
    public string Text { get; set; }
    public bool Answer { get; set; }
    public string Comment { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; }
}
