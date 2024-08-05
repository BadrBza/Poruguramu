using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Puroguramu.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puroguramu.App.Pages
{
    public class Exercice : PageModel
    {
        private readonly IAssessExercise _assessor;
        private readonly IExercisesRepository _exercisesRepository;

        public ExerciseResult? _result { get; set; }
        public string ExerciseTitle { get; set; }
        public string ExerciseStatut { get; set; }
        public string Proposal { get; set; } = string.Empty;

        public IEnumerable<TestResultViewModel> TestResult
            => _result?.TestResults?.Select(result => new TestResultViewModel(result)) ?? Array.Empty<TestResultViewModel>();

        public Exercice(IAssessExercise assessor, IExercisesRepository exercisesRepository)
        {
            _assessor = assessor;
            _exercisesRepository = exercisesRepository;
        }

        public async Task OnGetAsync(Guid exerciseId)
        {
            var exercise =  _exercisesRepository.GetExercise(exerciseId);
            if (exercise == null)
            {
                Console.WriteLine("Exercice non trouvé miaow");
                return;
            }
            ExerciseTitle = exercise.Title;
            ExerciseStatut = exercise.Difficulty.ToString();

            Proposal = await _exercisesRepository.GetStudentProposalAsync(exerciseId, User.Identity.Name);

            if (string.IsNullOrEmpty(Proposal))
            {
                _result = await _assessor.StubForExercise(exerciseId);
                Proposal = _result.Proposal;
            }
        }

        public async Task<IActionResult> OnPostAsync(Guid exerciseId)
        {
            _result = await _assessor.Assess(exerciseId, Proposal);
            await _exercisesRepository.SaveStudentProposalAsync(exerciseId, User.Identity.Name, Proposal);

            return Page();
        }
    }

    public record TestResultViewModel(TestResult Result)
    {
        public string Status => Result.Status.ToString();
        public string Label => Result.Label;
        public bool HasError => Result.Status != TestStatus.Passed;
        public string ErrorMessage => Result.ErrorMessage;
    }
}
