using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace Dateidubletten;

public class Dublettenpruefung : IDublettenpruefung
{
    public Dublettenpruefung(IFileReader fileReader, IHashProvider hashProvider)
    {
        FileReader = fileReader;
        HashProvider = hashProvider;
    }

    private IFileReader FileReader { get; }
    private IHashProvider HashProvider { get; }

    public IEnumerable<IDublette> Sammle_Kandidaten(string pfad) => Sammle_Kandidaten(pfad, Vergleichsmodi.Groeße_und_Name);

    public IEnumerable<IDublette> Sammle_Kandidaten(string pfad, Vergleichsmodi modus)
    {
        return FileReader.GetFileNames(pfad).GroupBy(t => new Tuple<string, long>(modus == Vergleichsmodi.Groeße_und_Name ? t.Item1 : "", t.Item2))
            .Where(g => g.Count() > 1)
            .Select(g => new Dublette { Dateipfade = g.Select(t => t.Item3) });
    }

    public IEnumerable<IDublette> Pruefe_Kandidaten(IEnumerable<IDublette> kandidaten)
    {
        return kandidaten.SelectMany(d => d.Dateipfade.GroupBy(path => HashProvider.GetHash(path)))
            .Where(g => g.Count() > 1)
            .Select(g => new Dublette { Dateipfade = g.ToList() });
    }
}