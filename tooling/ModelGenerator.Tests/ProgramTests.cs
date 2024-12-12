using FluentAssertions;

namespace Abs.CommonCore.Drex.Contracts.ModelGenerator.Tests;

public class ProgramTests
{
    [Fact]
    public async Task Program_GivenValidInputs_ShouldReturnSuccessCodeAndGenerateOutputFiles()
    {
        // Arrange
        var args = new List<string>
        {
            "--input", "./TestData/Schemas",
            "--project", "./TestData/TestData.csproj",
            "--namespace", "TestNamespace"
        };
        const string outputDirectory = "./TestData/Generated";
        if (Directory.Exists(outputDirectory))
        {
            Directory.Delete(outputDirectory, true);
        }

        // Act
        var result = await Program.Main(args.ToArray());

        // Assert
        result.Should().Be(0);
        var outputFiles = Directory.GetFiles("./TestData/Generated");
        outputFiles.Should().HaveCount(2);
        outputFiles.Should().Satisfy(
            _ => _.Contains("SampleOneClass"),
            _ => _.Contains("SampleTwoClass"));
    }

    [Fact]
    public async Task Program_GivenEmptyInputDirectory_ShouldReturnSuccessCodeAndOutputNoFiles()
    {
        // Arrange
        var args = new List<string>
        {
            "--input", "./TestData/NoSchemas",
            "--project", "./TestData/TestData.csproj",
            "--namespace", "TestNamespace"
        };

        // Act
        var result = await Program.Main(args.ToArray());

        // Assert
        result.Should().Be(0);
        Directory.GetFiles("./TestData/Generated").Should().HaveCount(0);
    }

    [Fact]
    public async Task Program_GivenNonExistentInputDirectory_ShouldReturnErrorCode()
    {
        // Arrange
        var args = new List<string>
        {
            "--input", "this-is-not-a-valid-directory",
            "--project", "./TestData/TestData.csproj",
            "--namespace", "TestNamespace"
        };

        // Act
        var result = await Program.Main(args.ToArray());

        // Assert
        result.Should().Be(1);
    }

    [Theory]
    [InlineData("not-valid")]
    [InlineData(".NotValid")]
    [InlineData("NotValid.")]
    [InlineData("0NotValid")]
    [InlineData("")]
    public async Task Program_GivenInvalidNamespace_ShouldReturnErrorCode(string? invalidNamespace)
    {
        // Arrange
        var args = new List<string?>
        {
            "--input", "./TestData/Schemas",
            "--project", "./TestData/TestData.csproj",
            "--namespace", invalidNamespace
        };

        // Act
        var result = await Program.Main(args.ToArray()!);

        // Assert
        result.Should().Be(1);
    }
}
