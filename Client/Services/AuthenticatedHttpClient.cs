using System.Net.Http.Headers;
using Blazored.LocalStorage;

namespace BlazorWithApi.Client.Services
{
    public class AuthenticatedHttpClient : HttpClient
    {
        private readonly ILocalStorageService _localStorage;

        public AuthenticatedHttpClient(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
