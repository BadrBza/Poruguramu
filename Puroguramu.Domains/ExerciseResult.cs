namespace Puroguramu.Domains;

public record ExerciseResult(Exercise Subject, string Proposal, IEnumerable<TestResult>? TestResults = null)
{
    public IEnumerable<TestResult> TestResults { get; } = TestResults ?? Array.Empty<TestResult>();

    public ExerciseStatus Status
        => !TestResults.Any()
            ? ExerciseStatus.NotStarted
            : TestResults.Any(test => test.Status != TestStatus.Passed)
                ? ExerciseStatus.Started
                : ExerciseStatus.Passed;
}

public record TestResult(string Label, TestStatus Status, string ErrorMessage = "");

public enum ExerciseStatus
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
