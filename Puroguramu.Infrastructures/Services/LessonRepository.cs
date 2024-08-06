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
            return await _context.Lessons
                .OrderBy(lesson => lesson.Order)
                .Select(lesson => new LessonDto { Id = lesson.Id, Title = lesson.Title, IsPublished = lesson.IsPublished, Order = lesson.Order })
                .ToListAsync();
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
            return await _context.Lessons
                .Where(lesson => lesson.IsPublished)
                .OrderBy(lesson => lesson.Order)
                .Select(lesson => new LessonDto { Id = lesson.Id, Title = lesson.Title, IsPublished = lesson.IsPublished, Order = lesson.Order })
                .ToListAsync();
        }

        public async Task<List<LessonDto>> GetPublishedLessonsWithProgressAsync(string studentMatricule)
        {
            var lessons = await _context.Lessons
                .Where(lesson => lesson.IsPublished)
                .OrderBy(lesson => lesson.Order)
                .Include(lesson => lesson.Exercises)
                .ThenInclude(exo => exo.StudentExercises)
                .ToListAsync();

            return lessons.Select(lesson => new LessonDto
            {
                Id = lesson.Id,
                Title = lesson.Title,
                IsPublished = lesson.IsPublished,
                Order = lesson.Order,
                CompletedExercises = lesson.Exercises
                    .Count(e => e.StudentExercises
                        .Any(se => se.Student != null && se.Student.Matricule == studentMatricule && se.Status == ExerciseStatus.Passed)),
                TotalExercises = lesson.Exercises.Count
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
        public async Task<int> GetCompletedExercisesCountAsync(Guid lessonId, string studentMatricule)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Exercises)
                .ThenInclude(e => e.StudentExercises)
                .FirstOrDefaultAsync(l => l.Id == lessonId);

            if (lesson != null)
            {
                Console.WriteLine($"Lesson {lesson.Id} - {lesson.Title} loaded with {lesson.Exercises.Count} exercises.");
                foreach (var exercise in lesson.Exercises)
                {
                    Console.WriteLine($"Exercise {exercise.Id} has {exercise.StudentExercises?.Count ?? 0} student exercises.");
                    foreach (var se in exercise.StudentExercises ?? new List<StudentExercise>())
                    {
                        Console.WriteLine($"Student Exercise: {se.Id}, Student: {se.Student?.Matricule}, Status: {se.Status}");
                    }
                }
            }

            if (lesson != null)
            {
                return lesson.Exercises
                    .Count(e => e.StudentExercises
                        .Any(se => se.Status == ExerciseStatus.Passed && se.Student != null && se.Student.Matricule == studentMatricule));
            }

            return 0;
        }

        public async Task<List<LessonDto>> GetLessonsWithStudentProgressAsync()
        {
            // Récupérer les IDs des étudiants ayant le rôle "Student"
            var studentRole = await _context.Roles.SingleAsync(r => r.Name == "Student");
            var studentIds = await _context.UserRoles
                .Where(ur => ur.RoleId == studentRole.Id)
                .Select(ur => ur.UserId)
                .ToListAsync();
            foreach (var studentId in studentIds)
            {
                Console.WriteLine("studentID : " + studentId);
            }

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
                .Where(e => e.LessonId == lessonId)
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



}
}
