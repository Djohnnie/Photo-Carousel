using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace PhotoCarousel.Browser.Helpers;

public class UpdateHelper
{
    private readonly string _baseUri;

    public UpdateHelper()
    {
        _baseUri = "https://djohnnieke.blob.core.windows.net/photo-carousel";
    }

    public string GetCurrentVersion()
    {
        return $"{Assembly.GetExecutingAssembly().GetName().Version}";
    }

    public async Task<string> GetAvailableVersion()
    {
        using var client = new HttpClient();
        var version = await client.GetStringAsync($"{_baseUri}/PhotoCarousel.Browser.Version.txt");
        return version.Replace("\r\n", "");
    }

    public bool IsAvailableNewer(string currentVersion, string availableVersion)
    {
        long currentCalculatedVersion = GetCalculatedVersion(currentVersion);
        long availableCalculatedVersion = GetCalculatedVersion(availableVersion);

        return currentCalculatedVersion < availableCalculatedVersion;
    }

    public async Task<string> DownloadAvailableVersion(string availableVersion)
    {
        using var client = new HttpClient();
        var fileName = $"PhotoCarousel.Browser.{availableVersion}.Setup.exe";
        var bytes = await client.GetByteArrayAsync($"{_baseUri}/{fileName}");
        var tempPath = Path.Combine(Path.GetTempPath(), fileName);
        await File.WriteAllBytesAsync(tempPath, bytes);

        return tempPath;
    }

    public void InstallUpdate(string updatePath)
    {
        Process.Start(updatePath);
        Process.GetCurrentProcess().Kill();
    }

    private long GetCalculatedVersion(string version)
    {
        long calculatedVersion = 0;
        var splittedVersion = version.Split('.');
        long[] versionMultipliers = { 100000000L, 1000000L, 10000L, 1L };
        for (int i = 0; i < 4; i++)
        {
            calculatedVersion += Convert.ToInt64(splittedVersion[i]) * versionMultipliers[i];
        }

        return calculatedVersion;
    }
}