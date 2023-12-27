using System.Security.Cryptography;

namespace Dateidubletten;

public interface IHashProvider
{
    string GetHash(string path);
}

public class Md5HashProvider : IHashProvider
{
    public Md5HashProvider(IFileReader fileReader)
    {
        FileReader = fileReader;
    }
    private IFileReader FileReader { get; }

    public string GetHash(string path)
    {
        using var fileStream = FileReader.GetFile(path);
        using var md5 = MD5.Create();
        return BitConverter.ToString(md5.ComputeHash(fileStream));
    }

   
}