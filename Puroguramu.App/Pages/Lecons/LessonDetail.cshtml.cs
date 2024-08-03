using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Puroguramu.Domains.modelsDomains;
using Puroguramu.Domains.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            // Récupérer les informations de l'étudiant
            Student = await _studentRepository.GetStudentProfileAsync(User);

            // Récupérer la leçon par ID
            Lesson = await _lessonRepository.GetLessonByIdAsync(id);
            if (Lesson == null)
            {
                return NotFound();
            }

            // Récupérer tous les exercices pour l'étudiant dans la leçon spécifiée
            Exercises = await _lessonRepository.GetAllExercisesByLessonAsync(Student.Id, Lesson.Id);

            return Page();
        }
    }
}
