using Allure.Net.Commons;

namespace Petstore.Tests.Utils;

public static class TestSteps
{
    public static void TestCaseStep(string name, Action action)
    {
        AllureApi.Step(name, action);
    }

    public static async Task TestCaseStepAsync(string name, Func<Task> action)
    {
        await AllureApi.Step(name, action);
    }
}
