namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models
{
    public interface IRepositorioInmueble
    {
        int Alta(Inmueble inmueble);
        int Baja(int id);
        int Modificacion(Inmueble inmueble);
        Inmueble? ObtenerPorId(int id);
        IList<Inmueble> ObtenerTodos();
        IList<Inmueble> ObtenerDisponibles();
        IList<Inmueble> ObtenerPorPropietario(int idPropietario);
    }
}
