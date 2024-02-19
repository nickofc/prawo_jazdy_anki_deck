namespace Prawko;

public record Item
{
    public int QuestionId { get; set; }

    public string Question { get; set; } = null!;

    public string? AnswerA { get; set; }
    public string? AnswerB { get; set; }
    public string? AnswerC { get; set; }

    public string Answer { get; set; } = null!;

    public IReadOnlyList<string> Category { get; set; } = Array.Empty<string>();

    public string? MediaName { get; set; }
}

public static class Category
{
    public static readonly string A = "A";
    public static readonly string B = "B";
    public static readonly string C = "C";
    public static readonly string D = "D";
    public static readonly string T = "T";
    public static readonly string AM = "AM";
    public static readonly string A1 = "A1";
    public static readonly string A2 = "A2";
    public static readonly string B1 = "B1";
    public static readonly string C1 = "C1";
    public static readonly string D1 = "D1";
}