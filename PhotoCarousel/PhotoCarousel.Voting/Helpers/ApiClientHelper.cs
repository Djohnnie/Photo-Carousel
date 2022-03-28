using PhotoCarousel.Contracts;
using PhotoCarousel.Enums;
using RestSharp;

namespace PhotoCarousel.Voting.Helpers
{
    public class ApiClientHelper
    {
        private readonly string _baseUri;

        public ApiClientHelper()
        {
            _baseUri = "http://192.168.10.2:8077";
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
            var apiClient = new RestClient(_baseUri);
            var request = new RestRequest($"downloads/thumbnail/{photoId}", Method.GET);
            var response = await apiClient.ExecuteAsync(request);

            return response.RawBytes;
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
}