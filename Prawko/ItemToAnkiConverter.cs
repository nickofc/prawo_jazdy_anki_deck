using AnkiNet;

namespace Prawko;

public class ItemToAnkiConverter
{
    private readonly string _deckName;

    public ItemToAnkiConverter(string deckName)
    {
        _deckName = deckName;
    }

    public async Task<Stream> Convert(IReadOnlyList<Item> items)
    {
        var collection = new AnkiCollection();

        var noteType = new AnkiNoteType(
            name: $"Basic_{_deckName}",
            cardTypes:
            [
                new AnkiCardType(
                    Name: "Card 1",
                    Ordinal: 0,
                    QuestionFormat: "{{Front}}",
                    AnswerFormat: "{{Front}}<hr id=\"answer\">{{Back}}"
                )
            ],
            fieldNames: ["Front", "Back"]
        );

        var noteTypeId = collection.CreateNoteType(noteType);
        var deckId = collection.CreateDeck(_deckName);

        var converter = new ItemToHtmlConverter();

        foreach (var item in items)
        {
            var front = converter.Convert(item);
            var back = item.Answer;
            
            collection.CreateNote(deckId, noteTypeId, front, back);
        }

        var memoryStream = new MemoryStream();
        await AnkiFileWriter.WriteToStreamAsync(memoryStream, collection);
        memoryStream.Position = 0;
        return memoryStream;
    }
}