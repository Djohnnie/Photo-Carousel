using System;
using System.Text.Json;

namespace PhotoCarousel.Entities;

public class Flag
{
    public Guid Id { get; set; }
    public long SysId { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
}