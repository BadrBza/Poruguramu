using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Puroguramu.Infrastructures.Data;
using Puroguramu.Infrastructures.Data.models;

public static class LessonSeeder
{
    public static async Task SeedLessons(ApplicationDbContext context)
    {
        if (!await context.Lessons.AnyAsync())
        {
            var lessons = new List<Lesson>
            {
                new Lesson
                {
                    Id = Guid.NewGuid(),
                    Title = "Introduction to Programming",
                    Description = "This lesson covers the basics of programming.",
                    IsPublished = true,
                    Order = 1,
                    Exercises = new List<Exo>()
                },
                new Lesson
                {
                    Id = Guid.NewGuid(),
                    Title = "Object-Oriented Programming",
                    Description = "This lesson introduces the concepts of object-oriented programming.",
                    IsPublished = true,
                    Order = 2,
                    Exercises = new List<Exo>()
                },
                new Lesson
                {
                    Id = Guid.NewGuid(),
                    Title = "Data Structures and Algorithms",
                    Description = "Learn about various data structures and algorithms.",
                    IsPublished = true,
                    Order = 3,
                    Exercises = new List<Exo>()
                }
            };

            context.Lessons.AddRange(lessons);
            await context.SaveChangesAsync();
        }
    }
}
