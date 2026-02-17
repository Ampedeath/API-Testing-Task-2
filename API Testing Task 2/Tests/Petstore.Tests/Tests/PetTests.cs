using FluentAssertions;
using Petstore.Client.Models;
using Petstore.Tests.Fixtures;
using Petstore.Tests.TestData;
using static Petstore.Tests.Utils.TestSteps;


namespace Petstore.Tests.Tests;

[TestFixture]
public class PetTests : TestBase
{
    [Test]
    public async Task CreatePet_ShouldCreatePet()
    {
        var pet = PetBuilder.Default();

        var created = await CreatePetWithCleanupAsync(pet);

        TestCaseStep("Verify created pet fields", () =>
        {
            created.Id.Should().BeGreaterThan(0);
            created.Name.Should().Be(pet.Name);
            created.Status.Should().Be(pet.Status);
        });
    }

    [Test]
    public async Task GetPet_ShouldReturnCreatedPet()
    {
        var pet = PetBuilder.Default();

        var created = await CreatePetWithCleanupAsync(pet);

        var fetched = await PetClient.GetAsync(created.Id);

        TestCaseStep("Verify fetched pet matches created pet", () =>
        {
            fetched.Id.Should().Be(created.Id);
            fetched.Name.Should().Be(created.Name);
            fetched.Status.Should().Be(created.Status);
        });
    }

    [Test]
    public async Task UpdatePet_ShouldPersistChanges()
    {
        var pet = PetBuilder.Default();

        var created = await CreatePetWithCleanupAsync(pet);

        created.Name = "updated-name";

        await PetClient.UpdateAsync(created);

        var updated = await PetClient.GetAsync(created.Id);

        TestCaseStep("Verify pet updated", () =>
        {
            updated.Name.Should().Be("updated-name");
        });
    }

    [TestCase("available")]
    [TestCase("pending")]
    [TestCase("sold")]
    public async Task FindByStatus_ShouldReturnValidPets(string status)
    {
        var result = await PetClient.FindByStatusAsync(status);

        TestCaseStep("Verify all pets have correct status", () =>
        {
            result.Should().NotBeEmpty();

            result.All(p => p.Status == status)
                .Should().BeTrue();
        });
    }

    [Test]
    public async Task DeletePet_ShouldRemovePet()
    {
        var pet = PetBuilder.Default();

        var created = await CreatePetWithCleanupAsync(pet);

        await PetClient.DeleteAsync(created.Id);

        Func<Task> act = async () => await PetClient.GetAsync(created.Id);

        await act.Should().ThrowAsync<HttpRequestException>();
    }

    // NEGATIVE

    [Test]
    public async Task CreatePet_InvalidBody_ShouldFail()
    {
        var invalidPet = new Pet
        {
            Name = null!,
            PhotoUrls = new()
        };

        Func<Task> act = async () => await PetClient.CreateAsync(invalidPet);

        var response = await PetClient.CreateRawAsync(invalidPet);

        response.IsSuccessStatusCode.Should().BeFalse();
    }

    [Test]
    public async Task GetPet_NonExistent_ShouldReturn404()
    {
        var response = await PetClient.GetRawAsync(122434543);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }


    [TestCase(0)]
    [TestCase(-1)]
    public async Task GetPet_InvalidId_ShouldFail(long invalidId)
    {
        Func<Task> act = async () => await PetClient.GetAsync(invalidId);

        await act.Should().ThrowAsync<HttpRequestException>();
    }
}
