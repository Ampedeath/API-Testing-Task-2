using FluentAssertions;
using Petstore.Client.Models;
using Petstore.Tests.Fixtures;
using static Petstore.Tests.Utils.TestSteps;

namespace Petstore.Tests.Tests;

[TestFixture]
public class UserTests : TestBase
{
    private User CreateUser()
    {
        return new User
        {
            Username = $"user_{Guid.NewGuid():N}".Substring(0, 8),
            Password = "password",
            Email = "test@test.com"
        };
    }

    [Test]
    public async Task CreateUser_ShouldSucceed()
    {
        var user = CreateUser();

        await UserClient.CreateAsync(user);

        RegisterCleanup(() => UserClient.DeleteAsync(user.Username));

        var fetched = await UserClient.GetAsync(user.Username);

        TestCaseStep("Verify created user username matches", () =>
        {
            fetched.Should().NotBeNull();
            fetched.Username.Should().Be(user.Username);
        });
    }

    [Test]
    public async Task UpdateUser_ShouldPersistChanges()
    {
        var user = CreateUser();

        await UserClient.CreateAsync(user);

        RegisterCleanup(() => UserClient.DeleteAsync(user.Username));

        var serverUser = await UserClient.GetAsync(user.Username);

        serverUser.Email = "updated@test.com";

        await UserClient.UpdateAsync(serverUser.Username, serverUser);

        var updated = await UserClient.GetAsync(user.Username);

        TestCaseStep("Verify email was updated", () =>
        {
            updated.Should().NotBeNull();
            updated.Email.Should().Be("updated@test.com");
        });
    }

    [Test]
    public async Task Login_ShouldSucceed()
    {
        var user = CreateUser();

        await UserClient.CreateAsync(user);

        RegisterCleanup(() => UserClient.DeleteAsync(user.Username));

        var result = await UserClient.LoginAsync(user.Username, user.Password);

        TestCaseStep("Verify login returned result", () =>
        {
            result.Should().NotBeNull();
        });
    }

    [Test]
    public async Task DeleteUser_ShouldRemoveUser()
    {
        var user = CreateUser();

        await UserClient.CreateAsync(user);

        await UserClient.DeleteAsync(user.Username);

        Func<Task> act = async () =>
            await UserClient.GetAsync(user.Username);

        await act.Should().ThrowAsync<HttpRequestException>();

        TestCaseStep("Verify user cannot be fetched after deletion", () =>
        {
            true.Should().BeTrue();
        });
    }

    // NEGATIVE

    [Test]
    public async Task Login_WrongPassword_ShouldFail()
    {
        var response = await UserClient.LoginRawAsync("fake", "wrong");

        TestCaseStep("Verify login fails with wrong password", () =>
        {
            response.IsSuccessStatusCode.Should().BeFalse();
        });
    }

    [Test]
    public async Task GetUser_NonExistent_ShouldFail()
    {
        Func<Task> act = async () =>
            await UserClient.GetAsync("fake");

        await act.Should().ThrowAsync<HttpRequestException>();

        TestCaseStep("Verify fetching non-existent user throws exception", () =>
        {
            true.Should().BeTrue();
        });
    }
}
