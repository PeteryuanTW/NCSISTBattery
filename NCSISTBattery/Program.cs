using NCSISTBattery.Services;
using NCSISTBattery.Components;
using Microsoft.EntityFrameworkCore;
using NCSISTBattery.EFModel;
using CommonLibraryP.MachinePKG;
using CommonLibraryP.ShopfloorPKG;
using CommonLibraryP.MapPKG;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDevExpressBlazor(options => {
    options.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v5;
    options.SizeMode = DevExpress.Blazor.SizeMode.Large;
});

builder.AddMachineService();
builder.AddShopfloorService();
builder.AddMapService();

builder.Services.AddSingleton<WeatherForecastService>();

builder.Services.AddDbContextFactory<NCSISTBatteryDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


builder.Services.AddSingleton<DataService>();
builder.Services.AddHostedService<InitService>();





builder.Services.AddMvc();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AllowAnonymous();

app.Run();