using System.CommandLine;
using System.Text;
using System.Text.RegularExpressions;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;

namespace Abs.CommonCore.Drex.Contracts.ModelGenerator;

/*
 Example parameters:

--input "../../../../../json"
--project "../../../../../tooling/Contracts/Contracts.csproj"
--namespace "Abs.CommonCore.Contracts.Json"

--input "../../../../../proto"
--project "../../../../../tooling/Contracts/Contracts.csproj"
 */

public static partial class Program
{
    public static async Task<int> Main(string[]? args)
    {
        var root = BuildRootCommand();
        return await root.InvokeAsync(args ?? Array.Empty<string>());
    }

    private static RootCommand BuildRootCommand()
    {
        var inputDirectoryParam = new Option<DirectoryInfo>("--input", "Input directory containing schema files")
        {
            IsRequired = true
        };
        inputDirectoryParam.AddValidator(result =>
        {
            var parameterValue = result.GetValueForOption(inputDirectoryParam);
            if (parameterValue?.Exists != true)
            {
                result.ErrorMessage = $"Input directory ({parameterValue?.FullName}) does not exist";
            }
        });

        inputDirectoryParam.AddAlias("-i");

        var projectFileDirectoryParam = new Option<FileInfo>("--project", "Contracts project file")
        {
            IsRequired = true
        };
        projectFileDirectoryParam.AddValidator(result =>
        {
            var parameterValue = result.GetValueForOption(projectFileDirectoryParam);
            if (parameterValue?.Exists != true)
            {
                result.ErrorMessage = $"Contracts project file ({parameterValue?.FullName}) does not exist";
            }
        });
        projectFileDirectoryParam.AddAlias("-p");

        var namespaceParam = new Option<string>("--namespace", "Namespace for generated models")
        {
            IsRequired = true
        };
        namespaceParam.AddValidator(result =>
        {
            var regex = NamespaceValidationRegex(); // Matches valid C# namespaces (e.g. This.Is.A.Valid.Namespace)
            var parameterValue = result.GetValueForOption(namespaceParam);
            if (string.IsNullOrWhiteSpace(parameterValue) || !regex.IsMatch(parameterValue))
            {
                result.ErrorMessage = "Valid namespace is required";
            }
        });
        namespaceParam.AddAlias("-n");

        var jsonCommand = new Command("json", "Generate C# models from JSON-Schema files");
        jsonCommand.AddAlias("j");
        jsonCommand.AddOption(inputDirectoryParam);
        jsonCommand.AddOption(projectFileDirectoryParam);
        jsonCommand.AddOption(namespaceParam);
        jsonCommand.SetHandler(GenerateJsonSchemaModels, inputDirectoryParam, projectFileDirectoryParam, namespaceParam);

        var protoCommand = new Command("proto", "Generate C# models from Protobuf files");
        protoCommand.AddAlias("p");
        protoCommand.AddOption(inputDirectoryParam);
        protoCommand.AddOption(projectFileDirectoryParam);
        protoCommand.SetHandler(GenerateProtoSchemaModels, inputDirectoryParam, projectFileDirectoryParam);

        var root = new RootCommand("Generator that converts JSON-Schema and Protobuf files to C# POCO models. Root command will use JSON-Schema generator by default.")
        {
            TreatUnmatchedTokensAsErrors = true
        };
        root.Add(inputDirectoryParam);
        root.Add(projectFileDirectoryParam);
        root.Add(namespaceParam);
        root.SetHandler(GenerateJsonSchemaModels, inputDirectoryParam, projectFileDirectoryParam, namespaceParam);

        root.AddCommand(protoCommand);

        return root;
    }

