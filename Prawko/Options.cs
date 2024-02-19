using CommandLine;

public class Options
{
    [Option(HelpText = "Katalog, w którym znajdują się wizualizacje do pytań pobrane z https://www.gov.pl/web/infrastruktura/prawo-jazdy.", Required = true)]
    public string MediaDirectory { get; set; }
    
    [Option(HelpText = "Baza pytań, która została pobrana z https://www.gov.pl/web/infrastruktura/prawo-jazdy.", Required = true)]
    public string DatabasePath { get; set; }
    
    [Option(HelpText = "Nazwa wynikowego pliku .apkg.", Required = true, Default = "Prawo jazdy - kategoria A")]
    public string DeckName { get; set; }
    
    [Option(HelpText = "Kategorie, które nas interesują oddzielone przecinkiem np. A,B", Required = true, Separator = ',', Default = "A")]
    public IEnumerable<string> Categories { get; set; }
    
    [Option(HelpText = "Katalog wynikowy w którym pojawi się plik .apkg i collection.media.")]
    public string OutputDirectory { get; set; }

    public Options()
    {
        OutputDirectory = Directory.GetCurrentDirectory();
        Categories = new List<string>();
    }
}
