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
        ListaPaginada<Inmueble> ObtenerPaginado(string? filtro, int pagina, int tamano = 5);

        // Módulo 4: Gestión de Galería de Imágenes
        int AltaImagen(ImagenInmueble imagen);
        int EliminarImagen(int idImagen);
        IList<ImagenInmueble> ObtenerImagenesPorInmueble(int idInmueble);
        ImagenInmueble? ObtenerImagenPorId(int idImagen);
        int EstablecerPortada(int idInmueble, int idImagen);

        // Módulo 5: Consultas de Informes
        IList<Inmueble> ObtenerPorDisponibilidad(bool? disponible);
        IList<Inmueble> ObtenerSinReservas(int dias);
        IList<Inmueble> ObtenerLibresEntreFechas(DateTime desde, DateTime hasta);
        IList<InmuebleRankingDTO> ObtenerMasReservados(int dias = 365);
    }
}
