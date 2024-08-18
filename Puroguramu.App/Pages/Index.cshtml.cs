using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Puroguramu.Domains.Repositories;

namespace Puroguramu.App.Pages;

public class IndexModel : PageModel
{
    private readonly IStudentRepository _studentRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly IExoRepository _exoRepository;

    public int LessonsCount { get; set; }
    public int StudentsCount { get; set; }
    public int ExercicesCount { get; set; }

    public IndexModel(IStudentRepository studentRepository, ILessonRepository lessonRepository, IExoRepository exoRepository)
    {
        _studentRepository = studentRepository;
        _lessonRepository = lessonRepository;
        _exoRepository = exoRepository;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var redirectUrl = await _studentRepository.GetDashboardRedirectUrlAsync(User);

        if (redirectUrl != null)
        {
            return Redirect(redirectUrl);
        }

        LessonsCount = await _lessonRepository.GetLessonsCountAsync();
        StudentsCount = await _studentRepository.GetStudentsCountAsync();
        ExercicesCount = await _exoRepository.GetExercicesCountAsync();

        return Page();
    }
}
