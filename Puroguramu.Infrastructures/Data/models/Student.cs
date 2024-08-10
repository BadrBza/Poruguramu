using Microsoft.AspNetCore.Identity;

namespace Puroguramu.Infrastructures.Data.models;

public class Student : IdentityUser
{
    public string Matricule { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string LabGroup { get; set; }
    public byte[]? ProfilePicture { get; set; }
    public List<StudentExercise> StudentExercises { get; set; }
    public List<Tentative> Tentatives { get; set; } = new List<Tentative>();
}
