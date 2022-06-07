namespace PhotoCarousel.Contracts;

public class Duplicates
{
    public byte[] Sha256Hash { get; set; }

    public List<DuplicatePhoto> Photos { get; set; }
}

public class DuplicatePhoto
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public string SourcePath { get; set; }
}