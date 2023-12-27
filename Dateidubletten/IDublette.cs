namespace Dateidubletten;

public interface IDublette
{
    IEnumerable<string> Dateipfade { get; }
}

public class Dublette : IDublette
{
    public IEnumerable<string> Dateipfade { get; init; }
}