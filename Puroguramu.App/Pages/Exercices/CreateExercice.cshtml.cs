#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Puroguramu.Domains.Repositories;

namespace Puroguramu.App.Pages.Exercices
{
    [ValidateAntiForgeryToken]
    public class CreateExerciceModel : PageModel
    {
        private readonly IExoRepository _exoRepository;

        public CreateExerciceModel(IExoRepository exoRepository)
        {
            _exoRepository = exoRepository;
        }

        [BindProperty]
        [Required(ErrorMessage = "Le titre est requis.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Le titre doit comporter au moins cinq caractères.")]
        public string Title { get; set; }

        [BindProperty(SupportsGet = true)] public Guid LessonId { get; set; }

        public void OnGet(Guid lessonId)
        {
            LessonId = lessonId;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                // Vérifier l'unicité du titre au sein de la leçon
                if (await _exoRepository.ExerciseTitleExistsAsync(LessonId, Title))
                {
                    ModelState.AddModelError("Title", "Un exercice avec cet intitulé existe déjà dans cette leçon.");
                    return Page();
                }

                try
                {
                    await _exoRepository.CreateExerciseAsync(LessonId, Title);
                }
                catch (InvalidOperationException)
                {
                    ModelState.AddModelError(string.Empty, "La leçon spécifiée n'existe pas.");
                    return Page();
                }

                return RedirectToPage("/Lecons/EditLesson", new { id = LessonId });
            }

            return Page();
        }
    }
}
