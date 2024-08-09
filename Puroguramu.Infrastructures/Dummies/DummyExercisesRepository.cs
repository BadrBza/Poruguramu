    using Microsoft.EntityFrameworkCore;
    using Puroguramu.Domains;
    using Puroguramu.Infrastructures.Data;
    using Puroguramu.Infrastructures.Data.models;

    namespace Puroguramu.Infrastructures.Dummies;

    public class DummyExercisesRepository : IExercisesRepository
    {
        private readonly ApplicationDbContext _context;

        public DummyExercisesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Exercise GetExercise(Guid exerciseId)
        {
            var exoEntity = _context.Exercises
                .SingleOrDefault(e => e.Id == exerciseId);

            if (exoEntity == null)
            {
                return null;
            }

            return new Exercise
            {
                Id = exoEntity.Id,
                Title = exoEntity.Title,
                Description = exoEntity.Description,
                LessonId = exoEntity.LessonId,
                Difficulty = MapDifficulty(exoEntity.Difficulty),
                IsPublished = exoEntity.IsPublished,
                Template = exoEntity.Template,
                Stub = exoEntity.Stub,
                Solution = exoEntity.Solution
            };
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

        public async Task<string> GetStudentProposalAsync(Guid exerciseId, string studentId)
        {
            var studentExercise = await _context.StudentExercise
                .FirstOrDefaultAsync(se => se.ExoId == exerciseId && se.StudentId == studentId);
            return studentExercise?.Code ?? string.Empty;
        }

        public async Task SaveStudentProposalAsync(Guid exerciseId, string studentId, string proposal)
        {
            var studentExercise = await _context.StudentExercise
                .SingleOrDefaultAsync(se => se.ExoId == exerciseId && se.StudentId == studentId);

            if (studentExercise == null)
            {
                var student = await _context.Students.SingleOrDefaultAsync(s => s.Id == studentId);
                if (student == null)
                {
                    throw new Exception("Étudiant non trouvé");
                }

                studentExercise = new StudentExercise
                {
                    Id = Guid.NewGuid(),
                    ExoId = exerciseId,
                    StudentId = studentId,
                    StudentMatricule = student.Matricule,
                    Code = proposal,
                    Status = ExerciseStatus.NotStarted
                };
                _context.StudentExercise.Add(studentExercise);
            }
            else
            {
                studentExercise.Code = proposal;
                studentExercise.Status = ExerciseStatus.Started;
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateStudentExerciseAbandonnedStatusAsync(Guid exerciseId, string studentId)
        {
            var studentExercise = await _context.StudentExercise
                .SingleOrDefaultAsync(se => se.ExoId == exerciseId && se.StudentId == studentId);

            if (studentExercise != null)
            {
                studentExercise.Status = ExerciseStatus.Failed;
                await _context.SaveChangesAsync();
            }
        }






    }
