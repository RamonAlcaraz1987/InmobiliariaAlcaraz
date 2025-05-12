
using Microsoft.AspNetCore.Mvc;
using InmobiliariaAlcaraz.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Authorization;
using MySql.Data.MySqlClient;

namespace InmobiliariaAlcaraz.Controllers
{
    [Authorize]
    public class InquilinoController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IRepositorioInquilino repositorio;

        public InquilinoController(IConfiguration config, IRepositorioInquilino repo)
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
            var inquilino = repositorio.ObtenerPorId(id);
            return View(inquilino);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Crear(Inquilino inquilino)
        {
            if (ModelState.IsValid)
            {
                repositorio.Alta(inquilino);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(inquilino);
            }
        }

        public IActionResult Editar(int id)
        {
            var inquilino = repositorio.ObtenerPorId(id);
            return View(inquilino);
        }

        [HttpPost]
        public IActionResult Editar(Inquilino inquilino)
        {
            if (ModelState.IsValid)
            {
                repositorio.Modificacion(inquilino);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(inquilino);
            }
        }

        [Authorize(Policy = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var inquilino = repositorio.ObtenerPorId(id);
            return View(inquilino);
        }

        
        [Authorize(Policy = "Administrador")]
        [HttpPost, ActionName("Eliminar")]
        public IActionResult EliminarConfirmado(int id)
        {
            try
            {
                repositorio.Baja(id);
                TempData["SuccessMessage"] = "Inquilino eliminado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (MySqlException ex) when (ex.Number == 1451) 
            {
                TempData["ErrorMessage"] = "No se puede borrar, es informacion importante y utilizada.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ocurrió un error al intentar eliminar el inquilino: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }        [HttpGet]
                public IActionResult Buscar(string q)
        {
            try
            {
                var inquilinos = repositorio.ObtenerTodos()
                    .Where(i => i.Nombre.Contains(q, StringComparison.OrdinalIgnoreCase) || 
                            i.Apellido.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                            i.Dni.Contains(q))
                    .Select(i => new {
                        idInquilino = i.IdInquilino,
                        nombre = i.Nombre,
                        apellido = i.Apellido,
                        dni = i.Dni
                    })
                    .ToList();

                return Json(new { datos = inquilinos });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}