using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using System.Text;
using System.Threading.Tasks;
using System;

namespace BlazorWithApi.Client.Services
{
    public class AuthenticationService : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;

        public AuthenticationService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            var identity = string.IsNullOrEmpty(token)
                ? new ClaimsIdentity()
                : new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public async Task MarkUserAsAuthenticatedAsync(string token)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
            await _localStorage.SetItemAsync("authToken", token);
        }

        public async Task MarkUserAsLoggedOutAsync()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymousUser)));
            await _localStorage.RemoveItemAsync("authToken");
        }

        private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            if (string.IsNullOrEmpty(jwt)) 
                return new List<Claim>();

            var claims = new List<Claim>();
            var payload = jwt.Split('.');
            if (payload.Length < 2) 
                return claims;
            
            var jsonBytes = ParseBase64WithoutPadding(payload[1]);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            if (keyValuePairs == null) 
                return claims;

            if (keyValuePairs.TryGetValue(ClaimTypes.Role, out var roles) && roles != null)
            {
                var rolesString = roles.ToString();
                if (!string.IsNullOrEmpty(rolesString))
                {
                    if (rolesString.Trim().StartsWith('[') && rolesString.Trim().EndsWith(']'))
                    {
                        var parsedRoles = JsonSerializer.Deserialize<string[]>(rolesString);
                        if (parsedRoles != null)
                        {
                            foreach (var parsedRole in parsedRoles)
                            {
                                if (!string.IsNullOrEmpty(parsedRole))
                                    claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                            }
                        }
                    }
                    else
                    {
                        claims.Add(new Claim(ClaimTypes.Role, rolesString));
                    }
                }
            }

            keyValuePairs.Remove(ClaimTypes.Role);
            
            foreach (var claim in keyValuePairs)
            {
                var value = claim.Value?.ToString();
                if (!string.IsNullOrEmpty(value))
                {
                    claims.Add(new Claim(claim.Key, value));
                }
            }

            return claims;
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
