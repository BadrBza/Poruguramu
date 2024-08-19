using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Puroguramu.Domains.modelsDomains;
using Puroguramu.Domains.Repositories;
using Puroguramu.Infrastructures.Data.models;

namespace Puroguramu.App.Pages.Lecons
{
    public class EditLesson : PageModel
    {
        private readonly ILessonRepository _lessonRepository;
        private readonly IExoRepository _exoRepository;
        private readonly IStudentRepository _studentRepository;

        public List<ExerciseDto> Exercises { get; set; } = new List<ExerciseDto>();

        [BindProperty]
        public LessonEditDto Lesson { get; set; }
        public StudentExerciseDto StudentExercise { get; set; }

        public EditLesson(ILessonRepository lessonRepository, IExoRepository exoRepository, IStudentRepository studentRepository)
        {
            _lessonRepository = lessonRepository;
            _exoRepository = exoRepository;
            _studentRepository = studentRepository;
        }

        public async Task OnGetAsync(Guid id)
        {
            Lesson = await _lessonRepository.GetLessonByIdAsync(id);
            Exercises = await _lessonRepository.GetExercisesByLessonIdAsync(id);
        }


        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Console.WriteLine("OnPostAsync UpdateLessonAsync");
            await _lessonRepository.UpdateLessonAsync(Lesson);
            return RedirectToPage("/Dashboard/TeacherDashboard");
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostMoveExerciseAsync(Guid id, string direction, string lessonId)
        {
            Console.WriteLine("Entry OnPostMoveExerciseAsync");
            var moveUp = direction == "up";
            await _exoRepository.MoveExerciseAsync(id, moveUp);
            return RedirectToPage("/Lecons/EditLesson", new { id = lessonId });
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostToggleExerciseAsync(Guid id, string lessonId)
        {
            Console.WriteLine($"Entry OnPostToggleExerciseAsync with id: {id} and lessonId: {lessonId}");
            await _exoRepository.ToggleExerciseAsync(id);
            return RedirectToPage("/Lecons/EditLesson", new { id = lessonId });
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostDeleteExerciseAsync(Guid id, string lessonId)
        {
            await _exoRepository.DeleteExerciseAsync(id);
            return RedirectToPage("/Lecons/EditLesson", new { id = lessonId });
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostResetExerciseAsync(Guid id, Guid lessonId)
        {
            var student = _studentRepository.GetStudentProfileAsync(User);
            Console.WriteLine("id trouvé = " + student.Id);
            await _lessonRepository.DeleteStudentExerciseDataAsync(id, student.Id.ToString());
            await _lessonRepository.ResetExerciseAsync(id);
            return RedirectToPage("/Lecons/EditLesson", new { id = lessonId });
        }
    }
}
