namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models
{
    public interface IRepositorioPago
    {
        int Alta(Pago pago);
        int Modificar(Pago pago); // Solo modifica Concepto
        int Anular(int idPago, int idUsuarioAnulador); // Baja logica / Anulacion
        Pago? ObtenerPorId(int id);
        IList<Pago> ObtenerPorReserva(int idReserva);
        ListaPaginada<Pago> ObtenerPaginado(string? filtro, int pagina, int tamano = 10);
        int ObtenerSiguienteNumeroPago(int idReserva);
        decimal ObtenerTotalAbonadoPorReserva(int idReserva);
    }
}
