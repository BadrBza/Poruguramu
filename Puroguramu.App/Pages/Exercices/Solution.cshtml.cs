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

            // Vérifier si l'utilisateur est authentifié
            var student = await _studentRepository.GetStudentProfileAsync(User);
            if (student == null)
            {
                return NotFound("Student not found.");
            }

            // Récupérer l'exercice de l'étudiant
            var studentExercise = await _studentRepository.GetStudentExerciseByIdAsync(student.Id, exerciseId);
            if (studentExercise == null)
            {
                return NotFound("Student exercise not found.");
            }

            // Récupérer l'exercice
            var exercise = _exercisesRepository.GetExercise(exerciseId);
            if (exercise == null)
            {
                return NotFound("Exercise not found.");
            }

            ExerciseTitle = exercise.Title;


            // Vérifier si l'exercice est en cours ou échoué, puis le marquer comme abandonné
            if (studentExercise.Statuts == ExerciseStatuts.Started || studentExercise.Statuts == ExerciseStatuts.Failed || studentExercise.Statuts == ExerciseStatuts.NotStarted)
            {
                await _exercisesRepository.UpdateStudentExerciseAbandonnedStatusAsync(exerciseId, student.Id);
            }

            // Assigner le texte de la solution
            SolutionText = exercise.Solution;

            return Page();
        }

    }
}
