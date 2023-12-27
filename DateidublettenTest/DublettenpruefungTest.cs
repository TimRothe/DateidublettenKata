namespace DateidublettenTest;

public class Tests
{
    private IDublettenpruefung _sut;
    private Mock<IFileReader> _fileReaderMock;
    private Mock<IHashProvider> _hashProviderMock;

    [SetUp]
    public void Setup()
    {
        _fileReaderMock = new Mock<IFileReader>();
        _hashProviderMock = new Mock<IHashProvider>();
        _sut = new Dublettenpruefung(_fileReaderMock.Object, _hashProviderMock.Object);
    }

    [Test]
    public void GetFileNames_with_default_mode_should_return_duplicated_file_paths_for_size_and_name()
    {
        _fileReaderMock.Setup(f => f.GetFileNames(It.IsAny<string>())).Returns([
            new Tuple<string, long, string>("foo", 1L, "foo"),
            new Tuple<string, long, string>("bar", 2L, "bar"),
            new Tuple<string, long, string>("foo", 2L, @"foo\foo"),
            new Tuple<string, long, string>("foo", 1L, @"bar\foo"),
            new Tuple<string, long, string>("foobar", 1L, "foobar")
        ]);

        var result = _sut.Sammle_Kandidaten("foo");

        result.Should().OnlyContain(d => d.Dateipfade.SequenceEqual(new List<string>
        {
            "foo", @"bar\foo"
        }));
    }

    [Test]
    public void GetFileNames_with_Groeße_mode_should_return_duplicated_file_paths_for_size()
    {
        _fileReaderMock.Setup(f => f.GetFileNames(It.IsAny<string>())).Returns([
            new Tuple<string, long, string>("foo", 1L, "foo"),
            new Tuple<string, long, string>("bar", 2L, "bar"),
            new Tuple<string, long, string>("foo", 2L, @"foo\foo"),
            new Tuple<string, long, string>("foo", 1L, @"bar\foo"),
            new Tuple<string, long, string>("foobar", 1L, "foobar")
        ]);

        var result = _sut.Sammle_Kandidaten("foo", Vergleichsmodi.Groeße);

        result.Should().HaveCount(2).And.ContainEquivalentOf(new Dublette
        {
            Dateipfade = new List<string> { "foo", @"bar\foo", "foobar" }
        }).And.ContainEquivalentOf(new Dublette
        {
            Dateipfade = new List<string> { "bar", @"foo\foo" }
        });
    }

    [Test]
    public void Pruefe_Kandidaten_should_return_duplicates()
    {
        _hashProviderMock.Setup(h => h.GetHash("foo")).Returns("1");
        _hashProviderMock.Setup(h => h.GetHash(@"foo\foo")).Returns("1");
        _hashProviderMock.Setup(h => h.GetHash(@"bar\foo")).Returns("2");

        var result = _sut.Pruefe_Kandidaten(new List<IDublette>
        {
            new Dublette
            {
                Dateipfade = new List<string> { "foo", @"foo\foo", @"bar\foo" }
            }
        });

        result.Should().ContainEquivalentOf(new Dublette
        {
            Dateipfade = new List<string> { "foo", @"foo\foo" }
        });
    }

    [Test]
    public void Pruefe_Kandidaten_should_return_empty_list_for_empty_input()
    {
        var result = _sut.Pruefe_Kandidaten(new List<IDublette>
        {
            new Dublette
            {
                Dateipfade = new List<string>()
            }
        });

        result.Should().BeEmpty();
    }

    [Test]
    public void Dublettenpruefung_should_find_duplicates()
    {
        var sut = new Dublettenpruefung(new FileReader(), new Md5HashProvider(new FileReader()));

        var candidates = sut.Sammle_Kandidaten(@"..\net8.0\resources");
        var duplicates = sut.Pruefe_Kandidaten(candidates);

        duplicates.Should().OnlyContain(d => d.Dateipfade.SequenceEqual(new List<string>
        {
            @"..\net8.0\resources\foo.txt", @"..\net8.0\resources\foo\foo.txt"
        }));
    }
}