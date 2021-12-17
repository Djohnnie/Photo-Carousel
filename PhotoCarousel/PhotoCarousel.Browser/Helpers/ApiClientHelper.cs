using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using PhotoCarousel.Contracts;
using RestSharp;

namespace PhotoCarousel.Browser.Helpers;

internal class ApiClientHelper
{
    private readonly RestClient _apiClient;

    public ApiClientHelper()
    {
        _apiClient = new RestClient("http://192.168.10.2:8077");
    }

    public async Task<List<Folder>> GetFolders()
    {
        var request = new RestRequest("folders", Method.GET);
        var response = await _apiClient.ExecuteAsync<List<Folder>>(request);

        return response.Data;
    }

    public async Task<List<Photo>> GetPhotos(string folderPath)
    {
        var request = new RestRequest($"photos/byfolder", Method.GET);
        request.Parameters.Add(new Parameter("folderPath", folderPath, ParameterType.QueryString));
        var response = await _apiClient.ExecuteAsync<List<Photo>>(request);

        return response.Data;
    }

    public async Task<byte[]> GetThumbnail(Guid photoId)
    {
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri("http://192.168.10.2:8077");
        
        var photo = await httpClient.GetByteArrayAsync($"downloads/thumbnail/{photoId}");

        return photo;
    }
}