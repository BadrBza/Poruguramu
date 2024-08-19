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
using Puroguramu.Domains;
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
                string role = studentDto.Matricule.StartsWith("P") ? "teacher" : "student";
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
            var students = await _userManager.GetUsersInRoleAsync("student");
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
                    Statuts = Domains.ExerciseStatuts.NotStarted,
                    DifficultyExo = Domains.DifficultyExo.Easy,
                };
            }
            else
            {
                // Si aucun exercice non complété n'est trouvé, tous les exercices ont été réalisés
                return new StudentExerciseDto
                {
                    ExerciseId = Guid.Empty, // ou tout autre identifiant qui indique qu'il n'y a plus d'exercices
                    Title = "Tout les exercices de cette leçon ont été réalisé.",
                    Statuts = Domains.ExerciseStatuts.Passed,
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
                    if (roles.Contains("student"))
                    {
                        return "~/Dashboard/studentDashboard";
                    }
                    else if (roles.Contains("teacher"))
                    {
                        return "~/Dashboard/TeacherDashboard";
                    }
                }
            }

            return null;
        }


        public async Task<StudentExerciseDto> GetStudentExerciseByIdAsync(string userId, Guid exerciseId)
        {
            // Rechercher l'étudiant par son ID d'utilisateur
            var student = await _userManager.Users
                .Include(u => u.StudentExercises)
                .ThenInclude(se => se.Exo)
                .Include(u => u.Tentatives)
                .SingleOrDefaultAsync(u => u.Id == userId);

            if (student == null)
            {
                throw new InvalidOperationException("Student not found.");
            }

            // Rechercher l'exercice de l'étudiant par l'ID de l'exercice
            var studentExercise = student.StudentExercises
                .SingleOrDefault(se => se.ExoId == exerciseId);

            if (studentExercise == null)
            {
                return null;
            }

            var tentatives = await _context.Tentatives
                .Where(a => a.StudentId == userId && a.ExoId == exerciseId)
                .OrderBy(a => a.AttemptedOn)
                .ToListAsync();

            // Retourner le DTO correspondant à l'exercice de l'étudiant
            return new StudentExerciseDto
            {
                ExerciseId = studentExercise.ExoId,
                Title = studentExercise.Exo.Title,
                Statuts = MapStatus(studentExercise.Status),
                DifficultyExo = MapDifficulty(studentExercise.Exo.Difficulty),
                Tentative = tentatives.Select(a => new TentativeDto()
                {
                    Id = a.Id,
                    Code = a.Code,
                    AttemptedOn = a.AttemptedOn,
                    Status = a.Status,
                }).ToList()
            };
        }

        public async Task UpdateStudentExerciseStatusAsync(StudentExerciseDto studentExercise)
        {
            var existingStudentExercise = await _context.StudentExercise
                .FirstOrDefaultAsync(se => se.ExoId == studentExercise.ExerciseId && se.StudentId == studentExercise.StudentId);

            if (existingStudentExercise != null)
            {
                existingStudentExercise.Status = (ExerciseStatus)studentExercise.Statuts;
                _context.StudentExercise.Update(existingStudentExercise);
            }
            else
            {
                var newStudentExercise = new StudentExercise
                {
                    Id = Guid.NewGuid(),
                    ExoId = studentExercise.ExerciseId,
                    StudentId = studentExercise.StudentId,
                    Status = (ExerciseStatus)studentExercise.Statuts,
                    Code = studentExercise.Code,
                };
                _context.StudentExercise.Add(newStudentExercise);
            }

            await _context.SaveChangesAsync();
        }

        private DifficultyExo MapDifficulty(Difficulty difficulty)
        {
            return difficulty switch
            {
                Difficulty.Easy => DifficultyExo.Easy,
                Difficulty.Medium => DifficultyExo.Medium,
                Difficulty.Hard => DifficultyExo.Hard,
                _ => throw new ArgumentOutOfRangeException(nameof(difficulty), "Unknown difficulty level")
            };
        }

        private ExerciseStatuts MapStatus(ExerciseStatus status)
        {
            return status switch
            {
                ExerciseStatus.NotStarted => ExerciseStatuts.NotStarted,
                ExerciseStatus.Passed => ExerciseStatuts.Passed,
                ExerciseStatus.Failed => ExerciseStatuts.Failed,
                ExerciseStatus.Started => ExerciseStatuts.Started,
                _ => throw new ArgumentOutOfRangeException(nameof(status), "Unknown status")
            };
        }

        public async Task SaveStudentAttemptAsync(Guid exerciseId, string studentId, string code, ExerciseStatuts status)
        {
            var tentative = new Tentative()
            {
                Id = Guid.NewGuid(),
                ExoId = exerciseId,
                StudentId = studentId,
                Code = code,
                AttemptedOn = DateTime.UtcNow,
                Status = status,
            };

            _context.Tentatives.Add(tentative);
            await _context.SaveChangesAsync();
        }

        public async Task<List<TentativeDto>> GetStudentTentativesAsync(Guid exerciseId, string studentId)
        {
            return await _context.Tentatives
                .Where(t => t.ExoId == exerciseId && t.StudentId == studentId)
                .OrderBy(t => t.AttemptedOn)
                .Select(t => new TentativeDto { AttemptedOn = t.AttemptedOn, Status = (ExerciseStatuts)t.Status, Code = t.Code})
                .ToListAsync();
        }


        public async Task AddStudentExerciseAsync(StudentExerciseDto studentExerciseDto)
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.Id == studentExerciseDto.StudentId);

            if (student == null)
            {
                throw new InvalidOperationException("Student not found.");
            }

            var newStudentExercise = new StudentExercise
            {
                Id = Guid.NewGuid(),
                ExoId = studentExerciseDto.ExerciseId,
                StudentId = studentExerciseDto.StudentId,
                Status = (ExerciseStatus)studentExerciseDto.Statuts,
                Code = studentExerciseDto.Code ?? string.Empty,
                StudentMatricule = student.Matricule
            };

            _context.StudentExercise.Add(newStudentExercise);
            await _context.SaveChangesAsync();
        }




    }

}
