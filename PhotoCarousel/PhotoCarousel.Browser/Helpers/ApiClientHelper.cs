using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using PhotoCarousel.Contracts;
using PhotoCarousel.Enums;
using RestSharp;

namespace PhotoCarousel.Browser.Helpers;

internal class ApiClientHelper
{
    private readonly string _baseUri;


    public ApiClientHelper()
    {
        _baseUri = "https://api.djohnnie.be:8077";
    }

    public async Task<List<Folder>> GetFolders()
    {
        var apiClient = new RestClient(_baseUri);
        var request = new RestRequest("folders", Method.GET);
        var response = await apiClient.ExecuteAsync<List<Folder>>(request);

        return response.Data;
    }

    public async Task<List<Photo>> GetPhotos(string folderPath)
    {
        var apiClient = new RestClient(_baseUri);
        var request = new RestRequest($"photos/byfolder", Method.GET);
        request.Parameters.Add(new Parameter("folderPath", folderPath, ParameterType.QueryString));
        var response = await apiClient.ExecuteAsync<List<Photo>>(request);

        return response.Data;
    }

    public async Task<Photo> GetPreviousPhoto()
    {
        var apiClient = new RestClient(_baseUri);
        var request = new RestRequest($"photos/previous", Method.GET);
        var response = await apiClient.ExecuteAsync<Photo>(request);

        return response.Data;
    }

    public async Task<Photo> GetCurrentPhoto()
    {
        var apiClient = new RestClient(_baseUri);
        var request = new RestRequest($"photos/current", Method.GET);
        var response = await apiClient.ExecuteAsync<Photo>(request);

        return response.Data;
    }

    public async Task<Photo> GetNextPhoto()
    {
        var apiClient = new RestClient(_baseUri);
        var request = new RestRequest($"photos/next", Method.GET);
        var response = await apiClient.ExecuteAsync<Photo>(request);

        return response.Data;
    }

    public async Task<byte[]> GetThumbnail(Guid photoId)
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(_baseUri);

        var photo = await httpClient.GetByteArrayAsync($"downloads/thumbnail/{photoId}");

        return photo;
    }

    public async Task SetRating(Guid photoId, Rating rating)
    {
        var apiClient = new RestClient(_baseUri);
        var request = new RestRequest($"ratings", Method.POST);
        request.AddJsonBody(new PhotoRating { PhotoIds = new[] { photoId }, Rating = rating });
        var response = await apiClient.ExecuteAsync(request);
    }

    public async Task SetRating(Guid[] photoIds, Rating rating)
    {
        var apiClient = new RestClient(_baseUri);
        var request = new RestRequest($"ratings", Method.POST);
        request.AddJsonBody(new PhotoRating { PhotoIds = photoIds, Rating = rating });
        var response = await apiClient.ExecuteAsync(request);
    }
}