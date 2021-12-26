using PhotoCarousel.Enums;

namespace PhotoCarousel.Contracts;

public class PhotoRating
{
    public Guid[] PhotoIds { get; set; }
    public Rating Rating { get; set; }
}