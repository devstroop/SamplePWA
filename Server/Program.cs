using Radzen;
using SamplePWA.Server.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using Microsoft.AspNetCore.OData;
using SamplePWA.Server.Data;
using Microsoft.AspNetCore.Identity;
using SamplePWA.Server.Models;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents().AddHubOptions(options => options.MaximumReceiveMessageSize = 10 * 1024 * 1024).AddInteractiveWebAssemblyComponents();
builder.Services.AddControllers();
builder.Services.AddRadzenComponents();
builder.Services.AddHttpClient();
builder.Services.AddScoped<SamplePWA.Server.SampleDBService>();
builder.Services.AddDbContext<SamplePWA.Server.Data.SampleDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SampleDBConnection"));
});
builder.Services.AddControllers().AddOData(opt =>
{
    var oDataBuilderSampleDB = new ODataConventionModelBuilder();
    oDataBuilderSampleDB.EntitySet<SamplePWA.Server.Models.SampleDB.Customer>("Customers");
    oDataBuilderSampleDB.EntitySet<SamplePWA.Server.Models.SampleDB.Employee>("Employees");
    oDataBuilderSampleDB.EntitySet<SamplePWA.Server.Models.SampleDB.Payment>("Payments");
    oDataBuilderSampleDB.EntitySet<SamplePWA.Server.Models.SampleDB.Product>("Products");
    oDataBuilderSampleDB.EntitySet<SamplePWA.Server.Models.SampleDB.ProductCategory>("ProductCategories");
    oDataBuilderSampleDB.EntitySet<SamplePWA.Server.Models.SampleDB.PurchaseOrder>("PurchaseOrders");
    oDataBuilderSampleDB.EntitySet<SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail>("PurchaseOrderDetails");
    oDataBuilderSampleDB.EntitySet<SamplePWA.Server.Models.SampleDB.Sale>("Sales");
    oDataBuilderSampleDB.EntitySet<SamplePWA.Server.Models.SampleDB.SalesDetail>("SalesDetails");
    oDataBuilderSampleDB.EntitySet<SamplePWA.Server.Models.SampleDB.Supplier>("Suppliers");
    opt.AddRouteComponents("odata/SampleDB", oDataBuilderSampleDB.GetEdmModel()).Count().Filter().OrderBy().Expand().Select().SetMaxTop(null).TimeZone = TimeZoneInfo.Utc;
});
builder.Services.AddScoped<SamplePWA.Client.SampleDBService>();
builder.Services.AddHttpClient("SamplePWA.Server").ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { UseCookies = false }).AddHeaderPropagation(o => o.Headers.Add("Cookie"));
builder.Services.AddHeaderPropagation(o => o.Headers.Add("Cookie"));
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddScoped<SamplePWA.Client.SecurityService>();
builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SampleDBConnection"));
});
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>().AddEntityFrameworkStores<ApplicationIdentityDbContext>().AddDefaultTokenProviders();
builder.Services.AddControllers().AddOData(o =>
{
    var oDataBuilder = new ODataConventionModelBuilder();
    oDataBuilder.EntitySet<ApplicationUser>("ApplicationUsers");
    var usersType = oDataBuilder.StructuralTypes.First(x => x.ClrType == typeof(ApplicationUser));
    usersType.AddProperty(typeof(ApplicationUser).GetProperty(nameof(ApplicationUser.Password)));
    usersType.AddProperty(typeof(ApplicationUser).GetProperty(nameof(ApplicationUser.ConfirmPassword)));
    oDataBuilder.EntitySet<ApplicationRole>("ApplicationRoles");
    o.AddRouteComponents("odata/Identity", oDataBuilder.GetEdmModel()).Count().Filter().OrderBy().Expand().Select().SetMaxTop(null).TimeZone = TimeZoneInfo.Utc;
});
builder.Services.AddScoped<AuthenticationStateProvider, SamplePWA.Client.ApplicationAuthenticationStateProvider>();
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapControllers();
app.UseHeaderPropagation();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode().AddInteractiveWebAssemblyRenderMode().AddAdditionalAssemblies(typeof(SamplePWA.Client._Imports).Assembly);
app.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>().Database.Migrate();
app.Run();