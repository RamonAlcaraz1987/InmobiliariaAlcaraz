using Microsoft.AspNetCore.Mvc;
using InmobiliariaAlcaraz.Models;

using Microsoft.AspNetCore.Mvc.ModelBinding;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace InmobiliariaAlcaraz.Controllers
{
    public class PropietarioController : Controller
    {
       private readonly IConfiguration configuration;
       private readonly IRepositorioPropietario repositorio;

        public PropietarioController(IConfiguration config, IRepositorioPropietario repo)
        {
            configuration = config;
            repositorio = repo;
        }

        public IActionResult Index()
        {
            IList<Propietario> propietarios = repositorio.ObtenerTodos();
            return View(propietarios);
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

        public IActionResult Eliminar(int id)
        {
            var propietario = repositorio.ObtenerPorId(id);
            return View(propietario);
        }
        [HttpPost,ActionName("Eliminar")]
        public IActionResult EliminarConfirmado(int id)
        {
            repositorio.Baja(id);
            return RedirectToAction(nameof(Index));
        }

       // public IActionResult Buscar(string email)
       // { 
           // var propietario = repositorio.ObtenerPorEmail(email);
           // return View("detalle",propietario);
       // }
    }

}