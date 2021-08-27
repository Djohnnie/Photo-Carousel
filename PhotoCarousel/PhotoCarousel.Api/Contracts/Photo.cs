using System;

namespace PhotoCarousel.Api.Contracts
{
    public class Photo
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
    }
}