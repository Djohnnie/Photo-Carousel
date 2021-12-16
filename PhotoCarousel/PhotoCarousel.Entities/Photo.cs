using System;
using PhotoCarousel.Entities.Enums;

namespace PhotoCarousel.Entities
{
    public class Photo
    {
        public Guid Id { get; set; }
        public int SysId { get; set; }
        public string Description { get; set; }
        public byte[] Sha256Hash { get; set; }
        public string SourcePath { get; set; }
        public string FolderPath { get; set; }
        public string ThumbnailPath { get; set; }
        public Orientation Orientation { get; set; }
        public Rating Rating { get; set; }
        public DateTime DateTaken { get; set; }
        public DateTime DateIndexed { get; set; }
    }
}