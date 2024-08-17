using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Puroguramu.Domains;
using Puroguramu.Domains.Repositories;
using Puroguramu.Domains.modelsDomains;
using Puroguramu.Infrastructures.Data;
using Puroguramu.Infrastructures.Data.models;
using ExerciseStatus = Puroguramu.Infrastructures.Data.models.ExerciseStatus;

namespace Puroguramu.Infrastructures.Services
{
    public class LessonRepository : ILessonRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Student> _userManager;

        public LessonRepository(ApplicationDbContext context, UserManager<Student> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<int> GetLessonsCountAsync()
        {
            return await _context.Lessons.CountAsync();
        }

        public async Task CreateLessonAsync(LessonDto lessonDto)
        {
            if (await LessonTitleExistsAsync(lessonDto.Title))
            {
                throw new InvalidOperationException("Une leçon avec ce titre existe déjà.");
            }

            var lesson = new Lesson
            {
                Id = Guid.NewGuid(),
                Title = lessonDto.Title,
                Description = string.Empty,
                IsPublished = false,
                Order = await GetLessonsCountAsync(),
                Exercises = new List<Exo>()
            };

            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> LessonTitleExistsAsync(string title)
        {
            return await _context.Lessons.AnyAsync(lesson => lesson.Title == title);
        }

        public async Task<List<LessonDto>> GetAllLessonsAsync()
        {
            var lessons = await _context.Lessons
                .OrderBy(lesson => lesson.Order)
                .Include(lesson => lesson.Exercises)
                .ToListAsync();

            return lessons
                .Where(lesson => lesson.Exercises.Any(e => e.IsPublished)) // Filtre les leçons sans exercices publiés
                .Select(lesson => new LessonDto
                {
                    Id = lesson.Id,
                    Title = lesson.Title,
                    IsPublished = lesson.IsPublished,
                    Order = lesson.Order,
                    TotalExercises = lesson.Exercises.Count(e => e.IsPublished),
                }).ToList();
        }

        public async Task ToggleLessonAsync(Guid id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson != null)
            {
                lesson.IsPublished = !lesson.IsPublished;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteLessonAsync(Guid id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson != null)
            {
                _context.Lessons.Remove(lesson);
                await _context.SaveChangesAsync();
            }
        }

        public async Task MoveLessonAsync(Guid id, bool moveUp)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson != null)
            {
                var targetOrder = moveUp ? lesson.Order - 1 : lesson.Order + 1;
                var swapLesson = await _context.Lessons.FirstOrDefaultAsync(l => l.Order == targetOrder);
                if (swapLesson != null)
                {
                    swapLesson.Order = lesson.Order;
                    lesson.Order = targetOrder;
                    await _context.SaveChangesAsync();
                }
            }
        }


        public async Task<LessonEditDto> GetLessonByIdAsync(Guid id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson != null)
            {
                return new LessonEditDto { Id = lesson.Id, Title = lesson.Title, Description = lesson.Description };
            }

