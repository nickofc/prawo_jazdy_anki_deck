using CommandLine;
using Prawko;

await Parser.Default.ParseArguments<Options>(args)
    .WithParsedAsync(Start);

static async Task Start(Options options)
{
    var xlsxParser = new XlsxParser();
    var questions = xlsxParser.Parse(options.DatabasePath);

    var totalQuestionCount = questions.Count;

    questions = DeleteQuestionsWithNoResources(questions, options.MediaDirectory);

    var apkgFullPath = Path.Combine(options.OutputDirectory, $"{options.DeckName}.apkg");
    var apkgResourceDictionary = Path.Combine(options.OutputDirectory, $"{options.DeckName}_collection.media");

    Console.WriteLine($"Exporting {options.DeckName} questions to {apkgFullPath}...");

    var selectedQuestions = questions
        .Where(x => x.Categories.Any(y => options.Categories.Contains(y)))
        .ToArray();

    var converter = new ItemToAnkiConverter(options.DeckName);
    await using var stream = await converter.Convert(selectedQuestions);
    await using var fileStream = new FileStream(apkgFullPath, FileMode.Create);
    await stream.CopyToAsync(fileStream);

    var resources = selectedQuestions
        .Where(x => !string.IsNullOrEmpty(x.MediaName))
        .Select(x => new
        {
            Source = Path.Combine(options.MediaDirectory, x.MediaName),
            Destination = Path.Combine(apkgResourceDictionary, x.MediaName)
        })
        .ToArray();

    if (!Directory.Exists(apkgResourceDictionary))
    {
        Directory.CreateDirectory(apkgResourceDictionary);
    }

    Console.WriteLine($"Exporting {options.DeckName} media to {apkgResourceDictionary}...");

    foreach (var resource in resources)
    {
        File.Copy(resource.Source, resource.Destination, true);
    }

    Console.WriteLine($"Exported {selectedQuestions.Length} questions out of {totalQuestionCount}.");
    Console.WriteLine("Exporting completed.");
}

static IReadOnlyCollection<Question> DeleteQuestionsWithNoResources(
    IReadOnlyCollection<Question> questions,
    string resourcesPath)
{
    var questionWithNoResources = GetQuestionWithNoResources(questions, resourcesPath);

    if (questionWithNoResources.Count > 0)
    {
        Console.WriteLine(
            $"{questionWithNoResources.Count} questions with missing resources were found. These question will be skipped. ");

        var questionWithNoResourcesIds = questionWithNoResources
            .Select(x => x.Id)
            .ToHashSet();

        questions = questions
            .Where(x => questionWithNoResourcesIds.Contains(x.Id) is false)
            .ToArray();
    }

    return questions;
}

static IReadOnlyCollection<Question> GetQuestionWithNoResources(
    IReadOnlyCollection<Question> items,
    string resourcesPath)
{
    var resourceNames = items
        .Where(x => string.IsNullOrEmpty(x.MediaName) is false)
        .ToArray();

    var files = Directory
        .GetFiles(resourcesPath, "*")
        .Select(Path.GetFileName)
        .ToHashSet();

    return resourceNames
        .Where(x => files.Contains(x.MediaName) is false)
        .ToList();
}