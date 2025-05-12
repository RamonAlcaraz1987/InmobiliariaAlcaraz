using Microsoft.AspNetCore.Mvc;
using InmobiliariaAlcaraz.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System;

namespace InmobiliariaAlcaraz.Controllers
{
    [Authorize]
    public class PagoController : Controller
    {
        private readonly IRepositorioPago _repoPago;
        private readonly IRepositorioContrato _repoContrato;
        private readonly IRepositorioUsuario _repoUsuario;

        public PagoController(
            IRepositorioPago repoPago,
            IRepositorioContrato repoContrato,
            IRepositorioUsuario repoUsuario)
        {
            _repoPago = repoPago;
            _repoContrato = repoContrato;
            _repoUsuario = repoUsuario;
        }

        public IActionResult Index(int pagina = 1)
        {
            int tamPagina = 10;
            var pagos = _repoPago.ObtenerTodos(pagina, tamPagina);
            var totalPagos = _repoPago.Contar();
            var totalPaginas = (int)Math.Ceiling((double)totalPagos / tamPagina);

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginas;

            return View(pagos);
        }

      [HttpGet]
        public IActionResult Crear(int idContrato)
        {
            // Redirect to the Contrato/Detalle view, which contains the modal
            return RedirectToAction("Detalle", "Contrato", new { id = idContrato });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Pago pago)
        {
            try
            {
                var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var usuario = _repoUsuario.ObtenerPorId(userId);
                if (usuario == null)
                {
                    TempData["ErrorMessage"] = "Usuario no válido.";
                    return RedirectToAction("Index", "Contrato");
                }

                var contrato = _repoContrato.ObtenerPorId(pago.IdContrato);
                if (contrato == null)
                {
                    TempData["ErrorMessage"] = "El contrato no es válido.";
                    return RedirectToAction("Index", "Contrato");
                }

                var endDate = contrato.FechaFinAnticipado.HasValue ? contrato.FechaFinAnticipado.Value : contrato.FechaFin;
                var expectedPayments = _repoContrato.CalcularPagosEsperados(contrato.FechaInicio, endDate);

                var pagosValidos = _repoPago.ObtenerPorContrato(pago.IdContrato)
                    .Count(p => !p.Anulado && p.EsMulta == 0);

                if (pago.EsMulta == 0 && pagosValidos >= expectedPayments)
                {
                    TempData["ErrorMessage"] = $"No se pueden registrar mas pagos regulares. Pagos validos: {pagosValidos}, Pagos esperados: {expectedPayments}.";
                    return RedirectToAction("Detalle", "Contrato", new { id = pago.IdContrato });
                }

                if (pago.EsMulta == 1)
                {
                    var hasMulta = _repoPago.ObtenerPorContrato(pago.IdContrato)
                        .Any(p => p.EsMulta == 1 && !p.Anulado);
                    if (hasMulta)
                    {
                        TempData["ErrorMessage"] = "Ya existe una multa activa para este contrato.";
                        return RedirectToAction("Detalle", "Contrato", new { id = pago.IdContrato });
                    }
                    if (!contrato.FechaFinAnticipado.HasValue)
                    {
                        TempData["ErrorMessage"] = "No se puede registrar una multa sin finalización anticipada.";
                        return RedirectToAction("Detalle", "Contrato", new { id = pago.IdContrato });
                    }
                }

                if (ModelState.IsValid)
                {
                    pago.IdUsuarioCreacion = userId;
                    pago.FechaCreacion = DateTime.Now;
                    pago.Monto = pago.EsMulta == 0 ? contrato.MontoMensual : pago.Monto; // Use contract Monto for regular payments
                    _repoPago.Alta(pago);

                    if (pago.EsMulta == 0)
                    {
                        contrato.PagosEsperados = Math.Max(0, contrato.PagosEsperados - 1);
                        _repoContrato.Modificacion(contrato);
                    }

                    TempData["SuccessMessage"] = pago.EsMulta == 1 ? "Multa registrada exitosamente." : "Pago registrado exitosamente.";
                    return RedirectToAction("Detalle", "Contrato", new { id = pago.IdContrato });
                }

                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["ErrorMessage"] = $"Error en los datos del formulario: {string.Join(", ", errors)}";
                return RedirectToAction("Detalle", "Contrato", new { id = pago.IdContrato });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al registrar el pago: {ex.Message}";
                return RedirectToAction("Detalle", "Contrato", new { id = pago.IdContrato });
            }
        }

        [HttpGet]
        public IActionResult Detalle(int id)
        {
            var pago = _repoPago.ObtenerPorId(id);
            if (pago == null)
            {
                return NotFound();
            }
            return View(pago);
        }

        [HttpGet]
        public IActionResult Anular(int id)
        {
            var pago = _repoPago.ObtenerPorId(id);
            if (pago == null)
            {
                return NotFound();
            }
            if (pago.Anulado)
            {
                TempData["ErrorMessage"] = "El pago ya está anulado.";
                return RedirectToAction("Index");
            }
            return View(pago);
        }

        [HttpPost, ActionName("Anular")]
        [ValidateAntiForgeryToken]
        public IActionResult AnularConfirmado(int id)
        {
            try
            {
                var pago = _repoPago.ObtenerPorId(id);
                if (pago == null)
                {
                    return NotFound();
                }
                if (pago.Anulado)
                {
                    TempData["ErrorMessage"] = "El pago ya está anulado.";
                    return RedirectToAction("Index");
                }

                pago.Anulado = true;
                pago.FechaAnulacion = DateTime.Now;
                pago.IdUsuarioAnulacion = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                _repoPago.Modificacion(pago);

                var contrato = _repoContrato.ObtenerPorId(pago.IdContrato);
                if (contrato != null && pago.EsMulta == 0)
                {
                    var endDate = contrato.FechaFinAnticipado.HasValue ? contrato.FechaFinAnticipado.Value : contrato.FechaFin;
                    var expectedPayments = _repoContrato.CalcularPagosEsperados(contrato.FechaInicio, endDate);
                    contrato.PagosEsperados = Math.Min(expectedPayments, contrato.PagosEsperados + 1);
                    _repoContrato.Modificacion(contrato);
                }

                TempData["SuccessMessage"] = "Pago anulado exitosamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al anular el pago: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public IActionResult Editar(int id)
        {
            var pago = _repoPago.ObtenerPorId(id);
            if (pago == null)
            {
                return NotFound();
            }
            ViewBag.Contrato = _repoContrato.ObtenerPorId(pago.IdContrato);
            return View(pago);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Pago pago)
        {
            try
            {
                var contrato = _repoContrato.ObtenerPorId(pago.IdContrato);
                if (contrato == null || contrato.Estado != 1)
                {
                    TempData["ErrorMessage"] = "El contrato no es válido o no está activo.";
                    return RedirectToAction("Index");
                }

                if (ModelState.IsValid)
                {
                    _repoPago.Modificacion(pago);
                    TempData["SuccessMessage"] = "Pago actualizado exitosamente.";
                    return RedirectToAction("Index");
                }

                ViewBag.Contrato = contrato;
                return View(pago);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al actualizar el pago: {ex.Message}";
                ViewBag.Contrato = _repoContrato.ObtenerPorId(pago.IdContrato);
                return View(pago);
            }
        }
    }
}