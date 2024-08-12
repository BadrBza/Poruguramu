using Microsoft.EntityFrameworkCore;
using Puroguramu.Infrastructures.Data.models;
namespace Puroguramu.Infrastructures.Data.Config;
public class StudentExerciseSeeder
{
    public static async Task SeedStudentExercises(ApplicationDbContext context)
    {
        if (!await context.StudentExercise.AnyAsync())
        {
            var students = await context.Students.ToListAsync();
            var exercises = await context.Exercises.ToListAsync();

            if (!students.Any() || !exercises.Any())
            {
                throw new InvalidOperationException("Students or Exercises are missing in the database.");
            }

            var studentExercises = new List<StudentExercise>
            {
                new StudentExercise
                {
                    Id = Guid.NewGuid(),
                    StudentMatricule = students[1].Matricule,
                    StudentId = students[1].Id,
                    ExoId = exercises[0].Id,
                    Status = ExerciseStatus.Passed,
                    Code = "public class Solution { /* Completed code here */ }"
                },
                new StudentExercise
                {
                    Id = Guid.NewGuid(),
                    StudentMatricule = students[1].Matricule,
                    StudentId = students[1].Id,
                    ExoId = exercises[1].Id,
                    Status = ExerciseStatus.Passed,
                    Code = "public class Solution { /* Completed code here */ }"
                },
                new StudentExercise
                {
                    Id = Guid.NewGuid(),
                    StudentMatricule = students[1].Matricule,
                    StudentId = students[1].Id,
                    ExoId = exercises[2].Id,
                    Status = ExerciseStatus.Started,
                    Code = "public class Solution { /* Completed code here */ }"
                },

            };

            context.StudentExercise.AddRange(studentExercises);
            await context.SaveChangesAsync();
        }
    }
}
