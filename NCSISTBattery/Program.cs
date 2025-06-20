using CommonLibraryP.Data;
using CommonLibraryP.MachinePKG;
using CommonLibraryP.MapPKG;
using CommonLibraryP.NotificationUtility;
using CommonLibraryP.ShopfloorPKG;
using DevExpress.Blazor;
using Microsoft.EntityFrameworkCore;
using NCSISTBattery.Components;
using NCSISTBattery.EFModel;
using NCSISTBattery.Machine;
using NCSISTBattery.Services;
using OfficeOpenXml;
using System.Drawing;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDevExpressBlazor(options => {
    options.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v5;
    options.SizeMode = DevExpress.Blazor.SizeMode.Large;
});

//builder.AddMachineService();
//MachineTypeEnumHelper.AddCustomConnection<CustomModbusMachine>(10);
//CommonEnumHelper.AddCustomStatus(150, "Custom Status", ButtonRenderStyle.Danger, Color.FromArgb(204, 0, 0));


builder.AddShopfloorService();
//builder.Services.AddScoped<NotificationService>();
//builder.AddMapService();

builder.Services.AddSingleton<WeatherForecastService>();

builder.Services.AddDbContextFactory<NCSISTBatteryDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


builder.Services.AddSingleton<DataService>();
builder.Services.AddHostedService<InitService>();

builder.Services.AddMvc();

ExcelPackage.License.SetNonCommercialOrganization("TM Robot");

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