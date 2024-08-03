namespace Puroguramu.Domains.modelsDomains;

public class StudentDto
{
    public string Id { get; set; }
    public string Matricule { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string LabGroup { get; set; }
    public byte[] ProfilePicture { get; set; }

    public List<StudentExerciseDto> CompletedExercises { get; set; } = new List<StudentExerciseDto>();
    public StudentExerciseDto CurrentExercise { get; set; } // Exercice en cours
}
