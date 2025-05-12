using Microsoft.AspNetCore.Mvc;
using InmobiliariaAlcaraz.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace InmobiliariaAlcaraz.Controllers
{   [Authorize]
    public class PropietarioController : Controller
    {
       private readonly IConfiguration configuration;
       private readonly IRepositorioPropietario repositorio;
       

        public PropietarioController(IConfiguration config, IRepositorioPropietario repo)
        {
            configuration = config;
            repositorio = repo;
        }

          public IActionResult Index(int pagina = 1)
        {
            int tamanioPagina = 10;
            var lista = repositorio.ObtenerLista(pagina, tamanioPagina);
            int totalRegistros = repositorio.ObtenerCantidad();
            int totalPaginas = (int)Math.Ceiling((double)totalRegistros / tamanioPagina);
            
            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginas;

            return View(lista);
        }
        
        public IActionResult Detalle(int id)
        {
            var propietario = repositorio.ObtenerPorId(id);
            return View(propietario);
        }

        public IActionResult Crear()
        {
            return View();
            
        }
        [HttpPost]
        public IActionResult Crear(Propietario propietario)
        {
            if (ModelState.IsValid)
            {
                repositorio.Alta(propietario);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(propietario);
            }
        }

        public IActionResult Editar(int id)
        {
            var propietario = repositorio.ObtenerPorId(id);
            return View(propietario);
        }
        [HttpPost]
        public IActionResult Editar(Propietario propietario)
        {
            if (ModelState.IsValid)
            {
                repositorio.Modificacion(propietario);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(propietario);
            }
        }
        [Authorize(Policy = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var propietario = repositorio.ObtenerPorId(id);
            return View(propietario);
        }
        [Authorize(Policy = "Administrador")]
        
        [HttpPost, ActionName("Eliminar")]
        public IActionResult EliminarConfirmado(int id)
        {
            try
            {
                repositorio.Baja(id);
                TempData["Mensaje"] = "Propietario eliminado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (MySqlException ex) when (ex.Number == 1451)
            {
                TempData["Error"] = "No se puede borrar, es informacion importante y utilizada.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error al intentar eliminar el propietario: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
       // public IActionResult Buscar(string email)
       // { 
           // var propietario = repositorio.ObtenerPorEmail(email);
           // return View("detalle",propietario);
       // }
       [HttpGet]
         [Route("Propietario/Buscar")]   
        public IActionResult Buscar(string q)
        {
            try
            {
                var res = repositorio.BuscarPorNombre(q);
                return Json(new { datos = res });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

    }

}