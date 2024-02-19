using Prawko;

const string dataPath = @"C:\Users\Damian\Desktop\data";

var skipLines = 1;
var resourcesPath = Path.Combine(dataPath, "resources");
var databasePath = Path.Combine(dataPath, "database.tsv");

var lineNo = 1;

var lines = await File.ReadAllLinesAsync(databasePath);
var items = lines
    .Skip(skipLines)
    .Select(line =>
    {
        try
        {
            var values = line.Split("\t");

            var item = new Item
            {
                QuestionId = int.Parse(values[1]),
                Question = values[2],
                AnswerA = values[3],
                AnswerB = values[4],
                AnswerC = values[5],
                Answer = values[6],
                MediaName = values[7],
                Category = values[8].Split(',')
            };

            return item;
        }
        catch (Exception)
        {
            Console.WriteLine($"Unable to parse line: {line}");
            return null;
        }
        finally
        {
            lineNo++;
        }
    }).Where(x => x != null).ToArray();

var missing = GetMissingResources(items!, resourcesPath);
Console.WriteLine($"Found missing files {missing.Count}!! These will be skipped.");

var exports = new List<(string DeckName, IReadOnlyCollection<string> Category, string FileName)>
{
    new("Prawo jazdy - kategoria B - mp4", new[] { Category.B }, "deck_category_b_mp4.apkg"),
    new("Prawo jazdy - kategoria A - mp4", new[] { Category.A }, "deck_category_a_mp4.apkg")
};

var missingQuestionIds = missing
    .Select(x => x.QuestionId)
    .ToHashSet();

foreach (var (deckName, category, fileName) in exports)
{
    var selectedItems = items
        .Where(x => x.Category.Any(y => category.Contains(y)))
        .Where(x => missingQuestionIds.Contains(x.QuestionId) is false)
        .ToArray();

    var converter = new ItemToAnkiConverter(deckName);
    await using var ms = await converter.Convert(selectedItems);
    await using var fileStream = new FileStream(fileName, FileMode.Create);
    await ms.CopyToAsync(fileStream);

    var resDir = $"{deckName}_collection.media";

    
    var res = selectedItems.Where(x => !string.IsNullOrEmpty(x.MediaName)).Select(x => new
    {
        Src = Path.Combine(resourcesPath, x.MediaName),
        Dst = Path.Combine(resDir, x.MediaName)
    }).ToArray();

    if (!Directory.Exists(resDir))
    {
        Directory.CreateDirectory(resDir);
    }
    
    foreach (var item in res)
    {
        File.Copy(item.Src, item.Dst, true);
    }
    
    Console.WriteLine($"Exported {deckName} | {selectedItems.Length}/{items.Length} | TotalLines: {lines.Length}");
}

Console.WriteLine("Done");

static IReadOnlyCollection<Item> GetMissingResources(IReadOnlyCollection<Item> items, string resourcesPath)
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