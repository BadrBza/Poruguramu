using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Puroguramu.Domains;

namespace Puroguramu.App.Pages;

public class Exercice : PageModel
{
    private readonly IAssessExercise _assessor;

    private ExerciseResult? _result;

    [BindProperty]
    public string Proposal { get; set; } = string.Empty;

    public string ExerciseTitle { get; set; }
    public string ExerciseStatut { get; set; }

    public string ExerciseResultStatus
        => _result?.Status switch
        {
            ExerciseStatus.NotStarted => "Not Started",
            ExerciseStatus.Started => "Started",
            ExerciseStatus.Passed => "Succeeded",
            ExerciseStatus.Failed => "Failed",
            _ => "Unknown"
        };

    public IEnumerable<TestResultViewModel> TestResult
        => _result
            ?.TestResults
            ?.Select(result => new TestResultViewModel(result)) ?? Array.Empty<TestResultViewModel>();

    public Exercice(IAssessExercise assessor)
    {
        _assessor = assessor;
    }

    public async Task OnGetAsync(string exerciseTitle, string exerciseStatus)
    {
        ExerciseTitle = exerciseTitle;
        ExerciseStatut = exerciseStatus;

        _result = await _assessor.StubForExercise(Guid.Empty);
        Proposal = _result.Proposal;
    }

    public async Task OnPostAsync()
    {
        _result = await _assessor.Assess(Guid.Empty, Proposal);
    }
}

public record TestResultViewModel(TestResult Result)
{
    public string Status
        => Result.Status.ToString();

    public string Label
        => Result.Label;

    public bool HasError
        => Result.Status != TestStatus.Passed;

    public string ErrorMessage
        => Result.ErrorMessage;
}
