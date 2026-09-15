using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models;

namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Controllers
{
    [Authorize]
    public class InformesController : Controller
    {
        private readonly IRepositorioInmueble _repoInmueble;
        private readonly IRepositorioReserva _repoReserva;
        private readonly IRepositorioPropietario _repoPropietario;
        private readonly IRepositorioPago _repoPago;

        public InformesController(
            IRepositorioInmueble repoInmueble,
            IRepositorioReserva repoReserva,
            IRepositorioPropietario repoPropietario,
            IRepositorioPago repoPago)
        {
            _repoInmueble = repoInmueble;
            _repoReserva = repoReserva;
            _repoPropietario = repoPropietario;
            _repoPago = repoPago;
        }

        // ==========================================
        // VISTA PRINCIPAL / TABLERO DE INFORMES
        // ==========================================
        public IActionResult Index()
        {
            ViewBag.Propietarios = _repoPropietario.ObtenerTodos();
            ViewBag.Reservas = _repoReserva.ObtenerTodos();
            return View();
        }

        // ==========================================
        // INFORME 1: Inmuebles y dueños (Filtro por Disponible)
        // ==========================================
        [HttpGet]
        public IActionResult ApiInmueblesPorDisponibilidad(bool? disponible)
        {
            var lista = _repoInmueble.ObtenerPorDisponibilidad(disponible);
            var res = lista.Select(i => new
            {
                i.IdInmueble,
                i.Direccion,
                Tipo = i.Tipo?.Descripcion ?? "Sin tipo",
                i.Cupo,
                i.PrecioDia,
                i.PorcentajeReserva,
                i.Disponible,
                i.Portada,
                Duenio = $"{i.Duenio?.Nombre} {i.Duenio?.Apellido}",
                DniDuenio = i.Duenio?.Dni ?? "",
                TelefonoDuenio = i.Duenio?.Telefono ?? ""
            });
            return Json(res);
        }

        // ==========================================
        // INFORME 2: Inmuebles de un propietario específico
        // ==========================================
        [HttpGet]
        public IActionResult ApiInmueblesPorPropietario(int? idPropietario)
        {
            if (!idPropietario.HasValue || idPropietario.Value <= 0)
            {
                return Json(new List<object>());
            }

            var lista = _repoInmueble.ObtenerPorPropietario(idPropietario.Value);
            var res = lista.Select(i => new
            {
                i.IdInmueble,
                i.Direccion,
                Tipo = i.Tipo?.Descripcion ?? "Sin tipo",
                i.Cupo,
                i.PrecioDia,
                i.PorcentajeReserva,
                i.Disponible,
                i.Portada
            });
            return Json(res);
        }

        // ==========================================
        // INFORME 3: Inmuebles más reservados en los últimos 365 días
        // ==========================================
        [HttpGet]
        public IActionResult ApiInmueblesMasReservados(int dias = 365)
        {
            var ranking = _repoInmueble.ObtenerMasReservados(dias);
            return Json(ranking);
        }

        // ==========================================
        // INFORME 4: Inmuebles sin reservas en los últimos X días
        // ==========================================
        [HttpGet]
        public IActionResult ApiInmueblesSinReservas(int dias = 30)
        {
            var lista = _repoInmueble.ObtenerSinReservas(dias);
            var res = lista.Select(i => new
            {
                i.IdInmueble,
                i.Direccion,
                Tipo = i.Tipo?.Descripcion ?? "Sin tipo",
                i.Cupo,
                i.PrecioDia,
                i.Disponible,
                i.Portada,
                Duenio = $"{i.Duenio?.Nombre} {i.Duenio?.Apellido}"
            });
            return Json(res);
        }

        // ==========================================
        // INFORME 5: Reservas vigentes (Filtro por rango de fechas)
        // ==========================================
        [HttpGet]
        public IActionResult ApiReservasVigentes(DateTime? desde, DateTime? hasta)
        {
            var lista = _repoReserva.ObtenerVigentes(desde, hasta);
            var res = lista.Select(r => new
            {
                r.IdReserva,
                Inquilino = $"{r.Inquilino?.Nombre} {r.Inquilino?.Apellido}",
                DniInquilino = r.Inquilino?.Dni ?? "",
                Inmueble = r.Inmueble?.Direccion ?? "",
                TipoInmueble = r.Inmueble?.Tipo?.Descripcion ?? "",
                FechaDesde = r.FechaDesde.ToString("dd/MM/yyyy"),
                FechaHasta = r.FechaHasta.ToString("dd/MM/yyyy"),
                Dias = r.CantidadDias,
                r.MontoTotal,
                r.Estado
            });
            return Json(res);
        }

        // ==========================================
        // INFORME 6: Reservas que terminan en los próximos X días
        // ==========================================
        [HttpGet]
        public IActionResult ApiReservasPorVencer(int dias = 7)
        {
            var lista = _repoReserva.ObtenerPorVencer(dias);
            var res = lista.Select(r => new
            {
                r.IdReserva,
                Inquilino = $"{r.Inquilino?.Nombre} {r.Inquilino?.Apellido}",
                TelefonoInquilino = r.Inquilino?.Telefono ?? "",
                Inmueble = r.Inmueble?.Direccion ?? "",
                FechaDesde = r.FechaDesde.ToString("dd/MM/yyyy"),
                FechaHasta = r.FechaHasta.ToString("dd/MM/yyyy"),
                DiasRestantes = Math.Max(0, (r.FechaHasta.Date - DateTime.Today).Days),
                r.MontoTotal,
                r.Estado
            });
            return Json(res);
        }

        // ==========================================
        // INFORME 7: Pagos realizados para una reserva particular
        // ==========================================
        [HttpGet]
        public IActionResult ApiPagosPorReserva(int? idReserva)
        {
            if (!idReserva.HasValue || idReserva.Value <= 0)
            {
                return Json(new { pagos = new List<object>(), reserva = (object?)null });
            }

            var reserva = _repoReserva.ObtenerPorId(idReserva.Value);
            if (reserva == null)
            {
                return Json(new { pagos = new List<object>(), reserva = (object?)null });
            }

            var pagos = _repoPago.ObtenerPorReserva(idReserva.Value);
            decimal totalAbonado = pagos.Where(p => p.Estado == "Activo").Sum(p => p.Importe);
            decimal saldo = (reserva.MontoTotal + reserva.Multa) - totalAbonado;

            var infoReserva = new
            {
                reserva.IdReserva,
                Inquilino = $"{reserva.Inquilino?.Nombre} {reserva.Inquilino?.Apellido}",
                Inmueble = reserva.Inmueble?.Direccion,
                FechaDesde = reserva.FechaDesde.ToString("dd/MM/yyyy"),
                FechaHasta = reserva.FechaHasta.ToString("dd/MM/yyyy"),
                reserva.MontoTotal,
                reserva.Multa,
                TotalAbonado = totalAbonado,
                Saldo = Math.Max(0, saldo),
                reserva.Estado
            };

            var listaPagos = pagos.Select(p => new
            {
                p.IdPago,
                p.NumeroPago,
                FechaPago = p.FechaPago.ToString("dd/MM/yyyy HH:mm"),
                p.Importe,
                p.Concepto,
                p.Estado,
                CreadoPor = p.UsuarioCreador != null ? $"{p.UsuarioCreador.Nombre} {p.UsuarioCreador.Apellido}" : "Sistema"
            });

            return Json(new { pagos = listaPagos, reserva = infoReserva });
        }

        // ==========================================
        // INFORME 8: Inmuebles libres entre dos fechas posibles
        // ==========================================
        [HttpGet]
        public IActionResult ApiInmueblesLibresEntreFechas(DateTime? desde, DateTime? hasta)
        {
            if (!desde.HasValue || !hasta.HasValue || hasta.Value <= desde.Value)
            {
                return Json(new List<object>());
            }

            var lista = _repoInmueble.ObtenerLibresEntreFechas(desde.Value, hasta.Value);
            var res = lista.Select(i => new
            {
                i.IdInmueble,
                i.Direccion,
                Tipo = i.Tipo?.Descripcion ?? "Sin tipo",
                i.Cupo,
                i.PrecioDia,
                TotalEstadia = (hasta.Value - desde.Value).Days * i.PrecioDia,
                i.PorcentajeReserva,
                MontoSenia = Math.Round(((hasta.Value - desde.Value).Days * i.PrecioDia) * (i.PorcentajeReserva / 100m), 2),
                i.Portada,
                Duenio = $"{i.Duenio?.Nombre} {i.Duenio?.Apellido}"
            });
            return Json(res);
        }
    }
}
