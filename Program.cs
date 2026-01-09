using Microsoft.EntityFrameworkCore;
using AplikasiCheckDimensi.Data;
using AplikasiCheckDimensi.Hubs;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Force InvariantCulture for decimal parsing (use dot as decimal separator)
var cultureInfo = new CultureInfo("en-US");
cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add Session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8); // Session timeout 8 hours
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add SignalR for real-time WebSocket communication
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Disable HTTPS redirection for local network access
// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable Session
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map SignalR Hub for real-time updates
app.MapHub<QCHub>("/qcHub");

// Note: Database dan users sudah dibuat dari script SQLServer_Setup.sql
// Test auto-reload 22.32.19
// Tidak perlu seed lagi karena database sudah ada dengan struktur yang benar

app.Run();

