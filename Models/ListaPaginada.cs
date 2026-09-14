namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models
{
    public class ListaPaginada<T>
    {
        public IList<T> Items { get; set; } = new List<T>();
        public int Pagina { get; set; } = 1;
        public int Tamano { get; set; } = 5;
        public int Total { get; set; }
        public string Filtro { get; set; } = "";
        public int TotalPaginas => Tamano == 0 ? 1 : (int)Math.Ceiling(Total / (double)Tamano);
    }
}
