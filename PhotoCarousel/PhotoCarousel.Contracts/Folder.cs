namespace PhotoCarousel.Contracts;

public class Folder
{
    public string Name { get; set; }
    public string FullPath { get; set; }
    public List<Folder> ChildFolders { get; set; }
}