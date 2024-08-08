namespace Puroguramu.Domains;

public class Exercise
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Guid LessonId { get; set; }
    public DifficultyExo Difficulty { get; set; }
    public bool IsPublished { get; set; }
    public string Template { get; set; }
    public string Stub { get; set; }
    public string Solution { get; set; }

    public string InjectIntoTemplate(string code)
        => Template.Replace("// code-insertion-point", code);
}
