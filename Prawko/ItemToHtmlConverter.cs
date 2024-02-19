using System.Text;
using System.Text.Json;
using Prawko;

public class ItemToHtmlConverter
{
    private readonly bool _randomizeAnswerOrder;

    public ItemToHtmlConverter(bool randomizeAnswerOrder)
    {
        _randomizeAnswerOrder = randomizeAnswerOrder;
    }

    public (string front, string back) Convert(Item item)
    {
        var sb = new StringBuilder();

        sb.AppendLine("<div>");
        sb.AppendLine($"<h2>{item.Question}</h2>");

        if (!string.IsNullOrEmpty(item.MediaName))
        {
            var isImage = item.MediaName.EndsWith(".jpg", StringComparison.InvariantCultureIgnoreCase);
            var isVideo = item.MediaName.EndsWith(".wmv", StringComparison.InvariantCultureIgnoreCase);

            if (isImage)
            {
                sb.AppendLine($"<img src=\"{item.MediaName}\">");
            } 
            else if (isVideo)
            {
                sb.AppendLine($"[sound:{item.MediaName.Replace(".wmv", ".mp4", StringComparison.InvariantCultureIgnoreCase)}]");
            }
        }

        if (_randomizeAnswerOrder)
        {
            var answers = new List<(string answer, bool isValid)>();

            if (!string.IsNullOrEmpty(item.AnswerA))
            {
                answers.Add(new ValueTuple<string, bool>(item.AnswerA, item.AnswerA ==  item.Answer));    
            }
            
            if (!string.IsNullOrEmpty(item.AnswerB))
            {
                answers.Add(new ValueTuple<string, bool>(item.AnswerB, item.AnswerB == item.Answer));    
            }
            
            if (!string.IsNullOrEmpty(item.AnswerC))
            {
                answers.Add(new ValueTuple<string, bool>(item.AnswerC, item.AnswerC == item.Answer));    
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(item.AnswerA))
            {
                sb.AppendLine($"<p><strong>A</strong>: {item.AnswerA}</p>");
            }

            if (!string.IsNullOrEmpty(item.AnswerB))
            {
                sb.AppendLine($"<p><strong>B</strong>: {item.AnswerB}</p>");
            }

            if (!string.IsNullOrEmpty(item.AnswerC))
            {
                sb.AppendLine($"<p><strong>C</strong>: {item.AnswerC}</p>");
            }
        }
        
        sb.AppendLine("<div style=\"display:none\">");
        sb.AppendLine(JsonSerializer.Serialize(item));
        sb.AppendLine("</div>");

        sb.AppendLine("</div>");

        return (sb.ToString(), "");
    }
}