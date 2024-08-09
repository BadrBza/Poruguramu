using Puroguramu.Domains;

namespace Puroguramu.Infrastructures.Data.models;

public class Tentative
{
    public Guid Id { get; set; }
    public Guid ExoId { get; set; }
    public Exo Exo { get; set; }
    public string StudentId { get; set; }
    public Student Student { get; set; }
    public string Code { get; set; }
    public DateTime AttemptedOn { get; set; }
    public ExerciseStatuts Status { get; set; }
}
