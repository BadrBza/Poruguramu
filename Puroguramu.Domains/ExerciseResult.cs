namespace Puroguramu.Domains;

public record ExerciseResult(Exercise Subject, string Proposal, IEnumerable<TestResult>? TestResults = null)
{
    public IEnumerable<TestResult> TestResults { get; } = TestResults ?? Array.Empty<TestResult>();

    public ExerciseStatuts Statuts
        => !TestResults.Any()
            ? ExerciseStatuts.NotStarted
            : TestResults.Any(test => test.Status != TestStatus.Passed)
                ? ExerciseStatuts.Started
                : ExerciseStatuts.Passed;
}

public record TestResult(string Label, TestStatus Status, string ErrorMessage = "");

public enum ExerciseStatuts
{
    Failed=0,
    NotStarted=1,
    Started=2,
    Passed=3,
}

public enum TestStatus
{
    Inconclusive  = 0,
    Failed = 1,
    Passed = 2,
}

public enum DifficultyExo
{
    Easy,
    Medium,
    Hard
}
