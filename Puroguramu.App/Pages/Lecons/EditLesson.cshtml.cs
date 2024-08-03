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
        public List<ExerciseDto> Exercises { get; set; } = new List<ExerciseDto>();

        [BindProperty]
        public LessonEditDto Lesson { get; set; }

        public EditLesson(ILessonRepository lessonRepository)
        {
            _lessonRepository = lessonRepository;
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
                return Page();
            }
            await _lessonRepository.UpdateLessonAsync(Lesson);
            return RedirectToPage("/Dashboard/TeacherDashboard");
        }

    }
}
