using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Puroguramu.Domains;
using Puroguramu.Domains.Repositories;
using System;
using System.Threading.Tasks;
using Puroguramu.Domains.modelsDomains;

namespace Puroguramu.App.Pages.Exercices
{
     public class SolutionModel : PageModel
    {
        private readonly IExercisesRepository _exercisesRepository;
        private readonly IStudentRepository _studentRepository;

        public string ExerciseTitle { get; set; }
        public string SolutionText { get; set; }
        public Guid ExerciseId { get; set; }

        public SolutionModel(IExercisesRepository exercisesRepository, IStudentRepository studentRepository)
        {
            _exercisesRepository = exercisesRepository;
            _studentRepository = studentRepository;
        }

        public async Task<IActionResult> OnGetAsync(Guid exerciseId)
        {
            ExerciseId = exerciseId;

            var student = await _studentRepository.GetStudentProfileAsync(User);
            if (student == null)
            {
                return NotFound("Student not found.");
            }

            var exercise = _exercisesRepository.GetExercise(exerciseId);
            if (exercise == null)
            {
                return NotFound("Exercise not found.");
            }

            var studentExercise = await _studentRepository.GetStudentExerciseByIdAsync(student.Id, exerciseId);

            if (studentExercise == null)
            {
                studentExercise = new StudentExerciseDto
                {
                    ExerciseId = exerciseId,
                    StudentId = student.Id,
                    Statuts = ExerciseStatuts.Started,
                    Code = string.Empty,
                    StudentMatricule = student.Matricule
                };
                await _studentRepository.AddStudentExerciseAsync(studentExercise);
            }


            ExerciseTitle = exercise.Title;

            if (studentExercise.Statuts == ExerciseStatuts.Started || studentExercise.Statuts == ExerciseStatuts.Failed || studentExercise.Statuts == ExerciseStatuts.NotStarted)
            {
                await _exercisesRepository.UpdateStudentExerciseAbandonnedStatusAsync(exerciseId, student.Id);
            }

            SolutionText = exercise.Solution;

            return Page();
        }
    }
}
