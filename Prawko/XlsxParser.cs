using OfficeOpenXml;

namespace Prawko;

public class XlsxParser
{
    public IReadOnlyCollection<Question> Parse(string path)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var xlPackage = new ExcelPackage(new FileInfo(path));

        var worksheet = xlPackage.Workbook.Worksheets.First();
        var totalRows = worksheet.Dimension.End.Row;
        var totalColumns = worksheet.Dimension.End.Column;

        var output = new List<Question>();

        for (var rowNum = 2; rowNum <= totalRows; rowNum++)
        {
            var row = worksheet.Cells[rowNum, 1, rowNum, totalColumns]
                .Select(x => x.Value == null ? string.Empty : x.Value.ToString())
                .ToArray();

            try
            {
                var question = new Question
                {
                    Id = int.Parse(row[1]!),
                    Value = row[2]!,
                    AnswerA = row[3],
                    AnswerB = row[4],
                    AnswerC = row[5],
                    Answer = row[6]!,
                    MediaName = row[7],
                    Categories = row[8]!.Split(',')
                };

                output.Add(question);
            }
            catch (Exception)
            {
                Console.Error.WriteLine($"Unable to parse question.\n" +
                                        $"Current row num: {rowNum}, current row: {row}");
                throw;
            }
        }

        return output;
    }
}