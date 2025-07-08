using System.Text.Json;

namespace PhotoCarousel.Contracts;

public interface IFlag
{
    string Serialize();
}

public class Flag
{
    public string Name { get; set; }
    public string Value { get; set; }
}

public class DisplayPingFlag : IFlag
{
    public const string Name = "DisplayPingFlag";

    public static DisplayPingFlag Default => new DisplayPingFlag();

    public DateTime LastPing { get; set; }

    public string Serialize()
    {
        return JsonSerializer.Serialize(this);
    }

    public static DisplayPingFlag Deserialize(string json)
    {
        return JsonSerializer.Deserialize<DisplayPingFlag>(json);
    }
}