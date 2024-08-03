using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Puroguramu.Domains.Repositories;
using Puroguramu.Infrastructures.Data.models;
using Puroguramu.Domains.modelsDomains;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Puroguramu.Infrastructures.Data;

namespace Puroguramu.Infrastructures.Services
{
    public class StudentRepository : IStudentRepository
    {
        private readonly UserManager<Student> _userManager;
        private readonly SignInManager<Student> _signInManager;

        private readonly ApplicationDbContext _context;

        private readonly ILogger<StudentRepository> _logger;

        public StudentRepository(UserManager<Student> userManager, SignInManager<Student> signInManager, ApplicationDbContext context, ILogger<StudentRepository> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
        }



        public async Task<bool> IsEmailInUseAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }




        public async Task<bool> IsMatriculeInUseAsync(string matricule)
        {
            return await _userManager.Users.AnyAsync(u => u.Matricule == matricule);
        }




        public async Task<string> RegisterStudentAsync(StudentDto studentDto, string password)
        {
            var student = new Student
            {
                UserName = studentDto.Email,
                Email = studentDto.Email,
                Matricule = studentDto.Matricule,
                FirstName = studentDto.FirstName,
                LastName = studentDto.LastName,
                LabGroup = studentDto.LabGroup,
                ProfilePicture = studentDto.ProfilePicture
            };

            var result = await _userManager.CreateAsync(student, password);
            if (result.Succeeded)
            {
                string role = studentDto.Matricule.StartsWith("P") ? "Teacher" : "Student";
                await _userManager.AddToRoleAsync(student, role);
                return role;
            }

            return null;
        }





        public async Task<string> GetUserIdByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user?.Id;
        }





        public async Task<IList<string>> GetExternalAuthenticationSchemesAsync()
        {
            var schemes = await _signInManager.GetExternalAuthenticationSchemesAsync();
            return schemes.Select(s => s.Name).ToList();
        }




        public async Task SignInAsync(string userId, bool isPersistent)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }
            await _signInManager.SignInAsync(user, isPersistent);
        }





        public async Task<SignInResult> AuthenticateAsync(string matricule, string password)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Matricule == matricule);
            if (user == null)
            {
                return SignInResult.Failed;
            }

            var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);
            return result;
        }





        public async Task<string> GetUserRoleAsync(string matricule)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Matricule == matricule);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                return roles.FirstOrDefault();
            }

            return null;
        }





        public async Task<int> GetStudentsCountAsync()
        {
            var students = await _userManager.GetUsersInRoleAsync("Student");
            return students.Count;
        }




        public async Task<StudentDto> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            return new StudentDto
            {
                Id = user.Id,
                Matricule = user.Matricule,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                LabGroup = user.LabGroup,
                ProfilePicture = user.ProfilePicture
            };
        }






        public async Task<StudentDto> GetStudentProfileAsync(ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return null;

            return await GetUserByIdAsync(userId);
        }






        public async Task<bool> UpdateStudentProfileAsync(ClaimsPrincipal user, StudentDto studentDto, IFormFile profilePictureFile)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return false;

            var existingStudent = await _userManager.FindByIdAsync(userId);
            if (existingStudent == null)
                return false;

            existingStudent.FirstName = studentDto.FirstName;
            existingStudent.LastName = studentDto.LastName;
            existingStudent.Email = studentDto.Email;
            existingStudent.UserName = studentDto.Email;
            existingStudent.NormalizedEmail = studentDto.Email.ToUpper();
            existingStudent.LabGroup = studentDto.LabGroup;

            if (profilePictureFile != null)
            {
                using var stream = new MemoryStream();
                await profilePictureFile.CopyToAsync(stream);
                existingStudent.ProfilePicture = stream.ToArray();
            }

            var result = await _userManager.UpdateAsync(existingStudent);
            return result.Succeeded;
        }

        public async Task<bool> ChangePasswordAsync(ClaimsPrincipal user, string currentPassword, string newPassword)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return false;

            var existingStudent = await _userManager.FindByIdAsync(userId);
            if (existingStudent == null)
                return false;

            var changePasswordResult = await _userManager.ChangePasswordAsync(existingStudent, currentPassword, newPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    _logger.LogError(error.Description);
                }
                return false;
            }

            return true;
        }

        public async Task<StudentExerciseDto> GetNextExerciseAsync(string studentMatricule, Guid lessonId)
        {
            // Récupération de l'étudiant par son matricule
            var student = await _userManager.Users
                .Include(u => u.StudentExercises)
                .ThenInclude(se => se.Exo)
                .SingleOrDefaultAsync(u => u.Matricule == studentMatricule);

            if (student == null)
            {
                throw new InvalidOperationException("Student not found.");
            }

            if (student.StudentExercises == null)
            {
                student.StudentExercises = new List<StudentExercise>();
            }

            // Obtenir les IDs des exercices déjà complétés dans la leçon donnée
            var completedExercises = student.StudentExercises
                .Where(se => se.Status == ExerciseStatus.Passed && se.Exo.LessonId == lessonId)
                .Select(se => se.ExoId)
                .ToList();

            // Recherche du premier exercice non complété dans la leçon spécifiée
            var nextExercise = await _context.Exercises
                .Where(e => e.LessonId == lessonId && !completedExercises.Contains(e.Id))
                .OrderBy(e => e.Order)
                .FirstOrDefaultAsync();

            if (nextExercise != null)
            {
                return new StudentExerciseDto
                {
                    ExerciseId = nextExercise.Id,
                    Title = nextExercise.Title,
                    Status = Domains.ExerciseStatus.NotStarted,
                };
            }
            else
            {
                // Si aucun exercice non complété n'est trouvé, tous les exercices ont été réalisés
                return new StudentExerciseDto
                {
                    ExerciseId = Guid.Empty, // ou tout autre identifiant qui indique qu'il n'y a plus d'exercices
                    Title = "Tout les exercices de cette leçon ont été réalisé.",
                    Status = Domains.ExerciseStatus.Passed,
                };
            }
        }

        public async Task<string> GetDashboardRedirectUrlAsync(ClaimsPrincipal userPrincipal)
        {
            if (userPrincipal.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(userPrincipal);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Student"))
                    {
                        return "/Dashboard/StudentDashboard";
                    }
                    else if (roles.Contains("Teacher"))
                    {
                        return "/Dashboard/TeacherDashboard";
                    }
                }
            }

            return null; // Ne redirige pas, retourne null
        }

    }

}
