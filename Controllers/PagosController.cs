using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models;

namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Controllers
{
    [Authorize]
    public class PagosController : Controller
    {
        private readonly IRepositorioPago _repoPago;
        private readonly IRepositorioReserva _repoReserva;

        public PagosController(IRepositorioPago repoPago, IRepositorioReserva repoReserva)
        {
            _repoPago = repoPago;
            _repoReserva = repoReserva;
        }

        private int ObtenerUsuarioActualId()
        {
            var claimId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claimId, out int id) ? id : 0;
        }

        // ==========================================
        // LISTADO GENERAL DE PAGOS (PAGINADO)
        // ==========================================
        public IActionResult Index(string? filtro, int pagina = 1)
        {
            ViewBag.Filtro = filtro;
            var pagos = _repoPago.ObtenerPaginado(filtro, pagina, 10);
            return View(pagos);
        }

        // ==========================================
        // LISTADO DE PAGOS POR RESERVA ESPECIFICA
        // ==========================================
        public IActionResult PorReserva(int id)
        {
            var reserva = _repoReserva.ObtenerPorId(id);
            if (reserva == null)
            {
                TempData["Error"] = "La reserva solicitada no existe.";
                return RedirectToAction("Index", "Reservas");
            }

            var pagos = _repoPago.ObtenerPorReserva(id);
            reserva.Pagos = pagos;

            ViewBag.Reserva = reserva;
            return View(pagos);
        }

        // ==========================================
        // DETALLES DEL PAGO
        // ==========================================
        public IActionResult Details(int id)
        {
            var pago = _repoPago.ObtenerPorId(id);
            if (pago == null)
            {
                TempData["Error"] = "El pago solicitado no existe.";
                return RedirectToAction(nameof(Index));
            }

            return View(pago);
        }

        // ==========================================
        // CREAR PAGO
        // ==========================================
        public IActionResult Create(int? idReserva)
        {
            CargarComboReservas(idReserva);

            var nuevoPago = new Pago
            {
                FechaPago = DateTime.Now
            };

            if (idReserva.HasValue && idReserva.Value > 0)
            {
                var reserva = _repoReserva.ObtenerPorId(idReserva.Value);
                if (reserva != null)
                {
                    nuevoPago.IdReserva = reserva.IdReserva;
                    nuevoPago.NumeroPago = _repoPago.ObtenerSiguienteNumeroPago(reserva.IdReserva);

                    var pagosPrevios = _repoPago.ObtenerPorReserva(reserva.IdReserva);
                    decimal totalAbonado = pagosPrevios.Where(p => p.Estado == "Activo").Sum(p => p.Importe);
                    decimal saldo = (reserva.MontoTotal + reserva.Multa) - totalAbonado;

                    if (pagosPrevios.Count == 0)
                    {
                        // Sugerir seña
                        nuevoPago.Importe = reserva.MontoMinimoSenia;
                        nuevoPago.Concepto = $"Seña de reserva ({reserva.Inmueble?.PorcentajeReserva ?? 10}%)";
                    }
                    else
                    {
                        nuevoPago.Importe = Math.Max(0, saldo);
                        nuevoPago.Concepto = saldo <= 0 ? "Pago adicional" : "Cancelación / Saldo de reserva";
                    }
                }
            }

            return View(nuevoPago);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Pago pago)
        {
            if (pago.Importe <= 0)
            {
                ModelState.AddModelError(nameof(pago.Importe), "El importe a abonar debe ser superior a cero.");
            }

            var reserva = _repoReserva.ObtenerPorId(pago.IdReserva);
            if (reserva == null)
            {
                ModelState.AddModelError(nameof(pago.IdReserva), "La reserva seleccionada no es válida.");
            }

            if (!ModelState.IsValid)
            {
                CargarComboReservas(pago.IdReserva);
                return View(pago);
            }

            pago.IdUsuarioCreador = ObtenerUsuarioActualId();
            pago.Estado = "Activo";
            pago.Activo = true;

            int nuevoId = _repoPago.Alta(pago);
            if (nuevoId > 0)
            {
                TempData["Mensaje"] = $"Pago N° {pago.NumeroPago} registrado exitosamente por ${pago.Importe:N2}.";
                return RedirectToAction(nameof(PorReserva), new { id = pago.IdReserva });
            }

            TempData["Error"] = "No se pudo registrar el pago. Intente nuevamente.";
            CargarComboReservas(pago.IdReserva);
            return View(pago);
        }

        // ==========================================
        // EDITAR PAGO (SOLO CONCEPTO SEGUN REGLA)
        // ==========================================
        public IActionResult Edit(int id)
        {
            var pago = _repoPago.ObtenerPorId(id);
            if (pago == null)
            {
                TempData["Error"] = "El pago no existe.";
                return RedirectToAction(nameof(Index));
            }

            if (pago.Estado == "Anulado" || !pago.Activo)
            {
                TempData["Error"] = "No es posible modificar un pago anulado.";
                return RedirectToAction(nameof(Details), new { id });
            }

            return View(pago);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Pago pago)
        {
            if (id != pago.IdPago)
            {
                TempData["Error"] = "Identificador de pago inconsistente.";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(pago.Concepto))
            {
                ModelState.AddModelError(nameof(pago.Concepto), "El concepto no puede estar vacío.");
            }

            if (!ModelState.IsValid)
            {
                var pagoOriginal = _repoPago.ObtenerPorId(id);
                if (pagoOriginal != null)
                {
                    pago.Reserva = pagoOriginal.Reserva;
                    pago.NumeroPago = pagoOriginal.NumeroPago;
                    pago.FechaPago = pagoOriginal.FechaPago;
                    pago.Importe = pagoOriginal.Importe;
                    pago.Estado = pagoOriginal.Estado;
                }
                return View(pago);
            }

            int filas = _repoPago.Modificar(pago);
            if (filas > 0)
            {
                TempData["Mensaje"] = "Concepto del pago actualizado correctamente.";
                return RedirectToAction(nameof(Details), new { id = pago.IdPago });
            }

            TempData["Error"] = "No se pudo actualizar el pago.";
            return View(pago);
        }

        // ==========================================
        // ANULAR PAGO (BAJA LOGICA - SOLO ADMINISTRADOR)
        // ==========================================
        [Authorize(Roles = "Administrador")]
        public IActionResult Anular(int id)
        {
            var pago = _repoPago.ObtenerPorId(id);
            if (pago == null)
            {
                TempData["Error"] = "El pago no existe.";
                return RedirectToAction(nameof(Index));
            }

            if (pago.Estado == "Anulado" || !pago.Activo)
            {
                TempData["Error"] = "El pago ya se encuentra anulado.";
                return RedirectToAction(nameof(Details), new { id });
            }

            return View(pago);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult ConfirmarAnular(int idPago)
        {
            var pago = _repoPago.ObtenerPorId(idPago);
            if (pago == null)
            {
                TempData["Error"] = "El pago no existe.";
                return RedirectToAction(nameof(Index));
            }

            int usuarioId = ObtenerUsuarioActualId();
            int filas = _repoPago.Anular(idPago, usuarioId);

            if (filas > 0)
            {
                TempData["Mensaje"] = $"El pago N° {pago.NumeroPago} de la Reserva #{pago.IdReserva} ha sido anulado con éxito.";
                return RedirectToAction(nameof(PorReserva), new { id = pago.IdReserva });
            }

            TempData["Error"] = "No se pudo anular el pago.";
            return RedirectToAction(nameof(Details), new { id = idPago });
        }

        private void CargarComboReservas(int? idReservaSeleccionada = null)
        {
            var reservas = _repoReserva.ObtenerTodos();
            var items = reservas.Select(r => new
            {
                r.IdReserva,
                Descripcion = $"Reserva #{r.IdReserva} - {r.Inmueble?.Direccion} ({r.Inquilino?.Apellido}, {r.Inquilino?.Nombre}) [{r.FechaDesde:dd/MM/yyyy} al {r.FechaHasta:dd/MM/yyyy}]"
            });

            ViewBag.Reservas = new SelectList(items, "IdReserva", "Descripcion", idReservaSeleccionada);
        }
    }
}
