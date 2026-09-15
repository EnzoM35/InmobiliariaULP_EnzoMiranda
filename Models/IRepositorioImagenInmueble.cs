namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models
{
    public interface IRepositorioImagenInmueble
    {
        int Alta(ImagenInmueble imagen);
        int Baja(int id);
        IList<ImagenInmueble> ObtenerPorInmueble(int idInmueble);
    }
}
