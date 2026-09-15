namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models
{
    public interface IRepositorioReserva
    {
        int Alta(Reserva reserva);
        int Baja(int id);
        int Modificacion(Reserva reserva);
        int TerminarReserva(int idReserva, DateTime fechaTerminacion, decimal multa, int idUsuarioTerminador);
        Reserva? ObtenerPorId(int id);
        IList<Reserva> ObtenerTodos();
        IList<Reserva> ObtenerPorInmueble(int idInmueble);
        IList<Reserva> ObtenerPorInquilino(int idInquilino);
        IList<Reserva> ObtenerRenovaciones(int idReservaOrigen);
        bool ExisteSuperposicion(int idInmueble, DateTime desde, DateTime hasta, int? idReservaExcluir = null);
        ListaPaginada<Reserva> ObtenerPaginado(string? filtro, int pagina, int tamano = 5);

        // Módulo 5: Consultas de Informes
        IList<Reserva> ObtenerVigentes(DateTime? desde, DateTime? hasta);
        IList<Reserva> ObtenerPorVencer(int dias);
    }
}
