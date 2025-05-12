using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using InmobiliariaAlcaraz.Models;
using System.Security.Claims;

namespace InmobiliariaAlcaraz.Controllers;

public class HomeController : Controller
{
    private readonly IRepositorioPropietario propietarios;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IRepositorioPropietario propietarios, ILogger<HomeController> logger)
    {
        this.propietarios = propietarios;
        _logger = logger;
    }

    public IActionResult Index()
    {
        if (User.Identity.IsAuthenticated)
        {
           
            var nombreUsuario = User.Identity.Name;
            
            
            var nombreCompleto = User.FindFirst("FullName")?.Value ?? nombreUsuario;
            
            ViewBag.MensajeBienvenida = $"Bienvenido {nombreCompleto}, estos son nuestros clientes:";
        }
        else
        {
            ViewBag.Titulo = "Página de Inicio";
        }
        
        var clientes = propietarios.ObtenerTodos().Select(x => $"{x.Nombre} {x.Apellido}").ToList();
        return View(clientes);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Error()
    {
        return View(new ErrorViewModel { 
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier 
        });
    }

    public IActionResult Restringido()
    {
        return View();
    }
}