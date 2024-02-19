namespace Prawko;

public class Question
{
    public int Id { get; set; }

    public string Value { get; set; } = null!;

    public string? AnswerA { get; set; }
    public string? AnswerB { get; set; }
    public string? AnswerC { get; set; }

    public string Answer { get; set; } = null!;

    public IReadOnlyList<string> Categories { get; set; } = Array.Empty<string>();

    public string? MediaName { get; set; }
}