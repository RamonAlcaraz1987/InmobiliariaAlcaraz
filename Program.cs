using InmobiliariaAlcaraz.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(
    options =>
    {
        options.LoginPath = "/Usuario/Login";
        options.LogoutPath = "/Usuario/Logout";
        options.AccessDeniedPath = "/Home/Restringido";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    }
);
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Administrador", policy => 
        policy.RequireClaim(ClaimTypes.Role, "1", "Administrador"));
});
builder.Services.AddTransient<IRepositorioInmueble, RepositorioInmueble>();
builder.Services.AddTransient<IRepositorioImagen, RepositorioImagen>();
builder.Services.AddTransient<IRepositorioPropietario, RepositorioPropietario>();
builder.Services.AddTransient<IRepositorioInquilino, RepositorioInquilino>();
builder.Services.AddTransient<IRepositorioTipo, RepositorioTipo>();
builder.Services.AddTransient<IRepositorioUso, RepositorioUso>();
builder.Services.AddTransient<IRepositorioUsuario, RepositorioUsuario>();
builder.Services.AddTransient<IRepositorioContrato, RepositorioContrato>();
builder.Services.AddTransient<IRepositorioPago, RepositorioPago>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

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
app.MapControllerRoute("login", "entrar/{**accion}", new { controller = "Usuario", action = "Login" });
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
app.Run();
