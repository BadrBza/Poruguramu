using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Puroguramu.Domains.modelsDomains;
using Puroguramu.Domains.Repositories;

namespace Puroguramu.App.Pages.Dashboard;

[Authorize(Roles = "student")]
public class studentDashboard : PageModel
{
    private readonly IStudentRepository _studentRepository;
    private readonly ILessonRepository _lessonRepository;
    public studentDashboard(IStudentRepository studentRepository, ILessonRepository lessonRepository)
    {
        _studentRepository = studentRepository;
        _lessonRepository = lessonRepository;
    }

    public StudentDto Student { get; set; }
    public List<LessonDto> PublishedLessons { get; set; }
    public Dictionary<Guid, StudentExerciseDto> NextExercisesByLesson { get; set; } = new Dictionary<Guid, StudentExerciseDto>();
    public async Task OnGetAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Student = await _studentRepository.GetStudentProfileAsync(User);
        PublishedLessons = await _lessonRepository.GetPublishedLessonsWithProgressAsync(Student.Id);

        foreach (var lesson in PublishedLessons)
        {
            lesson.TotalExercises = await _lessonRepository.GetTotalExercisesCountAsync(lesson.Id);
            lesson.CompletedExercises = await _lessonRepository.GetCompletedExercisesCountAsync(lesson.Id, Student.Id);
            Console.WriteLine("total exercice dans lesson : " + lesson.TotalExercises);
            Console.WriteLine("total exercice fini dans lesson : " + lesson.CompletedExercises);
            var nextExercise = await _studentRepository.GetNextExerciseAsync(Student.Matricule, lesson.Id);
            NextExercisesByLesson[lesson.Id] = nextExercise;
        }
    }
}
