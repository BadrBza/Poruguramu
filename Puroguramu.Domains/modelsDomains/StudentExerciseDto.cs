namespace Puroguramu.Domains.modelsDomains;

public class StudentExerciseDto
{
    public Guid ExerciseId { get; set; }
    public string Title { get; set; }
    public ExerciseStatus Status { get; set; }
    public DifficultyExo DifficultyExo { get; set; }
}
