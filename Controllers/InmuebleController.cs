using Microsoft.AspNetCore.Mvc;
using InmobiliariaAlcaraz.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using MySql.Data.MySqlClient;

namespace InmobiliariaAlcaraz.Controllers
{   [Authorize]
    public class InmuebleController : Controller
    {
        private readonly IRepositorioInmueble repoInmueble;
        private readonly IRepositorioPropietario repoPropietario;
        private readonly IRepositorioTipo repoTipo;
        private readonly IRepositorioUso repoUso;

        public InmuebleController(IRepositorioInmueble repoInmueble, IRepositorioPropietario repoPropietario,  IRepositorioTipo repoTipo,
    IRepositorioUso repoUso)
        {
            this.repoInmueble = repoInmueble;
            this.repoPropietario = repoPropietario;
            this.repoTipo = repoTipo;
            this.repoUso = repoUso;
        }

    public IActionResult Index(int pagina = 1, int disponible = -1, int? idPropietario = null, BusquedaInmueblesViewModel busqueda = null)
        {
            int cantidadPorPagina = 10;
            IEnumerable<Inmueble> inmuebles;
            int totalRegistros;
            string propietarioNombre = "";
            bool hasFechas = busqueda?.FechaDesde.HasValue == true && busqueda?.FechaHasta.HasValue == true;

            
            if (hasFechas && busqueda.FechaHasta < busqueda.FechaDesde)
            {
                ModelState.AddModelError("", "La fecha 'Hasta' no puede ser anterior a la fecha 'Desde'.");
                ViewBag.BusquedaModel = busqueda ?? new BusquedaInmueblesViewModel();
                ViewBag.HasFechas = false;
                ViewBag.DisponibleFiltro = disponible;
                ViewBag.IdPropietarioFiltro = idPropietario ?? 0;
                ViewBag.PropietarioFiltroNombre = propietarioNombre;
                ViewBag.PaginaActual = pagina;
                ViewBag.TotalPaginas = 0;
                return View(new List<Inmueble>());
            }

           
            if (hasFechas)
            {
                inmuebles = repoInmueble.ObtenerPorFechasDisponibles(busqueda.FechaDesde.Value, busqueda.FechaHasta.Value, pagina, cantidadPorPagina);
                totalRegistros = repoInmueble.ContarPorFechasDisponibles(busqueda.FechaDesde.Value, busqueda.FechaHasta.Value);
            }
            else if (idPropietario.HasValue && idPropietario > 0)
            {
                inmuebles = repoInmueble.ObtenerPorPropietario(idPropietario.Value, pagina, cantidadPorPagina);
                totalRegistros = repoInmueble.ContarPorPropietario(idPropietario.Value);
            }
            else
            {
                inmuebles = repoInmueble.ObtenerLista(pagina, cantidadPorPagina);
                totalRegistros = repoInmueble.Contar();
            }

            
            if (disponible >= 0)
            {
                inmuebles = inmuebles.Where(i => i.Disponible == disponible).ToList();
                totalRegistros = inmuebles.Count();
                inmuebles = inmuebles.Skip((pagina - 1) * cantidadPorPagina).Take(cantidadPorPagina);
            }

            
            if (idPropietario.HasValue && idPropietario > 0)
            {
                var propietario = repoPropietario.ObtenerPorId(idPropietario.Value);
                if (propietario != null)
                {
                    propietarioNombre = $"{propietario.Nombre} {propietario.Apellido}";
                }
            }

            ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalRegistros / cantidadPorPagina);
            ViewBag.PaginaActual = pagina;
            ViewBag.DisponibleFiltro = disponible;
            ViewBag.IdPropietarioFiltro = idPropietario ?? 0;
            ViewBag.PropietarioFiltroNombre = propietarioNombre;
            ViewBag.HasFechas = hasFechas;
            ViewBag.BusquedaModel = busqueda ?? new BusquedaInmueblesViewModel();

            return View(inmuebles);
        }

        public IActionResult Detalle(int id)
        {
            var inmueble = repoInmueble.ObtenerPorId(id);
               if (inmueble == null)
            {
                return NotFound();
            }
            ViewBag.Tipos = repoTipo.ObtenerTodos();
            ViewBag.Usos = repoUso.ObtenerTodos();
            return View(inmueble);
        }

        public IActionResult Crear()
        {
            ViewBag.Propietarios = repoPropietario.ObtenerTodos();
            ViewBag.Tipos = repoTipo.ObtenerTodos();
            ViewBag.Usos = repoUso.ObtenerTodos();
            return View();
        }

        [HttpPost]
        public IActionResult Crear(Inmueble i)
        {
            if (ModelState.IsValid)
            {
                repoInmueble.Alta(i);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Propietarios = repoPropietario.ObtenerTodos();
            ViewBag.Tipos = repoTipo.ObtenerTodos();
            ViewBag.Usos = repoUso.ObtenerTodos();
            return View(i);
        }

        public IActionResult Editar(int id)
        {
            var inmueble = repoInmueble.ObtenerPorId(id);
               if (inmueble == null)
            {
                return NotFound();
            }
            ViewBag.Propietarios = repoPropietario.ObtenerTodos();
            ViewBag.Tipos = repoTipo.ObtenerTodos();
            ViewBag.Usos = repoUso.ObtenerTodos();
            return View(inmueble);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Inmueble i)
        {
            if (ModelState.IsValid)
            {
                repoInmueble.Modificacion(i);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Propietarios = repoPropietario.ObtenerTodos();
            return View(i);
        }

       [Authorize(Policy = "Administrador")]       
        public IActionResult Eliminar(int id)
        {
            var inmueble = repoInmueble.ObtenerPorId(id);
            ViewBag.Tipos = repoTipo.ObtenerTodos();
            ViewBag.Usos = repoUso.ObtenerTodos();
            return View(inmueble);
        }

        [Authorize(Policy = "Administrador")]
        [HttpPost, ActionName("Eliminar")]
        public IActionResult EliminarConfirmado(int id)
        {
            try
            {
                repoInmueble.Baja(id);
                TempData["SuccessMessage"] = "Inmueble eliminado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (MySqlException ex) when (ex.Number == 1451) 
            {
                TempData["ErrorMessage"] = "No se puede borrar, es informacion importante y utilizada.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ocurrió un error al intentar eliminar el inmueble: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}