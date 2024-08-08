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
                    Solution = @"public class Exercice
{
    public static float Power(float b, int e)
    {
        return (float)Math.Pow(b, e);
    }
}"
                },
                new Exo
                {
                    Id = Guid.NewGuid(),
                    Title = "Math Power C#",
                    Description = "Créez une fonction Power C# prenant en paramètre une base b de type float et un exposant e de type int.",
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
                    Solution = @"public class Exercice
                                {
                                    public static float Power(float b, int e)
                                    {
                                        return (float)Math.Pow(b, e);
                                    }
                                }"
                },
                new Exo
                {
                    Id = Guid.NewGuid(),
                    Title = "FizzBuzz Exercise",
                    Description = "Implémentez la fonction FizzBuzz.",
                    Difficulty = Difficulty.Easy,
                    IsPublished = true,
                    Order = 3,
                    LessonId = lessons[2].Id,
                    Template = @"// code-insertion-point

public class Test
{
    public static TestResult Ensure(int number, string expected)
    {
        TestStatus status = TestStatus.Passed;
        string actual = string.Empty;
        try
        {
            actual = Exercice.FizzBuzz(number);
            if (actual != expected)
            {
                status = TestStatus.Failed;
            }
        }
        catch (Exception ex)
        {
            status = TestStatus.Inconclusive;
        }

        return new TestResult(
            string.Format(""FizzBuzz of {0} should be '{1}'"", number, expected),
            status,
            status == TestStatus.Passed ? string.Empty : string.Format(""Expected '{0}'. Got '{1}'."", expected, actual)
        );
    }
}

return new TestResult[] {
    Test.Ensure(1, ""1""),
    Test.Ensure(2, ""2""),
    Test.Ensure(3, ""Fizz""),
    Test.Ensure(4, ""4""),
    Test.Ensure(5, ""Buzz""),
    Test.Ensure(6, ""Fizz""),
    Test.Ensure(10, ""Buzz""),
    Test.Ensure(15, ""FizzBuzz""),
    Test.Ensure(30, ""FizzBuzz""),
    Test.Ensure(16, ""16"")
};",
                    Stub = @"public class Exercice
{
    public static string FizzBuzz(int number)
    {
        // Tapez votre code ici
        return string.Empty;
    }
}",
                    Solution = @"public class Exercice
{
    public static string FizzBuzz(int number)
    {
        if (number % 3 == 0 && number % 5 == 0)
            return ""FizzBuzz"";
        if (number % 3 == 0)
            return ""Fizz"";
        if (number % 5 == 0)
            return ""Buzz"";
        return number.ToString();
    }
}"
                },
            };

            context.Exercises.AddRange(exercises);
            await context.SaveChangesAsync();
        }
    }
}
