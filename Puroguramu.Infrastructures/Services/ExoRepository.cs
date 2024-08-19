using Microsoft.EntityFrameworkCore;
using Puroguramu.Domains;
using Puroguramu.Domains.modelsDomains;
using Puroguramu.Domains.Repositories;
using Puroguramu.Infrastructures.Data;
using Puroguramu.Infrastructures.Data.models;

namespace Puroguramu.Infrastructures.Services;

public class ExoRepository : IExoRepository
{

    private readonly ApplicationDbContext _context;

    public ExoRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<bool> ExerciseTitleExistsAsync(Guid lessonId, string title)
    {
        return await _context.Exercises.AnyAsync(e => e.LessonId == lessonId && e.Title == title);
    }

    public async Task CreateExerciseAsync(Guid lessonId, string title)
    {
        // Vérifier si la leçon existe avant de créer un exercice
        var lesson = await _context.Lessons.FirstOrDefaultAsync(l => l.Id == lessonId);
        if (lesson == null)
        {
            throw new InvalidOperationException("La leçon spécifiée n'existe pas.");
        }

        var maxOrder = await _context.Exercises
            .Where(e => e.LessonId == lessonId)
            .MaxAsync(e => (int?)e.Order) ?? 0;

        var exercise = new Exo
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = string.Empty,
            Difficulty = Difficulty.Easy,
            IsPublished = false,
            Order = maxOrder + 1,
            LessonId = lessonId,
            Template = "public class Exercice { }", // Template par défaut
            Stub = "",
            Solution = string.Empty
        };

        _context.Exercises.Add(exercise);
        await _context.SaveChangesAsync();
    }

    public async Task<int> GetExercicesCountAsync()
    {
        return await _context.Exercises.CountAsync();
    }

    public async Task<ExerciseEditDto> GetExerciseByIdAsync(Guid id)
    {
        var exercise = await _context.Exercises.FindAsync(id);
        if (exercise == null) return null;

        return new ExerciseEditDto
        {
            Id = exercise.Id,
            Title = exercise.Title,
            Description = exercise.Description,
            Template = exercise.Template,
            Solution = exercise.Solution,
            Stub = exercise.Stub,
            LessonId = exercise.LessonId,
            Difficulty = (DifficultyExo)exercise.Difficulty
        };
    }

    public async Task UpdateExerciseAsync(ExerciseEditDto exerciseDto)
    {
        var exercise = await _context.Exercises.FindAsync(exerciseDto.Id);
        if (exercise != null)
        {
            exercise.Title = exerciseDto.Title;
            exercise.Description = exerciseDto.Description;
            exercise.Template = exerciseDto.Template;
            exercise.Stub = exerciseDto.Stub;
            exercise.Solution = exerciseDto.Solution;
            exercise.Difficulty = MapDtoToDifficulty(exerciseDto.Difficulty);

            await _context.SaveChangesAsync();
        }
    }

    private Puroguramu.Infrastructures.Data.models.Difficulty MapDtoToDifficulty(Domains.DifficultyExo difficultyExo)
    {
        return difficultyExo switch
        {
            Domains.DifficultyExo.Easy => Puroguramu.Infrastructures.Data.models.Difficulty.Easy,
            Domains.DifficultyExo.Medium => Puroguramu.Infrastructures.Data.models.Difficulty.Medium,
            Domains.DifficultyExo.Hard => Puroguramu.Infrastructures.Data.models.Difficulty.Hard,
            _ => throw new ArgumentOutOfRangeException(nameof(difficultyExo), "Unknown difficulty level")
        };
    }

    public async Task MoveExerciseAsync(Guid exerciseId, bool moveUp)
    {
        var exercise = await _context.Exercises.FindAsync(exerciseId);
        if (exercise != null)
        {
            var targetOrder = moveUp ? exercise.Order - 1 : exercise.Order + 1;
            var swapExercise = await _context.Exercises.FirstOrDefaultAsync(e => e.LessonId == exercise.LessonId && e.Order == targetOrder);
            if (swapExercise != null)
            {
                swapExercise.Order = exercise.Order;
                exercise.Order = targetOrder;
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task ToggleExerciseAsync(Guid id)
    {
        var exercise = await _context.Exercises.FindAsync(id);
        if (exercise != null)
        {
            exercise.IsPublished = !exercise.IsPublished;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteExerciseAsync(Guid id)
    {
        var exercise = await _context.Exercises.FindAsync(id);
        if (exercise != null)
        {
            _context.Exercises.Remove(exercise);
            await _context.SaveChangesAsync();
        }
    }



}
