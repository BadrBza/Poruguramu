namespace Puroguramu.Domains.Repositories;

public interface IExoRepository
{
    Task<bool> ExerciseTitleExistsAsync(Guid lessonId, string title);
    Task CreateExerciseAsync(Guid lessonId, string title);
    Task<int> GetExercicesCountAsync();
}
