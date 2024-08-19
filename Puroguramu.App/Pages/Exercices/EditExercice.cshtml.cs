using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Puroguramu.Domains;
using Puroguramu.Domains.Repositories;
using Puroguramu.Domains.modelsDomains;
using Puroguramu.Infrastructures.Data.models;

namespace Puroguramu.App.Pages.Exercices
{
    public class EditExerciceModel : PageModel
    {
        private readonly IExoRepository _exoRepository;
        private readonly IStudentRepository _studentRepository;

        public EditExerciceModel(IExoRepository exoRepository, IStudentRepository studentRepository)
        {
            _exoRepository = exoRepository;
            _studentRepository = studentRepository;
        }

        [BindProperty(SupportsGet = true)]
        public Guid LessonId { get; set; }

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Le titre est requis.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Le titre doit comporter au moins cinq caractères.")]
        public string Title { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "L'énoncé est requis.")]
        public string Description { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Le modèle est requis.")]
        public string Template { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Le stub est requis.")]
        public string Stub { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "La solution est requise.")]
        public string Solution { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "La difficulté est requise.")]
        public DifficultyExo DifficultyExo { get; set; }

        public StudentDto Student { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Student = await _studentRepository.GetStudentProfileAsync(User);
            var exercise = await _exoRepository.GetExerciseByIdAsync(Id);
            if (exercise == null)
            {
                return NotFound();
            }

            Title = exercise.Title;
            Description = exercise.Description;
            Template = exercise.Template;
            Stub = exercise.Stub;
            Solution = exercise.Solution;
            DifficultyExo = exercise.Difficulty;

            return Page();
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var exercise = new ExerciseEditDto
            {
                Id = Id,
                Title = Title,
                Description = Description,
                Template = Template,
                Stub = Stub,
                Solution = Solution,
                Difficulty = DifficultyExo
            };

            await _exoRepository.UpdateExerciseAsync(exercise);

            return RedirectToPage("/Lecons/EditLesson", new { id = LessonId });
        }
    }
}
