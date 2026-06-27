using NetArchTest.Rules;

namespace CleanModular.ArchTests;

/// <summary>
/// Adds ShouldBeSuccessful() to NetArchTest's TestResult.
/// Produces a clear failure message listing every type that violated the rule.
/// </summary>
internal static class TestResultExtensions
{
    internal static void ShouldBeSuccessful(this TestResult result)
    {
        if (result.IsSuccessful)
            return;

        var failingTypes = result.FailingTypes?
            .Select(t => t.FullName ?? t.Name)
            .ToList() ?? [];

        Assert.Fail(
            $"Architecture rule violated by the following types:\n  - " +
            string.Join("\n  - ", failingTypes));
    }
}
