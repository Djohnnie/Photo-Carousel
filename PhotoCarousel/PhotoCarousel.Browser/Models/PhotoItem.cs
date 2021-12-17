using System;

namespace PhotoCarousel.Browser.Models
{
    internal class PhotoItem
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public byte[] Bitmap { get; set; }
    }
}