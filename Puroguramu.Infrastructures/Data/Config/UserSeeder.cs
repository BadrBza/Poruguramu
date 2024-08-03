using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Puroguramu.Infrastructures.Data.models;

namespace Puroguramu.Infrastructures.Data.Config;

public class UserSeeder
{
    public static async Task SeedUsers(ApplicationDbContext context, UserManager<Student> userManager)
    {
        if (!await context.Students.AnyAsync())
        {
            var students = new List<Student>
            {
                new Student
                {
                    UserName = "jean.jadot@example.com", // Email as username
                    Email = "jean.jadot@example.com",
                    Matricule = "P123456",
                    FirstName = "Jean",
                    LastName = "Jadot",
                    LabGroup = "2i1",
                },
                new Student
                {
                    UserName = "ruben.yildiz@example.com",
                    Email = "ruben.yildiz@example.com",
                    Matricule = "E200382",
                    FirstName = "Ruben",
                    LastName = "Yildiz",
                    LabGroup = "2i1",
                },
                new Student
                {
                    UserName = "badr.bouzia@example.com",
                    Email = "badr.bouzia@example.com",
                    Matricule = "Q123456",
                    FirstName = "Badr",
                    LastName = "Bouzia",
                    LabGroup = "2i1",
                },
            };

            foreach (var student in students)
            {
                // Vérifiez si l'utilisateur existe déjà
                if (await userManager.FindByEmailAsync(student.Email) == null)
                {
                    // Créez l'utilisateur avec un mot de passe par défaut
                    var result = await userManager.CreateAsync(student, "kjjpLLa6r3wmfCO@");

                    if (result.Succeeded)
                    {
                        // Déterminer le rôle en fonction du matricule
                        string role = student.Matricule.StartsWith("P") ? "Teacher" : "Student";

                        // Ajoutez l'utilisateur au rôle approprié
                        await userManager.AddToRoleAsync(student, role);
                    }
                    else
                    {
                        // Gérer les erreurs si la création échoue
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine($"Error creating user {student.Email}: {error.Description}");
                        }
                    }
                }
            }
        }
    }
}
