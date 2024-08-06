using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Puroguramu.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Puroguramu.Domains.modelsDomains;
using Puroguramu.Domains.Repositories;

namespace Puroguramu.App.Pages
{
    public class Exercice : PageModel
    {
        private readonly IAssessExercise _assessor;
        private readonly IExercisesRepository _exercisesRepository;
        private readonly IStudentRepository _studentRepository;

        public ExerciseResult? _result { get; set; }
        public string ExerciseTitle { get; set; }
        public string ExerciseStatut { get; set; }
        public string DescriptionExo { get; set; }
        public StudentDto Student { get; set; }
        public string Proposal { get; set; } = string.Empty;

        public IEnumerable<TestResultViewModel> TestResult
            => _result?.TestResults?.Select(result => new TestResultViewModel(result)) ?? Array.Empty<TestResultViewModel>();

        public Exercice(IAssessExercise assessor, IExercisesRepository exercisesRepository, IStudentRepository studentRepository)
        {
            _assessor = assessor;
            _exercisesRepository = exercisesRepository;
            _studentRepository = studentRepository;
        }

        public async Task OnGetAsync(Guid exerciseId)
        {
            Student = await _studentRepository.GetStudentProfileAsync(User);
            var exercise = _exercisesRepository.GetExercise(exerciseId);
            if (exercise == null)
            {
                Console.WriteLine("Exercice non trouvé miaow");
                Console.WriteLine("id" + exercise);
            }

            ExerciseTitle = exercise.Title;
            ExerciseStatut = exercise.Difficulty.ToString();
            DescriptionExo = exercise.Description;

            Proposal = await _exercisesRepository.GetStudentProposalAsync(exerciseId, Student.Id);

            if (string.IsNullOrEmpty(Proposal))
            {
                _result = await _assessor.StubForExercise(exerciseId);
                Proposal = _result.Proposal;
            }
        }

        public async Task<IActionResult> OnPostAsync(Guid exerciseId)
        {
            var exercise = _exercisesRepository.GetExercise(exerciseId);
            ExerciseTitle = exercise.Title;
            ExerciseStatut = exercise.Difficulty.ToString();
            DescriptionExo = exercise.Description;

            Student = await _studentRepository.GetStudentProfileAsync(User);
            Proposal = await _exercisesRepository.GetStudentProposalAsync(exerciseId, Student.Id);
            Proposal = Request.Form["Proposal"];


            _result = await _assessor.Assess(exerciseId, Proposal);

            await _exercisesRepository.SaveStudentProposalAsync(exerciseId, Student.Id, Proposal);

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
