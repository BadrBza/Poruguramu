using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Puroguramu.Domains.modelsDomains;
using Puroguramu.Domains.Repositories;

namespace Puroguramu.App.Pages.Lecons
{
    public class CreateLessonModel : PageModel
    {
        private readonly ILessonRepository _lessonRepository;

        public CreateLessonModel(ILessonRepository lessonRepository)
        {
            _lessonRepository = lessonRepository;
        }

        [BindProperty]
        public LessonDto Lesson { get; set; }


        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _lessonRepository.CreateLessonAsync(Lesson);
                return RedirectToPage("/Dashboard/TeacherDashboard");
            }
            catch (InvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
        }
    }
}
