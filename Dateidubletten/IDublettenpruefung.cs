namespace Dateidubletten;

public interface IDublettenpruefung
{
    IEnumerable<IDublette> Sammle_Kandidaten(string pfad);
    IEnumerable<IDublette> Sammle_Kandidaten(string pfad, Vergleichsmodi modus);

    IEnumerable<IDublette> Pruefe_Kandidaten(IEnumerable<IDublette> kandidaten);
}