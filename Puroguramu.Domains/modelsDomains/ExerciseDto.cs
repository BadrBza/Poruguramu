namespace Puroguramu.Domains.modelsDomains;

public class ExerciseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsPublished { get; set; }
    public int Order { get; set; }
}
