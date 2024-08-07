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
        public StudentExerciseDto StudentExercise { get; set; }

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
            // Vérification si l'étudiant est null
            if (Student == null)
            {
                // Gérer le cas où l'étudiant est introuvable
                Console.WriteLine("Aucun etudiant");
            }

            StudentExercise = await _studentRepository.GetStudentExerciseByIdAsync(Student.Id, exerciseId);

            if (StudentExercise == null)
            {
                // L'étudiant n'a pas encore commencé cet exercice, vous pouvez initialiser un nouvel objet ou simplement informer l'utilisateur
                Console.WriteLine("Aucun exercice trouvé pour cet étudiant. Initialisation d'un nouvel exercice.");
            }

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
            if (exercise == null)
            {
                return NotFound();
            }

            ExerciseTitle = exercise.Title;
            ExerciseStatut = exercise.Difficulty.ToString();
            DescriptionExo = exercise.Description;



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
                // Si l'exercice étudiant n'existe pas, créer une nouvelle entrée
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

            // Mettre à jour le statut de l'exercice en fonction du résultat de l'évaluation
            studentExercise.Statuts = _result.Statuts;
            studentExercise.Code = Proposal;
            studentExercise.StudentMatricule = Student.Matricule;
            studentExercise.StudentId = Student.Id;

            Console.WriteLine("statut exo StudentExercice = " + studentExercise.Statuts);


            await _exercisesRepository.SaveStudentProposalAsync(exerciseId, Student.Id, Proposal);
            await _studentRepository.UpdateStudentExerciseStatusAsync(studentExercise);
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
