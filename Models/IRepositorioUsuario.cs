namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models
{
    public interface IRepositorioUsuario
    {
        int Alta(Usuario usuario);
        int Baja(int id);
        int Modificacion(Usuario usuario);
        Usuario? ObtenerPorId(int id);
        Usuario? ObtenerPorEmail(string email);
        IList<Usuario> ObtenerTodos();
        ListaPaginada<Usuario> ObtenerPaginado(string? filtro, int pagina, int tamano = 5);
        int ActualizarPerfil(int id, string nombre, string apellido, string email, string? avatarUrl);
        int CambiarPassword(int id, string nuevoPasswordHash);
    }
}