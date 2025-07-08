using System.Text.Json;

namespace PhotoCarousel.Contracts;

public class Flag
{
    public string Name { get; set; }
    public string Value { get; set; }
}

public class DisplayPingFlag
{
    public static string Name => "DisplayPingFlag";

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