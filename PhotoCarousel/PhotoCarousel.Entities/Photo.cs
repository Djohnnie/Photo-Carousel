using System;
using PhotoCarousel.Entities.Enums;

namespace PhotoCarousel.Entities
{
    public class Photo
    {
        public Guid Id { get; set; }
        public int SysId { get; set; }
        public string Description { get; set; }
        public string SourcePath { get; set; }
        public string ThumbnailPath { get; set; }
        public Orientation Orientation { get; set; }
        public int ThumbsUp { get; set; }
        public int ThumbsDown { get; set; }
        public DateTime DateTaken { get; set; }
    }
}