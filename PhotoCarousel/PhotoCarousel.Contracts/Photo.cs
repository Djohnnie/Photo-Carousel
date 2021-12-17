using PhotoCarousel.Enums;

namespace PhotoCarousel.Contracts;

public class Photo
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public Rating Rating { get; set; }
}