
using Microsoft.AspNetCore.Mvc;
using InmobiliariaAlcaraz.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace InmobiliariaAlcaraz.Controllers
{
    public class InquilinoController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IRepositorioInquilino repositorio;

        public InquilinoController(IConfiguration config, IRepositorioInquilino repo)
        {
            configuration = config;
            repositorio = repo;
        }

        public IActionResult Index()
        {
            IList<Inquilino> inquilinos = repositorio.ObtenerTodos();
            return View(inquilinos);
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

        public IActionResult Eliminar(int id)
        {
            var inquilino = repositorio.ObtenerPorId(id);
            return View(inquilino);
        }

        [HttpPost, ActionName("Eliminar")]
        public IActionResult EliminarConfirmado(int id)
        {
            repositorio.Baja(id);
            return RedirectToAction(nameof(Index));
        }
    }
}