    private static async Task GenerateJsonSchemaModels(
        DirectoryInfo inputDirectory,
        FileInfo contractsProjectFile,
        string namespaceName)
    {
        try
        {
            Console.WriteLine("Generating JSON-Schema C# models");

            const string jsonSchemaFileSuffix = ".schema.json";
            Console.WriteLine($"Input Directory: {inputDirectory.FullName}");
            Console.WriteLine($"Contracts Project File: {contractsProjectFile.FullName}");
            Console.WriteLine($"Namespace: {namespaceName}");

            var outputDirectoryPath = Path.Combine(contractsProjectFile.Directory!.FullName, "Generated");
            var outputDirectory = new DirectoryInfo(outputDirectoryPath);
            outputDirectory.EnsureBlankSlate();

            var allDirectories = inputDirectory.GetDirectories().ToList();
            allDirectories.Add(inputDirectory);
            foreach (var directory in allDirectories)
            {
                var completeNamespace = namespaceName;
                if (directory != inputDirectory)
                {
                    var namespacePart = directory.Name;
                    namespacePart = $"{namespacePart[0].ToString().ToUpper()}{namespacePart[1..]}";
                    completeNamespace = $"{completeNamespace}.{namespacePart}";
                }

                var settings = new CSharpGeneratorSettings
                {
                    Namespace = completeNamespace,
                    ArrayType = "List",
                    DictionaryType = "Dictionary",
                    ArrayInstanceType = "List",
                    JsonLibrary = CSharpJsonLibrary.SystemTextJson,
                    TypeNameGenerator = new CustomTypeNameGenerator(),
                };

                var schemaFileNames = directory
                    .GetFiles($"*{jsonSchemaFileSuffix}")
                    .Select(_ => _.FullName)
                    .ToList();
                if (!schemaFileNames.Any())
                {
                    Console.WriteLine($"No schema files found in input directory \"{inputDirectory.FullName}\"");
                }

                foreach (var schemaFileName in schemaFileNames)
                {
                    try
                    {
                        var schema = await JsonSchema.FromFileAsync(schemaFileName);
                        var generator = new CSharpGenerator(schema, settings);
                        var generatedModelClassText = generator.GenerateFile();

                        var schemaText = await File.ReadAllTextAsync(schemaFileName);
                        var expectedClassStartText = $"public partial class {schema.Title}";
                        var inlineSchemaText = schemaText.Replace("\r\n", "").Replace("\"", "\\\"");
                        var attributeTextToAdd = $"[JsonSchema(\"{inlineSchemaText}\")]";
                        generatedModelClassText = generatedModelClassText
                            .Replace(expectedClassStartText, $"{attributeTextToAdd}\n\t{expectedClassStartText}")
                            .Replace(
                                "[System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]",
                                "[System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumMemberConverter))]");

                        var fileName = $"{schema.Title}.cs";

                        var outputFileName = Path.Combine(outputDirectory.FullName, fileName);

                        var currentText = File.Exists(outputFileName)
                            ? File.ReadAllText(outputFileName)
                            : "";

                        if (currentText != generatedModelClassText)
                        {
                            await File.WriteAllTextAsync(outputFileName, generatedModelClassText);
                            Console.WriteLine($"Generated model \"{completeNamespace}.{schema.Title}\" in C# file \"{outputFileName}\" from schema file \"{schemaFileName}\"");
                        }
                        else
                        {
                            Console.WriteLine($"No changes to {outputFileName}");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Unable to build file for schema '{schemaFileName}'", ex);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            throw;
        }
    }

    private static async Task GenerateProtoSchemaModels(DirectoryInfo inputDirectory, FileInfo contractsProjectFile)
    {
        try
        {
            Console.WriteLine("Generating Protobuf C# models");

            const string protobufDirectoryName = "Protobuf";
            var outputDirectoryPath = Path.Combine(contractsProjectFile.Directory!.FullName, protobufDirectoryName);
            var outputDirectory = new DirectoryInfo(outputDirectoryPath);
            outputDirectory.EnsureBlankSlate();

            var allDirectories = inputDirectory.GetDirectories("*", SearchOption.AllDirectories);
            foreach (var directory in allDirectories)
            {
                var directoryToCreate = directory.FullName.Replace(inputDirectory.FullName, outputDirectoryPath);
                Directory.CreateDirectory(directoryToCreate);
            }

            var allFiles = inputDirectory.GetFiles("*", SearchOption.AllDirectories);
            var protoItemGroupStringBuilder = new StringBuilder();
            foreach (var file in allFiles)
            {
                var protoPathRelativeToProjectFile = file.FullName.Replace(inputDirectory.FullName, protobufDirectoryName);
                var destinationFileName = file.FullName.Replace(inputDirectory.FullName, outputDirectoryPath);
                File.Copy(file.FullName, destinationFileName, true);
                protoItemGroupStringBuilder.AppendLine($"\t\t<Protobuf Include=\"{protoPathRelativeToProjectFile}\" AdditionalImportDirs=\"Protobuf\" />");
            }

            var contractsProjectFileText = File.ReadAllText(contractsProjectFile.FullName);

            const string blankProtobufItemGroupText = "<ItemGroup Label=\"Protobuf\"></ItemGroup>";
            var regex = ProtobufRegex();
            var match = regex.Match(contractsProjectFileText);
            if (match.Success)
            {
                // Gut the existing Protobuf ItemGroup
                contractsProjectFileText = contractsProjectFileText.Replace(match.Value, blankProtobufItemGroupText);
            }
            else
            {
                // Add the blank Protobuf ItemGroup to the end of the file
                contractsProjectFileText = contractsProjectFileText.Replace("</Project>", $"{blankProtobufItemGroupText}\n</Project>");
            }

            var updatedProtoBlock = $"<ItemGroup Label=\"Protobuf\">\n{protoItemGroupStringBuilder}\t</ItemGroup>";
            var updatedContractsProjectFileText = contractsProjectFileText.Replace(blankProtobufItemGroupText, updatedProtoBlock);

            var currentText = File.Exists(contractsProjectFile.FullName)
                ? File.ReadAllText(contractsProjectFile.FullName)
                : "";

            if (currentText != updatedContractsProjectFileText)
            {
                await File.WriteAllTextAsync(contractsProjectFile.FullName, updatedContractsProjectFileText);
                Console.WriteLine($"Updated the Protobuf reference block to {contractsProjectFile.FullName}:\n{updatedProtoBlock}");
            }
            else
            {
                Console.WriteLine("No changes to Protobuf reference block");
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            throw;
        }
    }

    /// <summary>
    /// Ensure a blank version of the directory exists (delete contents if present)
    /// </summary>
    /// <param name="directory"></param>
    private static void EnsureBlankSlate(this DirectoryInfo directory)
    {
        if (directory.Exists == false)
        {
            directory.Create();
            return;
        }

        foreach (var file in directory.EnumerateFiles())
        {
            File.Delete(file.FullName);
        }
    }

    [GeneratedRegex("^(@?[a-z_A-Z]\\w+(?:\\.@?[a-z_A-Z]\\w+)*)$")]
    private static partial Regex NamespaceValidationRegex();
    [GeneratedRegex("<ItemGroup Label=\"Protobuf\">[\\s\\S]*</ItemGroup>")]
    private static partial Regex ProtobufRegex();
}
