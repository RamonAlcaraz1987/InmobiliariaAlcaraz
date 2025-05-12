using Microsoft.AspNetCore.Mvc;
using InmobiliariaAlcaraz.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace InmobiliariaAlcaraz.Controllers
{
    [Authorize]
    public class ContratoController : Controller
    {
        private readonly IRepositorioContrato _repoContrato;
        private readonly IRepositorioInmueble _repoInmueble;
        private readonly IRepositorioInquilino _repoInquilino;
        private readonly IRepositorioUsuario _repoUsuario;
        private readonly IRepositorioPago _repoPago;

        public ContratoController(
            IRepositorioContrato repoContrato,
            IRepositorioInmueble repoInmueble,
            IRepositorioInquilino repoInquilino,
            IRepositorioUsuario repoUsuario,
            IRepositorioPago repoPago)
        {
            _repoPago = repoPago;
            _repoContrato = repoContrato;
            _repoInmueble = repoInmueble;
            _repoInquilino = repoInquilino;
            _repoUsuario = repoUsuario;
        }

       public IActionResult Index(BusquedaContratosViewModel busqueda = null, int pagina = 1)
        {
            // Check and annul expired contracts
            _repoContrato.AnularContratosVencidos();

            int tamPagina = 10;
            IList<Contrato> contratos;
            int totalContratos;
            var multaStatus = new Dictionary<int, (string Status, bool HasMultaPending)>();

            bool isSearchActive = busqueda?.FechaDesde.HasValue == true && busqueda?.FechaHasta.HasValue == true;
            bool isPlazoSearch = busqueda?.PlazoDias.HasValue == true;

            if (isPlazoSearch)
            {
                if (busqueda.PlazoDias != 30 && busqueda.PlazoDias != 60 && busqueda.PlazoDias != 90)
                {
                    ModelState.AddModelError("", "El plazo debe ser 30, 60 o 90 días.");
                    contratos = new List<Contrato>();
                    totalContratos = 0;
                }
                else
                {
                    contratos = _repoContrato.ObtenerContratosPorPlazo(busqueda.PlazoDias.Value, pagina, tamPagina);
                    totalContratos = _repoContrato.ObtenerCantidadContratosPorPlazo(busqueda.PlazoDias.Value);
                }
                ViewBag.BusquedaActiva = true;
                ViewBag.BusquedaModel = busqueda;
            }
            else if (isSearchActive)
            {
                if (busqueda.FechaHasta < busqueda.FechaDesde)
                {
                    ModelState.AddModelError("", "La fecha 'Hasta' no puede ser anterior a la fecha 'Desde'.");
                    contratos = new List<Contrato>();
                    totalContratos = 0;
                }
                else
                {
                    contratos = _repoContrato.ObtenerContratosVigentes(
                        busqueda.FechaDesde.Value,
                        busqueda.FechaHasta.Value,
                        pagina,
                        tamPagina);
                    totalContratos = _repoContrato.ObtenerCantidadContratosVigentes(
                        busqueda.FechaDesde.Value,
                        busqueda.FechaHasta.Value);
                }
                ViewBag.BusquedaActiva = true;
                ViewBag.BusquedaModel = busqueda;
            }
            else
            {
                contratos = _repoContrato.ObtenerLista(pagina, tamPagina);
                totalContratos = _repoContrato.ObtenerCantidad();
                ViewBag.BusquedaActiva = false;
                ViewBag.BusquedaModel = busqueda ?? new BusquedaContratosViewModel();
                ModelState.Clear();
            }

            // Calcular estado de multa para cada contrato
            foreach (var contrato in contratos)
            {
                var pagos = _repoPago.ObtenerPorContrato(contrato.IdContrato);
                bool hasMultaActiva = pagos.Any(p => p.EsMulta == 1 && !p.Anulado);
                bool hasMultaAnulada = pagos.Any(p => p.EsMulta == 1 && p.Anulado);
                bool hasMultaPending = contrato.FechaFinAnticipado.HasValue && !hasMultaActiva;

                string status;
                if (hasMultaActiva)
                    status = "Pagada";
                else if (hasMultaAnulada)
                    status = "Anulada";
                else if (hasMultaPending)
                    status = "No pagada";
                else
                    status = "N/A";

                multaStatus[contrato.IdContrato] = (status, hasMultaPending);
            }

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalContratos / tamPagina);
            ViewBag.MultaStatus = multaStatus;

            return View(contratos);
        }
        [HttpGet]
        public IActionResult Crear(int id, int pagina = 1)
        {
            try
            {
                int tamPagina = 5;

                var inmueble = _repoInmueble.ObtenerPorId(id);
                if (inmueble == null || inmueble.Disponible != 1)
                {
                    TempData["ErrorMessage"] = "El inmueble no está disponible para contratar";
                    return RedirectToAction("Index", "Inmueble");
                }

                var historialContratos = _repoContrato.ObtenerPorInmueble(id, pagina, tamPagina);
                var totalContratos = _repoContrato.ObtenerCantidadPorInmueble(id);
                var totalPaginas = (int)Math.Ceiling((double)totalContratos / tamPagina);

                ViewBag.Inmueble = inmueble;
                ViewBag.HistorialContratos = historialContratos;
                ViewBag.PaginaActual = pagina;
                ViewBag.TotalPaginas = totalPaginas;
                ViewBag.IdInmueble = id;

                var contrato = new Contrato
                {
                    IdInmueble = id,
                    MontoMensual = inmueble.Precio,
                    IdUsuarioCreacion = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                    FechaCreacion = DateTime.Now,
                    Estado = 1
                };

                return View(contrato);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar la vista de creación: " + ex.Message;
                return RedirectToAction("Index", "Inmueble");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Contrato contrato, int pagina = 1)
        {
            try
            {
                int tamPagina = 5;
                var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var usuario = _repoUsuario.ObtenerPorId(userId);
                if (usuario == null)
                {
                    TempData["ErrorMessage"] = "Usuario no válido";
                    return RedirectToAction("Index");
                }

                var inmueble = _repoInmueble.ObtenerPorId(contrato.IdInmueble);
                if (inmueble == null || inmueble.Disponible != 1)
                {
                    TempData["ErrorMessage"] = "El inmueble no está disponible para contratar";
                    return RedirectToAction("Index", "Inmueble");
                }

                var inquilino = _repoInquilino.ObtenerPorId(contrato.IdInquilino);
                if (inquilino == null)
                {
                    ModelState.AddModelError("IdInquilino", "El inquilino seleccionado no existe");
                }

                if (contrato.FechaFin <= contrato.FechaInicio)
                {
                    ModelState.AddModelError("", "La fecha de fin debe ser posterior a la de inicio.");
                }

                int pagosEsperados = _repoContrato.CalcularPagosEsperados(contrato.FechaInicio, contrato.FechaFin);
                if (pagosEsperados < 1)
                {
                    ModelState.AddModelError("", "El contrato debe tener una duración mínima de un mes (un pago esperado).");
                }

                if (_repoContrato.TieneContratosSolapados(contrato.IdInmueble, contrato.FechaInicio, contrato.FechaFin))
                {
                    ModelState.AddModelError("", "Las fechas seleccionadas se solapan con un contrato existente.");
                }

                if (ModelState.IsValid)
                {
                    contrato.IdUsuarioCreacion = userId;
                    contrato.FechaCreacion = DateTime.Now;
                    contrato.Estado = 1;
                    contrato.MontoMensual = inmueble.Precio;

                    _repoContrato.Alta(contrato);
                    TempData["SuccessMessage"] = "Contrato creado exitosamente";
                    return RedirectToAction("Index");
                }

                var historialContratos = _repoContrato.ObtenerPorInmueble(contrato.IdInmueble, pagina, tamPagina);
                var totalContratos = _repoContrato.ObtenerCantidadPorInmueble(contrato.IdInmueble);
                var totalPaginas = (int)Math.Ceiling((double)totalContratos / tamPagina);

                ViewBag.Inmueble = inmueble;
                ViewBag.HistorialContratos = historialContratos;
                ViewBag.PaginaActual = pagina;
                ViewBag.TotalPaginas = totalPaginas;
                ViewBag.IdInmueble = contrato.IdInmueble;

                return View(contrato);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al crear el contrato: " + ex.Message;
                
                int tamPagina = 5;
                var inmueble = _repoInmueble.ObtenerPorId(contrato.IdInmueble);
                var historialContratos = _repoContrato.ObtenerPorInmueble(contrato.IdInmueble, pagina, tamPagina);
                var totalContratos = _repoContrato.ObtenerCantidadPorInmueble(contrato.IdInmueble);
                var totalPaginas = (int)Math.Ceiling((double)totalContratos / tamPagina);

                ViewBag.Inmueble = inmueble;
                ViewBag.HistorialContratos = historialContratos;
                ViewBag.PaginaActual = pagina;
                ViewBag.TotalPaginas = totalPaginas;
                ViewBag.IdInmueble = contrato.IdInmueble;

                return View(contrato);
            }
        }

        [HttpGet]
        public IActionResult ValidarFechas(int idInmueble, DateTime fechaInicio, DateTime fechaFin)
        {
            bool tieneSolapamiento = _repoContrato.TieneContratosSolapados(idInmueble, fechaInicio, fechaFin);
            return Json(new { isValid = !tieneSolapamiento });
        }

        public IActionResult Detalle(int id, int paginaPagos = 1)
        {
            int tamPagina = 10;
            var contrato = _repoContrato.ObtenerPorId(id);
            if (contrato == null)
            {
                return NotFound();
            }
            var pagos = _repoPago.ObtenerPorContrato(id, paginaPagos, tamPagina);
            var totalPagos = _repoPago.ContarPorContrato(id);
            var totalPaginas = (int)Math.Ceiling((double)totalPagos / tamPagina);

            // Calcular montoMulta si hay FechaFinAnticipado y no hay multa activa
            decimal? montoMulta = null;
            bool hasMulta = pagos.Any(p => p.EsMulta == 1 && !p.Anulado);
            if (contrato.FechaFinAnticipado.HasValue && !hasMulta)
            {
                var originalPagosEsperados = _repoContrato.CalcularPagosEsperados(contrato.FechaInicio, contrato.FechaFin);
                var pagosValidos = pagos.Count(p => !p.Anulado && p.EsMulta == 0);
                montoMulta = pagosValidos > (originalPagosEsperados / 2) ? contrato.MontoMensual : contrato.MontoMensual * 2;
            }

            ViewBag.Pagos = pagos;
            ViewBag.PaginaActualPagos = paginaPagos;
            ViewBag.TotalPaginasPagos = totalPaginas;
            ViewBag.MontoMulta = montoMulta;

            return View(contrato);
        }
        [Authorize(Policy = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var contrato = _repoContrato.ObtenerPorId(id);
            if (contrato == null)
            {
                return NotFound();
            }
            return View(contrato);
        }

        [Authorize(Policy = "Administrador")]
        [HttpPost, ActionName("Eliminar")]
        public IActionResult EliminarConfirmado(int id)
        {
            try
            {
                var contrato = _repoContrato.ObtenerPorId(id);
                if (contrato != null)
                {
                    var inmueble = _repoInmueble.ObtenerPorId(contrato.IdInmueble);
                    if (inmueble != null)
                    {
                        inmueble.Disponible = 1;
                        _repoInmueble.Modificacion(inmueble);
                    }

                    var idUsuarioFinalización = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    _repoContrato.Bajan(id, idUsuarioFinalización);
                    
                    TempData["SuccessMessage"] = "Contrato eliminado exitosamente";
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al eliminar el contrato: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            var contrato = _repoContrato.ObtenerPorId(id);
            if (contrato == null)
            {
                return NotFound();
            }
            if (contrato.Estado == 0)
            {
                TempData["ErrorMessage"] = "No se puede editar un contrato inactivo.";
                return RedirectToAction("Index");
            }
            return View(contrato);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Contrato contrato)
        {
            try
            {
                if (contrato.FechaFin < contrato.FechaInicio)
                {
                    ModelState.AddModelError("", "La fecha de fin no puede ser anterior a la fecha de inicio.");
                }

                var meses = ((contrato.FechaFin.Year - contrato.FechaInicio.Year) * 12) + 
                           contrato.FechaFin.Month - contrato.FechaInicio.Month + 
                           (contrato.FechaFin.Day >= contrato.FechaInicio.Day ? 1 : 0);
                contrato.PagosEsperados = meses;

                if (_repoContrato.TieneContratosSolapados(contrato.IdInmueble, contrato.FechaInicio, contrato.FechaFin, contrato.IdContrato))
                {
                    ModelState.AddModelError("", "Las fechas seleccionadas se solapan con un contrato existente.");
                }

                if (ModelState.IsValid)
                {
                    _repoContrato.Modificacion(contrato);
                    TempData["SuccessMessage"] = "Contrato actualizado exitosamente.";
                    return RedirectToAction("Index");
                }

                return View(contrato);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al actualizar el contrato: {ex.Message}";
                return View(contrato);
            }
        }
        [HttpGet]
        
        public IActionResult FinalizarAnticipado(int id)
        {
            var contrato = _repoContrato.ObtenerPorId(id);
            if (contrato == null)
            {
                return NotFound();
            }
            if (contrato.Estado == 0)
            {
                TempData["ErrorMessage"] = "El contrato ya está finalizado.";
                return RedirectToAction("Index");
            }
            return View(contrato);
        }

        [HttpPost, ActionName("FinalizarAnticipado")]
        [ValidateAntiForgeryToken]
        
        public IActionResult FinalizarAnticipadoConfirmado(int id, DateTime FechaFinAnticipado)
        {
            try
            {
                var contrato = _repoContrato.ObtenerPorId(id);
                if (contrato == null)
                {
                    return NotFound();
                }
                if (contrato.Estado == 0)
                {
                    TempData["ErrorMessage"] = "El contrato ya está finalizado.";
                    return RedirectToAction("Index");
                }

                
                if (FechaFinAnticipado < contrato.FechaInicio || FechaFinAnticipado > contrato.FechaFin)
                {
                    TempData["ErrorMessage"] = $"La fecha de finalización anticipada debe estar entre {contrato.FechaInicio:dd/MM/yyyy} y {contrato.FechaFin:dd/MM/yyyy}.";
                    return View(contrato);
                }

                var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                contrato.FechaFinAnticipado = FechaFinAnticipado;
                contrato.IdUsuarioFinalizacion = userId;
                

               
                int newPagosEsperados = _repoContrato.CalcularPagosEsperados(contrato.FechaInicio, contrato.FechaFinAnticipado.Value);
                
                var pagosValidos = _repoPago.ObtenerTodos()
                    .Where(p => p.IdContrato == id && !p.Anulado && p.EsMulta == 0)
                    .Count();
                
                contrato.PagosEsperados = Math.Max(0, newPagosEsperados - pagosValidos);

                _repoContrato.Modificacion(contrato);

                TempData["SuccessMessage"] = "Contrato finalizado anticipadamente con éxito.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al finalizar el contrato: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}