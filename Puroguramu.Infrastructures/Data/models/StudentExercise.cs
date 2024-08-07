using Microsoft.Build.Framework;

namespace Puroguramu.Infrastructures.Data.models;


public class StudentExercise
{
    public Guid Id { get; set; }
    public Guid ExoId { get; set; }
    public Exo? Exo { get; set; }
    public ExerciseStatus Status { get; set; }
    public string Code { get; set; }
    public string StudentMatricule { get; set; }
    public string StudentId { get; set; }
    public Student Student { get; set; }
}
