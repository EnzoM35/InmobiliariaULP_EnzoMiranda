namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models
{
    public class InmuebleRankingDTO
    {
        public int IdInmueble { get; set; }
        public string Direccion { get; set; } = string.Empty;
        public string TipoDescripcion { get; set; } = string.Empty;
        public string DuenioCompleto { get; set; } = string.Empty;
        public string? Portada { get; set; }
        public decimal PrecioDia { get; set; }
        public int CantidadReservas { get; set; }
        public int TotalDiasReservados { get; set; }
        public decimal TotalRecaudado { get; set; }
    }
}
