using Puroguramu.Domains.modelsDomains;

namespace Puroguramu.Domains.Repositories;

public interface ILessonRepository
{
    Task<int> GetLessonsCountAsync();
    Task CreateLessonAsync(LessonDto lessonDto);
    Task<List<LessonDto>> GetAllLessonsAsync();
    Task ToggleLessonAsync(Guid id);
    Task DeleteLessonAsync(Guid id);
    Task MoveLessonAsync(Guid id, bool moveUp);
    Task<LessonEditDto> GetLessonByIdAsync(Guid id);
    Task UpdateLessonAsync(LessonEditDto lessonDto);
    Task<List<LessonDto>> GetPublishedLessonsAsync();

    Task<List<LessonDto>> GetPublishedLessonsWithProgressAsync(string studentId);
    Task<int> GetTotalExercisesCountAsync(Guid lessonId);
    Task<int> GetCompletedExercisesCountAsync(Guid lessonId, string studentId);
    Task<List<LessonDto>> GetLessonsWithStudentProgressAsync();
    Task<List<ExerciseDto>> GetExercisesByLessonIdAsync(Guid lessonId);

    Task<List<StudentExerciseDto>> GetAllExercisesByLessonAsync(string studentMatricule, Guid lessonId);
    Task ResetLessonAsync(Guid lessonId);
    Task ResetAllLessonsAsync();
    Task ResetExerciseAsync(Guid exerciseId);
    Task DeleteStudentExerciseDataAsync(Guid exerciseId, string studentId);
}


