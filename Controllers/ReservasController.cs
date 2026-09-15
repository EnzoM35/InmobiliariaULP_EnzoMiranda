using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models;

namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Controllers
{
    [Authorize]
    public class ReservasController : Controller
    {
        private readonly IRepositorioReserva _repoReserva;
        private readonly IRepositorioInquilino _repoInquilino;
        private readonly IRepositorioInmueble _repoInmueble;
        private readonly IRepositorioPago _repoPago;

        public ReservasController(
            IRepositorioReserva repoReserva,
            IRepositorioInquilino repoInquilino,
            IRepositorioInmueble repoInmueble,
            IRepositorioPago repoPago)
        {
            _repoReserva = repoReserva;
            _repoInquilino = repoInquilino;
            _repoInmueble = repoInmueble;
            _repoPago = repoPago;
        }

        private int ObtenerUsuarioActualId()
        {
            var claimId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claimId, out int id) ? id : 0;
        }

        private void CargarSelects()
        {
            ViewBag.Inquilinos = _repoInquilino.ObtenerTodos();
            ViewBag.Inmuebles = _repoInmueble.ObtenerDisponibles();
        }

        // GET: Reservas
        public ActionResult Index()
        {
            var lista = _repoReserva.ObtenerTodos();
            return View(lista);
        }

        // GET: Reservas/Details/5
        public ActionResult Details(int id)
        {
            var reserva = _repoReserva.ObtenerPorId(id);
            if (reserva == null)
            {
                TempData["Error"] = "La reserva solicitada no existe.";
                return RedirectToAction(nameof(Index));
            }

            reserva.Pagos = _repoPago.ObtenerPorReserva(id);
            return View(reserva);
        }

        // GET: Reservas/Create
        public ActionResult Create()
        {
            CargarSelects();
            var modelo = new Reserva
            {
                FechaDesde = DateTime.Today,
                FechaHasta = DateTime.Today.AddDays(1)
            };
            return View(modelo);
        }

        // POST: Reservas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Reserva reserva)
        {
            try
            {
                if (reserva.FechaDesde < DateTime.Today)
                {
                    ModelState.AddModelError("FechaDesde", "La fecha de inicio no puede ser anterior a la fecha actual.");
                }

                if (reserva.FechaHasta <= reserva.FechaDesde)
                {
                    ModelState.AddModelError("FechaHasta", "La fecha de fin debe ser posterior a la fecha de inicio.");
                }

                if (reserva.IdInmueble > 0 && reserva.FechaDesde < reserva.FechaHasta)
                {
                    if (_repoReserva.ExisteSuperposicion(reserva.IdInmueble, reserva.FechaDesde, reserva.FechaHasta))
                    {
                        ModelState.AddModelError("", "El inmueble ya se encuentra reservado en el rango de fechas seleccionado.");
                    }
                }

                if (reserva.IdInmueble > 0)
                {
                    var inm = _repoInmueble.ObtenerPorId(reserva.IdInmueble);
                    if (inm != null)
                    {
                        reserva.PrecioPorDia = inm.PrecioDia;
                        int dias = (reserva.FechaHasta - reserva.FechaDesde).Days;
                        reserva.MontoTotal = dias > 0 ? dias * inm.PrecioDia : inm.PrecioDia;
                    }
                }

                if (ModelState.IsValid)
                {
                    reserva.IdUsuarioCreador = ObtenerUsuarioActualId();
                    reserva.Estado = "Vigente";
                    reserva.Activo = true;

                    int id = _repoReserva.Alta(reserva);
                    if (id > 0)
                    {
                        TempData["Mensaje"] = $"Reserva #{id} registrada con éxito. Ahora puede registrar el pago de la seña.";
                        return RedirectToAction("PorReserva", "Pagos", new { id });
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear la reserva: " + ex.Message);
            }

            CargarSelects();
            return View(reserva);
        }

        // GET: Reservas/Edit/5
        public ActionResult Edit(int id)
        {
            var reserva = _repoReserva.ObtenerPorId(id);
            if (reserva == null)
            {
                TempData["Error"] = "La reserva no existe.";
                return RedirectToAction(nameof(Index));
            }

            if (reserva.Estado == "Terminada anticipadamente" || reserva.Estado == "Cancelada" || reserva.Estado == "Anulada")
            {
                TempData["Error"] = $"No se puede editar una reserva en estado '{reserva.Estado}'.";
                return RedirectToAction(nameof(Details), new { id });
            }

            CargarSelects();
            return View(reserva);
        }

        // POST: Reservas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Reserva reserva)
        {
            if (id != reserva.IdReserva)
            {
                return NotFound();
            }

            try
            {
                if (reserva.FechaHasta <= reserva.FechaDesde)
                {
                    ModelState.AddModelError("FechaHasta", "La fecha de fin debe ser posterior a la fecha de inicio.");
                }

                if (reserva.IdInmueble > 0 && reserva.FechaDesde < reserva.FechaHasta)
                {
                    if (_repoReserva.ExisteSuperposicion(reserva.IdInmueble, reserva.FechaDesde, reserva.FechaHasta, id))
                    {
                        ModelState.AddModelError("", "El inmueble ya se encuentra reservado en el rango de fechas seleccionado.");
                    }
                }

                if (reserva.IdInmueble > 0)
                {
                    var inm = _repoInmueble.ObtenerPorId(reserva.IdInmueble);
                    if (inm != null)
                    {
                        reserva.PrecioPorDia = inm.PrecioDia;
                        int dias = (reserva.FechaHasta - reserva.FechaDesde).Days;
                        reserva.MontoTotal = dias > 0 ? dias * inm.PrecioDia : inm.PrecioDia;
                    }
                }

                if (ModelState.IsValid)
                {
                    _repoReserva.Modificacion(reserva);
                    TempData["Mensaje"] = "Reserva modificada correctamente.";
                    return RedirectToAction(nameof(Details), new { id });
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al modificar la reserva: " + ex.Message);
            }

            CargarSelects();
            return View(reserva);
        }

        // GET: Reservas/Delete/5 (Solo Administrador)
        [Authorize(Roles = "Administrador")]
        public ActionResult Delete(int id)
        {
            var reserva = _repoReserva.ObtenerPorId(id);
            if (reserva == null)
            {
                TempData["Error"] = "La reserva no existe.";
                return RedirectToAction(nameof(Index));
            }
            return View(reserva);
        }

        // POST: Reservas/Delete/5 (Solo Administrador)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                _repoReserva.Baja(id);
                TempData["Mensaje"] = "Reserva anulada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al anular la reserva: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // MÓDULO 3: TERMINACIÓN ANTICIPADA CON MULTA
        // ==========================================
        public ActionResult Terminar(int id, DateTime? fechaEfectiva)
        {
            var reserva = _repoReserva.ObtenerPorId(id);
            if (reserva == null)
            {
                TempData["Error"] = "La reserva no existe.";
                return RedirectToAction(nameof(Index));
            }

            if (reserva.Estado != "Vigente")
            {
                TempData["Error"] = $"Solo es posible rescindir anticipadamente reservas en estado 'Vigente'. Estado actual: {reserva.Estado}.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var vm = CalcularLiquidacionTerminacion(reserva, fechaEfectiva ?? DateTime.Today);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmarTerminacion(int idReserva, DateTime fechaTerminacion)
        {
            var reserva = _repoReserva.ObtenerPorId(idReserva);
            if (reserva == null)
            {
                TempData["Error"] = "La reserva no existe.";
                return RedirectToAction(nameof(Index));
            }

            if (reserva.Estado != "Vigente")
            {
                TempData["Error"] = "La reserva no se encuentra vigente.";
                return RedirectToAction(nameof(Details), new { id = idReserva });
            }

            var calculo = CalcularLiquidacionTerminacion(reserva, fechaTerminacion);
            int usuarioId = ObtenerUsuarioActualId();

            // 1. Actualizar estado de la reserva
            int res = _repoReserva.TerminarReserva(idReserva, fechaTerminacion, calculo.MultaCalculada, usuarioId);

            if (res > 0)
            {
                // 2. Generar automáticamente el comprobante de pago por la multa
                if (calculo.MultaCalculada > 0)
                {
                    var pagoMulta = new Pago
                    {
                        IdReserva = idReserva,
                        NumeroPago = _repoPago.ObtenerSiguienteNumeroPago(idReserva),
                        FechaPago = DateTime.Now,
                        Importe = calculo.MultaCalculada,
                        Concepto = $"Multa por rescisión anticipada ({calculo.PorcentajeMulta:0}%) - Saldo no utilizado: ${calculo.SaldoNoUtilizado:N2}",
                        Estado = "Activo",
                        IdUsuarioCreador = usuarioId,
                        Activo = true
                    };
                    _repoPago.Alta(pagoMulta);
                }

                TempData["Mensaje"] = $"Reserva #{idReserva} terminada anticipadamente al {fechaTerminacion:dd/MM/yyyy}. Se aplicó multa de ${calculo.MultaCalculada:N2} ({calculo.PorcentajeMulta:0}%).";
                return RedirectToAction("PorReserva", "Pagos", new { id = idReserva });
            }

            TempData["Error"] = "No se pudo registrar la terminación de la reserva.";
            return RedirectToAction(nameof(Details), new { id = idReserva });
        }

        private static TerminarReservaViewModel CalcularLiquidacionTerminacion(Reserva reserva, DateTime fechaTerminacion)
        {
            int totalDias = Math.Max(1, (reserva.FechaHasta - reserva.FechaDesde).Days);
            
            // Si la fecha elegida es anterior a FechaDesde, tomar FechaDesde
            if (fechaTerminacion < reserva.FechaDesde)
                fechaTerminacion = reserva.FechaDesde;

            // Si la fecha elegida es posterior o igual a FechaHasta, no hay dias restantes
            int diasCumplidos = Math.Max(0, (fechaTerminacion - reserva.FechaDesde).Days);
            int diasRestantes = Math.Max(0, (reserva.FechaHasta - fechaTerminacion).Days);

            decimal saldoNoUtilizado = diasRestantes * reserva.PrecioPorDia;

            // Regla narrativa:
            // Si se cumplió menos de la mitad del tiempo pactado -> multa = 50%
            // Si se cumplió más o la mitad del tiempo pactado -> multa = 25%
            decimal pctMulta;
            if (diasCumplidos < (totalDias / 2.0))
            {
                pctMulta = 50m;
            }
            else
            {
                pctMulta = 25m;
            }

            decimal multaCalculada = Math.Round(saldoNoUtilizado * (pctMulta / 100m), 2);

            return new TerminarReservaViewModel
            {
                IdReserva = reserva.IdReserva,
                Reserva = reserva,
                FechaTerminacion = fechaTerminacion,
                DiasPactados = totalDias,
                DiasCumplidos = diasCumplidos,
                DiasRestantes = diasRestantes,
                SaldoNoUtilizado = saldoNoUtilizado,
                PorcentajeMulta = pctMulta,
                MultaCalculada = multaCalculada
            };
        }

        // ==========================================
        // MÓDULO 3: RENOVACIÓN / EXTENSIÓN DE RESERVA
        // ==========================================
        public ActionResult Renovar(int id)
        {
            var original = _repoReserva.ObtenerPorId(id);
            if (original == null)
            {
                TempData["Error"] = "La reserva original no existe.";
                return RedirectToAction(nameof(Index));
            }

            int diasOriginales = Math.Max(1, original.CantidadDias);
            var fechaDesdeNueva = original.FechaHasta;
            var fechaHastaNueva = original.FechaHasta.AddDays(diasOriginales);

            var renovacion = new Reserva
            {
                IdInquilino = original.IdInquilino,
                Inquilino = original.Inquilino,
                IdInmueble = original.IdInmueble,
                Inmueble = original.Inmueble,
                FechaDesde = fechaDesdeNueva,
                FechaHasta = fechaHastaNueva,
                PrecioPorDia = original.Inmueble?.PrecioDia ?? original.PrecioPorDia,
                MontoTotal = (original.Inmueble?.PrecioDia ?? original.PrecioPorDia) * diasOriginales,
                IdReservaOrigen = original.IdReserva,
                ReservaOrigen = original,
                Estado = "Vigente"
            };

            // Verificar si hay conflicto para advertir de antemano
            bool ocupado = _repoReserva.ExisteSuperposicion(renovacion.IdInmueble, renovacion.FechaDesde, renovacion.FechaHasta);
            ViewBag.ConflictoFechas = ocupado;
            ViewBag.ReservaOriginal = original;

            return View(renovacion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Renovar(Reserva renovacion)
        {
            var original = _repoReserva.ObtenerPorId(renovacion.IdReservaOrigen ?? 0);
            if (original == null)
            {
                ModelState.AddModelError("", "No se encontró la reserva origen para la renovación.");
            }

            if (renovacion.FechaHasta <= renovacion.FechaDesde)
            {
                ModelState.AddModelError("FechaHasta", "La fecha de fin debe ser posterior a la fecha de inicio.");
            }

            if (renovacion.IdInmueble > 0 && renovacion.FechaDesde < renovacion.FechaHasta)
            {
                if (_repoReserva.ExisteSuperposicion(renovacion.IdInmueble, renovacion.FechaDesde, renovacion.FechaHasta))
                {
                    ModelState.AddModelError("", "El inmueble ya no se encuentra disponible para ese período de extensión.");
                }
            }

            var inm = _repoInmueble.ObtenerPorId(renovacion.IdInmueble);
            if (inm != null)
            {
                renovacion.PrecioPorDia = inm.PrecioDia;
                int dias = (renovacion.FechaHasta - renovacion.FechaDesde).Days;
                renovacion.MontoTotal = dias > 0 ? dias * inm.PrecioDia : inm.PrecioDia;
            }

            if (ModelState.IsValid)
            {
                renovacion.IdUsuarioCreador = ObtenerUsuarioActualId();
                renovacion.Estado = "Vigente";
                renovacion.Activo = true;

                int nuevoId = _repoReserva.Alta(renovacion);
                if (nuevoId > 0)
                {
                    TempData["Mensaje"] = $"Renovación registrada exitosamente como Reserva #{nuevoId} (vinculada a la Reserva #{renovacion.IdReservaOrigen}).";
                    return RedirectToAction("PorReserva", "Pagos", new { id = nuevoId });
                }
            }

            ViewBag.ReservaOriginal = original;
            return View(renovacion);
        }
    }
}
