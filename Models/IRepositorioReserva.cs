namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models
{
    public interface IRepositorioReserva
    {
        int Alta(Reserva reserva);
        int Baja(int id);
        int Modificacion(Reserva reserva);
        Reserva? ObtenerPorId(int id);
        IList<Reserva> ObtenerTodos();
        IList<Reserva> ObtenerPorInmueble(int idInmueble);
        IList<Reserva> ObtenerPorInquilino(int idInquilino);
        bool ExisteSuperposicion(int idInmueble, DateTime desde, DateTime hasta, int? idReservaExcluir = null);
    }
}
