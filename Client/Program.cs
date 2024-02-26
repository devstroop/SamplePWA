using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using SamplePWA.Client;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddRadzenComponents();
builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<SamplePWA.Client.SampleDBService>();
builder.Services.AddAuthorizationCore();
builder.Services.AddHttpClient("SamplePWA.Server", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("SamplePWA.Server"));
builder.Services.AddScoped<SamplePWA.Client.SecurityService>();
builder.Services.AddScoped<AuthenticationStateProvider, SamplePWA.Client.ApplicationAuthenticationStateProvider>();
var host = builder.Build();
await host.RunAsync();