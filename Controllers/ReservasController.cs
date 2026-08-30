using Microsoft.AspNetCore.Mvc;
using Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models;

namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Controllers
{
    public class ReservasController : Controller
    {
        private readonly IRepositorioReserva _repoReserva;
        private readonly IRepositorioInquilino _repoInquilino;
        private readonly IRepositorioInmueble _repoInmueble;

        public ReservasController(
            IRepositorioReserva repoReserva,
            IRepositorioInquilino repoInquilino,
            IRepositorioInmueble repoInmueble)
        {
            _repoReserva = repoReserva;
            _repoInquilino = repoInquilino;
            _repoInmueble = repoInmueble;
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
                return NotFound();
            }
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
                if (reserva.FechaHasta <= reserva.FechaDesde)
                {
                    ModelState.AddModelError("FechaHasta", "La fecha de fin debe ser posterior a la fecha de inicio.");
                }

                if (_repoReserva.ExisteSuperposicion(reserva.IdInmueble, reserva.FechaDesde, reserva.FechaHasta))
                {
                    ModelState.AddModelError("", "El inmueble ya se encuentra reservado en el rango de fechas seleccionado.");
                }

                if (ModelState.IsValid)
                {
                    // Obtener precio por día del inmueble si no se especificó o para asegurar valor base
                    if (reserva.PrecioPorDia <= 0)
                    {
                        var inm = _repoInmueble.ObtenerPorId(reserva.IdInmueble);
                        if (inm != null)
                        {
                            reserva.PrecioPorDia = inm.PrecioDia;
                        }
                    }

                    int dias = (reserva.FechaHasta - reserva.FechaDesde).Days;
                    if (dias <= 0) dias = 1;
                    if (reserva.MontoTotal <= 0)
                    {
                        reserva.MontoTotal = reserva.PrecioPorDia * dias;
                    }

                    _repoReserva.Alta(reserva);
                    return RedirectToAction(nameof(Index));
                }

                CargarSelects();
                return View(reserva);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                CargarSelects();
                return View(reserva);
            }
        }

        // GET: Reservas/Edit/5
        public ActionResult Edit(int id)
        {
            var reserva = _repoReserva.ObtenerPorId(id);
            if (reserva == null)
            {
                return NotFound();
            }
            CargarSelects();
            return View(reserva);
        }

        // POST: Reservas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Reserva reserva)
        {
            try
            {
                if (reserva.FechaHasta <= reserva.FechaDesde)
                {
                    ModelState.AddModelError("FechaHasta", "La fecha de fin debe ser posterior a la fecha de inicio.");
                }

                if (_repoReserva.ExisteSuperposicion(reserva.IdInmueble, reserva.FechaDesde, reserva.FechaHasta, id))
                {
                    ModelState.AddModelError("", "El inmueble ya se encuentra reservado en el rango de fechas seleccionado.");
                }

                if (ModelState.IsValid)
                {
                    reserva.IdReserva = id;
                    int dias = (reserva.FechaHasta - reserva.FechaDesde).Days;
                    if (dias <= 0) dias = 1;
                    if (reserva.MontoTotal <= 0)
                    {
                        reserva.MontoTotal = reserva.PrecioPorDia * dias;
                    }

                    _repoReserva.Modificacion(reserva);
                    return RedirectToAction(nameof(Index));
                }

                CargarSelects();
                return View(reserva);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                CargarSelects();
                return View(reserva);
            }
        }

        // GET: Reservas/Delete/5
        public ActionResult Delete(int id)
        {
            var reserva = _repoReserva.ObtenerPorId(id);
            if (reserva == null)
            {
                return NotFound();
            }
            return View(reserva);
        }

        // POST: Reservas/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                _repoReserva.Baja(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(_repoReserva.ObtenerPorId(id));
            }
        }
    }
}
