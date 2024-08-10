namespace Puroguramu.Domains.modelsDomains;

public class ExerciseEditDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Template { get; set; }
    public string Stub { get; set; }
    public string Solution { get; set; }
    public Guid LessonId { get; set; }
    public DifficultyExo Difficulty { get; set; }
}
