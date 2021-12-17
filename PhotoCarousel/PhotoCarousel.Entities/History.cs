using System;

namespace PhotoCarousel.Entities;

public class History
{
    public Guid Id { get; set; }
    public int SysId { get; set; }
    public Guid PhotoId { get; set; }
    public DateTime Scheduled { get; set; }
}