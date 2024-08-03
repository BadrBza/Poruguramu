namespace Puroguramu.Infrastructures.Data.models;

public class Lesson
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsPublished { get; set; }
    public int Order { get; set; }
    public List<Exo> Exercises { get; set; } = new List<Exo>();
}