            return null;
        }

        public async Task UpdateLessonAsync(LessonEditDto lessonDto)
        {
            var lesson = await _context.Lessons.FindAsync(lessonDto.Id);
            if (lesson != null)
            {
                Console.WriteLine($"Updating lesson with ID: {lessonDto.Id}");
                Console.WriteLine($"New Title: {lessonDto.Title}");
                Console.WriteLine($"New Description: {lessonDto.Description}");

                lesson.Title = lessonDto.Title;
                lesson.Description = lessonDto.Description;

                Console.WriteLine($"Lesson after update - Title: {lesson.Title}, Description: {lesson.Description}");

                await _context.SaveChangesAsync();
            }

        }

        public async Task<List<LessonDto>> GetPublishedLessonsAsync()
        {
            var lessons = await _context.Lessons
                .Where(lesson => lesson.IsPublished)
                .OrderBy(lesson => lesson.Order)
                .Include(lesson => lesson.Exercises)
                .ToListAsync();

            return lessons
                .Where(lesson => lesson.Exercises.Any(e => e.IsPublished)) // Filtre les leçons sans exercices publiés
                .Select(lesson => new LessonDto
                {
                    Id = lesson.Id,
                    Title = lesson.Title,
                    IsPublished = lesson.IsPublished,
                    Order = lesson.Order,
                    TotalExercises = lesson.Exercises.Count(e => e.IsPublished),
                }).ToList();
        }

        public async Task<List<LessonDto>> GetPublishedLessonsWithProgressAsync(string studentId)
        {
            var lessons = await _context.Lessons
                .Where(lesson => lesson.IsPublished)
                .OrderBy(lesson => lesson.Order)
                .Include(lesson => lesson.Exercises)
                .ThenInclude(exo => exo.StudentExercises)
                .ToListAsync();

            return lessons
                .Where(lesson => lesson.Exercises.Any(e => e.IsPublished))
                .Select(lesson => new LessonDto
            {
                Id = lesson.Id,
                Title = lesson.Title,
                IsPublished = lesson.IsPublished,
                Order = lesson.Order,
                CompletedExercises = lesson.Exercises
                    .Where(e => e.IsPublished)
                    .Count(e => e.StudentExercises
                        .Any(se => se.StudentId == studentId && se.Status == ExerciseStatus.Passed)),
                TotalExercises = lesson.Exercises.Count(e => e.IsPublished),
            }).ToList();
        }

        public async Task<int> GetTotalExercisesCountAsync(Guid lessonId)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Exercises)
                .FirstOrDefaultAsync(l => l.Id == lessonId);

            if (lesson != null)
            {
                return lesson.Exercises.Count;
            }

            return 0; // Retourne 0 si la leçon n'existe pas
        }

        // Méthode pour obtenir le nombre d'exercices terminés par leçon
        public async Task<int> GetCompletedExercisesCountAsync(Guid lessonId, string studentId)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Exercises)
                .ThenInclude(e => e.StudentExercises)
                .FirstOrDefaultAsync(l => l.Id == lessonId);

            if (lesson != null)
            {
                return lesson.Exercises
                    .Count(e => e.StudentExercises
                        .Any(se => se.Status == ExerciseStatus.Passed && se.StudentId == studentId));
            }

            return 0;
        }

        public async Task<List<LessonDto>> GetLessonsWithStudentProgressAsync()
        {
            // Récupérer les IDs des étudiants ayant le rôle "Student"
            var studentRole = await _context.Roles.SingleAsync(r => r.Name == "student");
            var studentIds = await _context.UserRoles
                .Where(ur => ur.RoleId == studentRole.Id)
                .Select(ur => ur.UserId)
                .ToListAsync();

            // Filtrer les leçons et leurs exercices
            var lessons = await _context.Lessons
                .Include(lesson => lesson.Exercises)
                .ThenInclude(exercise => exercise.StudentExercises)
                .ToListAsync();

            // Nombre total d'étudiants
            var totalStudents = studentIds.Count;

            return lessons.Select(lesson => new LessonDto
            {
                Id = lesson.Id,
                Title = lesson.Title,
                IsPublished = lesson.IsPublished,
                Order = lesson.Order,
                TotalExercises = lesson.Exercises.Count,
                CompletedExercises = lesson.Exercises.Sum(exercise =>
                    exercise.StudentExercises.Count(se => se.Status == ExerciseStatus.Passed && studentIds.Contains(se.StudentId))),
                TotalStudents = totalStudents,
                StudentsWhoCompleted = lesson.Exercises.Any()
                    ? lesson.Exercises
                        .SelectMany(e => e.StudentExercises)
                        .Where(se => se.Status == ExerciseStatus.Passed && studentIds.Contains(se.StudentId))
                        .GroupBy(se => se.StudentId)
                        .Count()
                    : 0
            }).ToList();
        }

        public async Task<List<ExerciseDto>> GetExercisesByLessonIdAsync(Guid lessonId)
        {
            var exercises = await _context.Exercises
                .Where(e => e.LessonId == lessonId)
                .Select(e => new ExerciseDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    IsPublished = e.IsPublished,
                    Order = e.Order
                })
                .ToListAsync();

            return exercises;
        }


        public async Task<List<StudentExerciseDto>> GetAllExercisesByLessonAsync(string studentID, Guid lessonId)
        {
            // Récupération de l'étudiant par son matricule
            var student = await _userManager.Users
                .Include(u => u.StudentExercises)
                .ThenInclude(se => se.Exo)
                .SingleOrDefaultAsync(u => u.Id == studentID);

            if (student == null)
            {
                throw new InvalidOperationException("Student not found.");
            }

            if (student.StudentExercises == null)
            {
                student.StudentExercises = new List<StudentExercise>();
            }

            // Récupérer tous les exercices de la leçon donnée
            var exercises = await _context.Exercises
                .Where(e => e.LessonId == lessonId && e.IsPublished)
                .OrderBy(e => e.Order)
                .ToListAsync();

            // Construire la liste des DTOs
            var studentExercises = exercises.Select(exercise =>
            {
                var studentExercise = student.StudentExercises
                    .FirstOrDefault(se => se.ExoId == exercise.Id);

                return new StudentExerciseDto
                {
                    ExerciseId = exercise.Id,
                    Title = exercise.Title,
                    Statuts = studentExercise != null ? (Domains.ExerciseStatuts)studentExercise.Status : Domains.ExerciseStatuts.NotStarted,
                    DifficultyExo = MapDifficultyToDto(exercise.Difficulty),
                };
            }).ToList();

            return studentExercises;
        }

        private Domains.DifficultyExo MapDifficultyToDto(Puroguramu.Infrastructures.Data.models.Difficulty difficulty)
        {
            return difficulty switch
            {
                Puroguramu.Infrastructures.Data.models.Difficulty.Easy => Domains.DifficultyExo.Easy,
                Puroguramu.Infrastructures.Data.models.Difficulty.Medium => Domains.DifficultyExo.Medium,
                Puroguramu.Infrastructures.Data.models.Difficulty.Hard => Domains.DifficultyExo.Hard,
                _ => throw new ArgumentOutOfRangeException(nameof(difficulty), "Unknown difficulty level")
            };
        }

        public async Task ResetLessonAsync(Guid lessonId)
        {
            var exercises = await _context.Exercises.Where(e => e.LessonId == lessonId).ToListAsync();
            foreach (var exercise in exercises)
            {
                await ResetExerciseAsync(exercise.Id);
            }
        }

        public async Task ResetAllLessonsAsync()
        {
            var lessons = await _context.Lessons.ToListAsync();
            foreach (var lesson in lessons)
            {
                await ResetLessonAsync(lesson.Id);
            }
        }

        public async Task ResetExerciseAsync(Guid exerciseId)
        {
            // Supprimer les tentatives liées à l'exercice
            var tentatives = await _context.Tentatives.Where(t => t.ExoId == exerciseId).ToListAsync();
            _context.Tentatives.RemoveRange(tentatives);
            var studentExercise = _context.StudentExercise.Where(se => se.ExoId == exerciseId);
            _context.StudentExercise.RemoveRange(studentExercise);
            // Mettre à jour le statut des exercices des étudiants à NotStarted
            var studentExercises = await _context.StudentExercise.Where(se => se.ExoId == exerciseId).ToListAsync();
            foreach (var se in studentExercises)
            {
                se.Status = ExerciseStatus.NotStarted;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteStudentExerciseDataAsync(Guid exerciseId, string studentId)
        {
            // Rechercher les entrées de StudentExercise liées à cet exercice et à cet étudiant
            var studentExercises = await _context.StudentExercise
                .Where(se => se.ExoId == exerciseId && se.StudentId == studentId)
                .ToListAsync();

            foreach (var studentExercise in studentExercises)
            {
                Console.WriteLine("Données trouvée pour l'id : " + studentExercise.StudentId);
            }
            // Supprimer les entrées trouvées
            if (studentExercises.Any())
            {
                _context.StudentExercise.RemoveRange(studentExercises);
                await _context.SaveChangesAsync();
            }
        }

    }
}
