namespace Puroguramu.Domains;

public interface IExercisesRepository
{
    Exercise GetExercise(Guid exerciseId);

    Task<string> GetStudentProposalAsync(Guid exerciseId, string studentId);

    Task SaveStudentProposalAsync(Guid exerciseId, string studentId, string proposal);

    Task UpdateStudentExerciseAbandonnedStatusAsync(Guid exerciseId, string studentId);

}
