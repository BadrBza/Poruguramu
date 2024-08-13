using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Puroguramu.Domains.modelsDomains;
using Puroguramu.Domains.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Puroguramu.Domains;

namespace Puroguramu.App.Pages.Lecons
{
    public class LessonDetail : PageModel
    {
        private readonly ILessonRepository _lessonRepository;
        private readonly IExoRepository _exoRepository;
        private readonly IStudentRepository _studentRepository;

        public LessonDetail(ILessonRepository lessonRepository, IExoRepository exoRepository, IStudentRepository studentRepository)
        {
            _lessonRepository = lessonRepository;
            _exoRepository = exoRepository;
            _studentRepository = studentRepository;
        }

        public LessonEditDto Lesson { get; set; }
        public List<StudentExerciseDto> Exercises { get; set; }
        public StudentDto Student { get; set; }

        public Exercice Exercice { get; set; }


        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            Student = await _studentRepository.GetStudentProfileAsync(User);

            Lesson = await _lessonRepository.GetLessonByIdAsync(id);
            if (Lesson == null)
            {
                return NotFound();
            }

            Exercises = await _lessonRepository.GetAllExercisesByLessonAsync(Student.Id, Lesson.Id);

            return Page();
        }
    }
}
