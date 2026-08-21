namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models
{
    public interface IRepositorioPropietario
    {
        int Alta(Propietario propietario);
        int Baja(int id);
        int Modificacion(Propietario propietario);
        Propietario? ObtenerPorId(int id);
        IList<Propietario> ObtenerTodos();
    }
}
