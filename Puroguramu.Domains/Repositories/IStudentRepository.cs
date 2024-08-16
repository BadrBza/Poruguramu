using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Puroguramu.Domains.modelsDomains;

namespace Puroguramu.Domains.Repositories
{
    public interface IStudentRepository
    {
        Task<bool> IsEmailInUseAsync(string email);
        Task<bool> IsMatriculeInUseAsync(string matricule);
        Task<string> RegisterStudentAsync(StudentDto studentDto, string password);
        Task<string> GetUserIdByEmailAsync(string email);
        Task<IList<string>> GetExternalAuthenticationSchemesAsync();
        Task SignInAsync(string userId, bool isPersistent);
        Task<SignInResult> AuthenticateAsync(string matricule, string password);
        Task<string> GetUserRoleAsync(string matricule);
        Task<int> GetStudentsCountAsync();
        Task<StudentDto> GetUserByIdAsync(string userId);

        Task<string> GetDashboardRedirectUrlAsync(ClaimsPrincipal userPrincipal);
        Task<StudentDto> GetStudentProfileAsync(ClaimsPrincipal user);
        Task<bool> UpdateStudentProfileAsync(ClaimsPrincipal user, StudentDto studentDto, IFormFile profilePictureFile);

        Task<bool> ChangePasswordAsync(ClaimsPrincipal user, string currentPassword, string newPassword);
        Task<StudentExerciseDto> GetNextExerciseAsync(string studentMatricule, Guid lessonId);
        Task<StudentExerciseDto> GetStudentExerciseByIdAsync(string userId, Guid exerciseId);
        Task UpdateStudentExerciseStatusAsync(StudentExerciseDto studentExercise);
        Task SaveStudentAttemptAsync(Guid exerciseId, string studentId, string code, ExerciseStatuts status);
        Task<List<TentativeDto>> GetStudentTentativesAsync(Guid exerciseId, string studentId);

        Task AddStudentExerciseAsync(StudentExerciseDto studentExerciseDto);
    }
}
