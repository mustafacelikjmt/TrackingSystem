using Microsoft.AspNetCore.Authentication.JwtBearer;
using Service.DAL;
using Service.JsonService;
using Service.MqttMessageManager;
using TrackingSystemWeb.BackgroundServices;
using TrackingSystemWeb.Hubs;
using TrackingSystemWeb.Mapping;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddSingleton<MqttClientService>();
builder.Services.AddSingleton<MqttDal>();
builder.Services.AddSingleton<JsonDeserialize>();
builder.Services.AddSingleton<LocationHub>();
builder.Services.AddAutoMapper(typeof(MapProfile));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddCookie(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Home/AccessDenied";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.Name = "ProductClientAppJwt";
    //options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    //options.SlidingExpiration = true;
});

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services.AddHostedService<MqttClientAPI>();

builder.Services.AddSignalR();

builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(365);
    options.IncludeSubDomains = true;
    options.Preload = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<LocationHub>("/LocationHub");

app.Run();