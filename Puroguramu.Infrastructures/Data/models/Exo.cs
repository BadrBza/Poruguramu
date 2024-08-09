namespace Puroguramu.Infrastructures.Data.models;

public class Exo
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Difficulty Difficulty { get; set; }
    public bool IsPublished { get; set; }
    public int Order { get; set; }
    public Guid LessonId { get; set; }
    public Lesson Lesson { get; set; }
    public string Template { get; set; }

    public string Stub { get; set; }
    public string Solution { get; set; }
    public List<StudentExercise> StudentExercises { get; set; } = new List<StudentExercise>();
    public List<Tentative> Tentatives { get; set; } = new List<Tentative>();
}
