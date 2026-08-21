namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models
{
    public interface IRepositorioInquilino
    {
        int Alta(Inquilino inquilino);
        int Baja(int id);
        int Modificacion(Inquilino inquilino);
        Inquilino? ObtenerPorId(int id);
        IList<Inquilino> ObtenerTodos();
    }
}
