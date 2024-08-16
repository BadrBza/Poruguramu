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
        private readonly ILessonRepository _lessonRepository;

        public ExerciseResult? _result { get; set; }
        public string ExerciseTitle { get; set; }
        public string ExerciseStatut { get; set; }
        public string DescriptionExo { get; set; }
        public StudentDto Student { get; set; }
        public string Proposal { get; set; } = string.Empty;
        public StudentExerciseDto StudentExercise { get; set; }
        public Guid IdExo { get; set; }

        public bool ShowRunButton { get; set; } = true;

        public IEnumerable<TestResultViewModel> TestResult
            => _result?.TestResults?.Select(result => new TestResultViewModel(result)) ?? Array.Empty<TestResultViewModel>();

        public Exercice(IAssessExercise assessor, IExercisesRepository exercisesRepository, IStudentRepository studentRepository, ILessonRepository lessonRepository)
        {
            _assessor = assessor;
            _exercisesRepository = exercisesRepository;
            _studentRepository = studentRepository;
            _lessonRepository = lessonRepository;
        }

        public async Task OnGetAsync(Guid exerciseId)
        {
            Student = await _studentRepository.GetStudentProfileAsync(User);
            StudentExercise = await _studentRepository.GetStudentExerciseByIdAsync(Student.Id, exerciseId);
            if (StudentExercise != null)
            {
                StudentExercise.Tentative = await _studentRepository.GetStudentTentativesAsync(exerciseId, Student.Id);
            }

            var exercise = _exercisesRepository.GetExercise(exerciseId);

            ExerciseTitle = exercise.Title;
            ExerciseStatut = exercise.Difficulty.ToString();
            DescriptionExo = exercise.Description;
            IdExo = exerciseId;

            Proposal = await _exercisesRepository.GetStudentProposalAsync(exerciseId, Student.Id);

            if (string.IsNullOrEmpty(Proposal))
            {
                _result = await _assessor.StubForExercise(exerciseId);
                Proposal = _result.Proposal;
            }

            if (StudentExercise != null && StudentExercise.Statuts == ExerciseStatuts.Failed)
            {
                ShowRunButton = false;
            }
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync(Guid exerciseId)
        {

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var exercise = _exercisesRepository.GetExercise(exerciseId);
            if (exercise == null)
            {
                return NotFound();
            }

            ExerciseTitle = exercise.Title;
            ExerciseStatut = exercise.Difficulty.ToString();
            DescriptionExo = exercise.Description;
            IdExo = exerciseId;

            Student = await _studentRepository.GetStudentProfileAsync(User);
            if (Student == null)
            {
                return NotFound();
            }

            Proposal = await _exercisesRepository.GetStudentProposalAsync(exerciseId, Student.Id);
            Proposal = Request.Form["Proposal"];
            _result = await _assessor.Assess(exerciseId, Proposal);

            var studentExercise = await _studentRepository.GetStudentExerciseByIdAsync(Student.Id, exerciseId);
            if (studentExercise == null)
            {
                studentExercise = new StudentExerciseDto
                {
                    ExerciseId = exerciseId,
                    Statuts = Domains.ExerciseStatuts.NotStarted,
                    Title = exercise.Title,
                    StudentId = Student.Id,
                    DifficultyExo = exercise.Difficulty,
                    Code = Proposal,
                };
            }

            studentExercise.Statuts = _result.Statuts;
            studentExercise.Code = Proposal;
            studentExercise.StudentMatricule = Student.Matricule;
            studentExercise.StudentId = Student.Id;

            await _studentRepository.SaveStudentAttemptAsync(exerciseId, Student.Id, Proposal, _result.Statuts);
            await _lessonRepository.GetCompletedExercisesCountAsync(exercise.LessonId, Student.Id);
            await _exercisesRepository.SaveStudentProposalAsync(exerciseId, Student.Id, Proposal);
            await _studentRepository.UpdateStudentExerciseStatusAsync(studentExercise);

            if (studentExercise.Statuts == ExerciseStatuts.Failed)
            {
                ShowRunButton = false;
            }
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
