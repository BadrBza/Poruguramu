using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Puroguramu.Infrastructures.Data;
using Puroguramu.Infrastructures.Data.models;

public static class ExerciseSeeder
{
    public static async Task SeedExercises(ApplicationDbContext context)
    {
        if (!await context.Exercises.AnyAsync())
        {
            var lessons = await context.Lessons.ToListAsync();

            var exercises = new List<Exo>
            {
                new Exo
                {
                    Id = Guid.NewGuid(),
                    Title = "Introduction to C#",
                    Description = "Learn the basics of C# programming.",
                    Difficulty = Difficulty.Easy,
                    IsPublished = true,
                    Order = 1,
                    LessonId = lessons[0].Id,
                    Template = "public class Exercise { }",
                    Solution = "public class Exercise { /* Solution goes here */ }"
                },
                new Exo
                {
                    Id = Guid.NewGuid(),
                    Title = "Advanced C# Features",
                    Description = "Explore advanced features of C#.",
                    Difficulty = Difficulty.Hard,
                    IsPublished = true,
                    Order = 2,
                    LessonId = lessons[1].Id,
                    Template = "public class Exercise { }",
                    Solution = "public class Exercise { /* Solution goes here */ }"
                },
                new Exo
                {
                    Id = Guid.NewGuid(),
                    Title = "LINQ in C#",
                    Description = "Learn how to use LINQ in C#.",
                    Difficulty = Difficulty.Medium,
                    IsPublished = true,
                    Order = 3,
                    LessonId = lessons[2].Id,
                    Template = "public class Exercise { }",
                    Solution = "public class Exercise { /* Solution goes here */ }"
                },
                new Exo
                {
                    Id = Guid.NewGuid(),
                    Title = "if-else in C#",
                    Description = "Learn how to use if-else in C#.",
                    Difficulty = Difficulty.Easy,
                    IsPublished = true,
                    Order = 4,
                    LessonId = lessons[2].Id,
                    Template = "public class Exercise { }",
                    Solution = "public class Exercise { /* Solution goes here */ }"
                },
                new Exo
                {
                    Id = Guid.NewGuid(),
                    Title = "While in C#",
                    Description = "Learn how to use while in C#.",
                    Difficulty = Difficulty.Medium,
                    IsPublished = true,
                    Order = 5,
                    LessonId = lessons[2].Id,
                    Template = "public class Exercise { }",
                    Solution = "public class Exercise { /* Solution goes here */ }"
                },
            };

            context.Exercises.AddRange(exercises);
            await context.SaveChangesAsync();
        }
    }
}
