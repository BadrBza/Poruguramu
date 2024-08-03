using Puroguramu.Domains.modelsDomains;

namespace Puroguramu.Domains.Repositories;

public interface IExoRepository
{
    Task<bool> ExerciseTitleExistsAsync(Guid lessonId, string title);
    Task CreateExerciseAsync(Guid lessonId, string title);
    Task<int> GetExercicesCountAsync();
    Task<ExerciseEditDto> GetExerciseByIdAsync(Guid id);
    Task UpdateExerciseAsync(ExerciseEditDto exerciseDto);
}
