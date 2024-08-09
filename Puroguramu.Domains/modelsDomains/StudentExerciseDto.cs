namespace Puroguramu.Domains.modelsDomains;

public class StudentExerciseDto
{
    public Guid ExerciseId { get; set; }
    public string Title { get; set; }
    public ExerciseStatuts Statuts { get; set; }
    public DifficultyExo DifficultyExo { get; set; }
    public string StudentId { get; set; }

    public string StudentMatricule { get; set; }
    public string Code { get; set; }
    public List<TentativeDto> Tentative { get; set; } = new List<TentativeDto>();
}
