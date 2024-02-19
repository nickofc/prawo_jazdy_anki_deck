using System.Text;
using System.Text.Json;
using AnkiNet;

namespace Prawko;

public class ItemToAnkiConverter
{
    private readonly string _deckName;

    public ItemToAnkiConverter(string deckName)
    {
        _deckName = deckName;
    }

    public async Task<Stream> Convert(IReadOnlyList<Question> questions)
    {
        var collection = new AnkiCollection();

        var noteType = GetAnkiNoteType();

        var noteTypeId = collection.CreateNoteType(noteType);
        var deckId = collection.CreateDeck(_deckName);
        
        foreach (var question in questions)
        {
            var (front, back) = Convert(question);
            collection.CreateNote(deckId, noteTypeId, front, back);
        }

        var memoryStream = new MemoryStream();
        await AnkiFileWriter.WriteToStreamAsync(memoryStream, collection);
        memoryStream.Position = 0;
        return memoryStream;
    }
    
    private AnkiNoteType GetAnkiNoteType()
    {
        return new AnkiNoteType(
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
    }

    private (string front, string back) Convert(Question question)
    {
        var sb = new StringBuilder();

        sb.AppendLine("<div>");
        sb.AppendLine($"<h2>{question.Value}</h2>");

        if (!string.IsNullOrEmpty(question.MediaName))
        {
            var isImage = question.MediaName.EndsWith(".jpg", StringComparison.InvariantCultureIgnoreCase);
            var isVideo = question.MediaName.EndsWith(".wmv", StringComparison.InvariantCultureIgnoreCase);

            if (isImage)
            {
                sb.AppendLine($"<img src=\"{question.MediaName}\">");
            }
            else if (isVideo)
            {
                sb.AppendLine($"[sound:{question.MediaName.Replace(".wmv", 
                    ".mp4", StringComparison.InvariantCultureIgnoreCase)}]");
            }
        }

        // TODO: testy ABC powinny zmieniać kolejność!
        
        if (!string.IsNullOrEmpty(question.AnswerA))
        {
            sb.AppendLine($"<p><strong>A</strong>: {question.AnswerA}</p>");
        }

        if (!string.IsNullOrEmpty(question.AnswerB))
        {
            sb.AppendLine($"<p><strong>B</strong>: {question.AnswerB}</p>");
        }

        if (!string.IsNullOrEmpty(question.AnswerC))
        {
            sb.AppendLine($"<p><strong>C</strong>: {question.AnswerC}</p>");
        }

        sb.AppendLine("<div style=\"display:none\">");
        sb.AppendLine(JsonSerializer.Serialize(question));
        sb.AppendLine("</div>");

        sb.AppendLine("</div>");

        return (sb.ToString(), question.Answer);
    }
}