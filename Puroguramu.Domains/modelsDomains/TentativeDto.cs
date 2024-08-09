namespace Puroguramu.Domains.modelsDomains;

public class TentativeDto
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public DateTime AttemptedOn { get; set; }
    public ExerciseStatuts Status { get; set; }
}
