using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorWithApi.Client;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using BlazorWithApi.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add services to the container.
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AuthenticationStateProvider, AuthenticationService>();
builder.Services.AddAuthorizationCore();

// Use relative URLs since we're hosting with the server
builder.Services.AddScoped(sp => 
{
    // Create HttpClient with base address pointing to the server
    var httpClient = new HttpClient 
    { 
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) 
    };
    return httpClient;
});

// Register our custom authenticated HttpClient
builder.Services.AddScoped<AuthenticatedHttpClient>(sp =>
{
    var localStorage = sp.GetRequiredService<ILocalStorageService>();
    return new AuthenticatedHttpClient(localStorage)
    {
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
    };
});

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

await builder.Build().RunAsync();
