namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models
{
    public interface IRepositorioTipoInmueble
    {
        int Alta(TipoInmueble tipo);
        int Baja(int id);
        int Modificacion(TipoInmueble tipo);
        TipoInmueble? ObtenerPorId(int id);
        IList<TipoInmueble> ObtenerTodos();
    }
}
