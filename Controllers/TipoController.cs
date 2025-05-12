using Microsoft.AspNetCore.Mvc;
using InmobiliariaAlcaraz.Models;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.AspNetCore.Authorization;
using MySql.Data.MySqlClient;

namespace InmobiliariaAlcaraz.Controllers
{
    [Authorize]
    public class TipoController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IRepositorioTipo repositorio;

        public TipoController(IConfiguration config, IRepositorioTipo repo)
        {
            configuration = config;
            repositorio = repo;
        }

        public IActionResult Index(int pagina = 1)
        {
            int tamanioPagina = 10;
            var lista = repositorio.ObtenerLista(pagina, tamanioPagina);
            int totalRegistros = repositorio.ObtenerTodos().Count;
            int totalPaginas = (int)Math.Ceiling((double)totalRegistros / tamanioPagina);

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginas;

            return View(lista);
        }

        public IActionResult Detalle(int id)
        {
            var tipo = repositorio.ObtenerPorId(id);
            return View(tipo);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Crear(Tipo tipo)
        {
            if (ModelState.IsValid)
            {
                repositorio.Alta(tipo);
                return RedirectToAction(nameof(Index));
            }
            return View(tipo);
        }

        public IActionResult Editar(int id)
        {
            var tipo = repositorio.ObtenerPorId(id);
            return View(tipo);
        }

        [HttpPost]
        public IActionResult Editar(Tipo tipo)
        {
            if (ModelState.IsValid)
            {
                repositorio.Modificacion(tipo);
                return RedirectToAction(nameof(Index));
            }
            return View(tipo);
        }

        [Authorize(Policy = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var tipo = repositorio.ObtenerPorId(id);
            return View(tipo);
        }

       [Authorize(Policy = "Administrador")]
        [HttpPost, ActionName("Eliminar")]
        public IActionResult EliminarConfirmado(int id)
        {
            try
            {
                repositorio.Baja(id);
                TempData["SuccessMessage"] = "Tipo eliminado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (MySqlException ex) when (ex.Number == 1451) 
            {
                TempData["ErrorMessage"] = "No se puede borrar, es informacion importante y utilizada.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ocurrió un error al intentar eliminar el Tipo: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}