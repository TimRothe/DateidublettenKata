namespace Dateidubletten;

public interface IFileReader
{
    IEnumerable<Tuple<string, long, string>> GetFileNames(string path);
    FileStream GetFile(string path);
}

public class FileReader : IFileReader
{
    public IEnumerable<Tuple<string, long, string>> GetFileNames(string path) => Directory
        .GetFiles(path, "*", SearchOption.AllDirectories)
        .Select(p =>
        {
            var fileInfo = new FileInfo(p);
            return new Tuple<string, long, string>(fileInfo.Name, fileInfo.Length, p);
        });

    public FileStream GetFile(string path)
    {
        return File.OpenRead(path);
    }
}