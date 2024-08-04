using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Puroguramu.Domains.modelsDomains;
using Puroguramu.Domains.Repositories;
using System;
using System.Threading.Tasks;

namespace Puroguramu.App.Pages.Lecons
{
    public class EditLesson : PageModel
    {
        private readonly ILessonRepository _lessonRepository;
        private readonly IExoRepository _exoRepository;
        public List<ExerciseDto> Exercises { get; set; } = new List<ExerciseDto>();

        [BindProperty]
        public LessonEditDto Lesson { get; set; }

        public EditLesson(ILessonRepository lessonRepository, IExoRepository exoRepository)
        {
            _lessonRepository = lessonRepository;
            _exoRepository = exoRepository;
        }

        public async Task OnGetAsync(Guid id)
        {
            Console.WriteLine("OnGetAsync ID : " + id);
            Lesson = await _lessonRepository.GetLessonByIdAsync(id);
            Console.WriteLine("Lesson ID = " + Lesson.Id);
            Exercises = await _lessonRepository.GetExercisesByLessonIdAsync(id);
            if (Lesson == null)
            {
                Console.WriteLine("Leçon introuvable pour l'ID : " + id);
            }
            else
            {
                Console.WriteLine("Leçon chargée : " + Lesson.Id);
            }
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            await _lessonRepository.UpdateLessonAsync(Lesson);
            return RedirectToPage("/Dashboard/TeacherDashboard");
        }

        public async Task<IActionResult> OnPostMoveExerciseAsync(Guid id, string direction, string lessonId)
        {
            try
            {
                bool moveUp = direction == "up";
                await _exoRepository.MoveExerciseAsync(id, moveUp);
                return RedirectToPage("/Lecons/EditLesson", new { id = lessonId });
            }
            catch (Exception ex)
            {
                // Logger l'erreur pour comprendre ce qui se passe
                Console.WriteLine($"Erreur lors du déplacement de l'exercice: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                // Optionnel: rediriger vers une page d'erreur
                return RedirectToPage("/Error");
            }
        }
    }
}
