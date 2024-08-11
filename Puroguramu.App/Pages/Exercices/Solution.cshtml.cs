using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Puroguramu.Domains;
using Puroguramu.Domains.Repositories;
using System;
using System.Threading.Tasks;

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
            var studentExercise = await _studentRepository.GetStudentExerciseByIdAsync(student.Id, exerciseId);
            var exercise = _exercisesRepository.GetExercise(exerciseId);

            if (exercise == null)
            {
                return NotFound();
            }

            ExerciseTitle = exercise.Title;

            if (studentExercise.Statuts != ExerciseStatuts.Passed)
            {
                await _exercisesRepository.UpdateStudentExerciseAbandonnedStatusAsync(exerciseId, student.Id);
            }

            SolutionText = exercise.Solution;
            return Page();
        }
    }
}
