using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Puroguramu.Domains.modelsDomains;
using Puroguramu.Domains.Repositories;

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
            Lesson = await _lessonRepository.GetLessonByIdAsync(id);
            Exercises = await _lessonRepository.GetExercisesByLessonIdAsync(id);
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page(); // Returns to the same page if validation fails
            }

            Console.WriteLine("OnPostAsync UpdateLessonAsync");
            await _lessonRepository.UpdateLessonAsync(Lesson);
            return RedirectToPage("/Dashboard/TeacherDashboard");
        }

        public async Task<IActionResult> OnPostMoveExerciseAsync(Guid id, string direction, string lessonId)
        {
            Console.WriteLine("Entry OnPostMoveExerciseAsync");
            var moveUp = direction == "up";
            await _exoRepository.MoveExerciseAsync(id, moveUp);
            return RedirectToPage("/Lecons/EditLesson", new { id = lessonId });
        }

        public async Task<IActionResult> OnPostToggleExerciseAsync(Guid id, string lessonId)
        {
            Console.WriteLine($"Entry OnPostToggleExerciseAsync with id: {id} and lessonId: {lessonId}");
            await _exoRepository.ToggleExerciseAsync(id);
            return RedirectToPage("/Lecons/EditLesson", new { id = lessonId });
        }

        public async Task<IActionResult> OnPostDeleteExerciseAsync(Guid id, string lessonId)
        {
            await _exoRepository.DeleteExerciseAsync(id);
            return RedirectToPage("/Lecons/EditLesson", new { id = lessonId });
        }
    }
}
