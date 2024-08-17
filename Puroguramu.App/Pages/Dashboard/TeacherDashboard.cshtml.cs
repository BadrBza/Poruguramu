using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Puroguramu.Domains.modelsDomains;
using Puroguramu.Domains.Repositories;

namespace Puroguramu.App.Pages.Dashboard
{
    [Authorize(Roles = "teacher")]
    public class TeacherDashboardModel : PageModel
    {
        private readonly ILessonRepository _lessonRepository;
        private readonly IStudentRepository _studentRepository;

        public TeacherDashboardModel(ILessonRepository lessonRepository, IStudentRepository studentRepository)
        {
            _lessonRepository = lessonRepository;
            _studentRepository = studentRepository;
        }

        public List<LessonDto> Lessons { get; set; }
        public StudentDto Student { get; set; }

        public async Task OnGetAsync()
        {
            Lessons = await _lessonRepository.GetLessonsWithStudentProgressAsync();
            Student = await _studentRepository.GetStudentProfileAsync(User);
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostToggleLessonAsync(Guid id)
        {
            await _lessonRepository.ToggleLessonAsync(id);
            return RedirectToPage();
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostDeleteLessonAsync(Guid id)
        {
            await _lessonRepository.DeleteLessonAsync(id);
            return RedirectToPage();
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostMoveLessonAsync(Guid id, string direction)
        {
            bool moveUp = direction == "up";
            await _lessonRepository.MoveLessonAsync(id, moveUp);
            return RedirectToPage();
        }

        [ValidateAntiForgeryToken]
        public IActionResult OnPostEditLesson(Guid id)
        {
            return RedirectToPage("/Lecons/EditLesson", new { id });
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostResetLessonAsync(Guid id)
        {
            await _lessonRepository.ResetLessonAsync(id);
            return RedirectToPage();
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostResetAllLessonsAsync()
        {
            await _lessonRepository.ResetAllLessonsAsync();
            return RedirectToPage();
        }

    }
}
