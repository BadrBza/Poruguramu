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
                    Stub = @"public class Exercice
{
  // Tapez votre code ici
}",
                    Solution = "public class Exercise { /* Solution goes here */ }"
                },
                new Exo
                {
                    Id = Guid.NewGuid(),
                    Title = "Math Power C#",
                    Description = "Créez une fonction Power C# prenant en paramètre une base b de type float et un exposant e de type int. Power(b, e) retourne le float b",
                    Difficulty = Difficulty.Medium,
                    IsPublished = true,
                    Order = 2,
                    LessonId = lessons[1].Id,
                    Template = @"// code-insertion-point

public class Test
{
    public static TestResult Ensure(float b, int exponent, float expected)
    {
      TestStatus status = TestStatus.Passed;
      float actual = float.NaN;
      try
      {
         actual = Exercice.Power(b, exponent);
         if(Math.Abs(actual - expected) > 0.00001f)
         {
             status = TestStatus.Failed;
         }
      }
      catch(Exception ex)
      {
         status = TestStatus.Inconclusive;
      }

      return new TestResult(
        string.Format(""Power of {0} by {1} should be {2}"", b, exponent, expected),
        status,
        status == TestStatus.Passed ? string.Empty : string.Format(""Expected {0}. Got {1}."", expected, actual)
      );
    }
}

return new TestResult[] {
  Test.Ensure(2, 4, 16.0f),
  Test.Ensure(2, -4, 1.0f/16.0f)
};",
                    Stub = @"public class Exercice
{
  // Tapez votre code ici
}",
                    Solution = "public static float Power(float b, int e)\n{\n    return (float)Math.Pow(b, e);\n}\n"
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
                    Stub = @"public class Exercice
{
  // Tapez votre code ici
}",
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
                    Stub = @"public class Exercice
{
  // Tapez votre code ici
}",
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
                    Template = "public class Exercise { }", Stub = @"public class Exercice
{
  // Tapez votre code ici
}",
                    Solution = "public class Exercise { /* Solution goes here */ }"
                },
            };

            context.Exercises.AddRange(exercises);
            await context.SaveChangesAsync();
        }
    }
}
