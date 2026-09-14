using MySqlConnector;

namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models
{
    public abstract class RepositorioBase
    {
        protected readonly string ConnectionString;

        protected RepositorioBase(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no fue encontrada.");
        }

        protected ListaPaginada<T> ConsultarPaginado<T>(
            string selectColumnas,
            string fromWhere,
            string orderBy,
            Action<MySqlCommand> cargarParametros,
            Func<MySqlDataReader, T> mapear,
            string? filtro,
            int pagina,
            int tamano = 5)
        {
            if (pagina < 1) pagina = 1;
            if (tamano < 1) tamano = 5;

            var resultado = new ListaPaginada<T>
            {
                Pagina = pagina,
                Tamano = tamano,
                Filtro = filtro ?? ""
            };

            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                using (var cmdCount = new MySqlCommand($"SELECT COUNT(*) {fromWhere}", connection))
                {
                    cargarParametros(cmdCount);
                    resultado.Total = Convert.ToInt32(cmdCount.ExecuteScalar());
                }

                int offset = (pagina - 1) * tamano;
                if (resultado.Total > 0 && offset >= resultado.Total)
                {
                    pagina = resultado.TotalPaginas;
                    resultado.Pagina = pagina;
                    offset = (pagina - 1) * tamano;
                }

                using (var cmd = new MySqlCommand($"{selectColumnas} {fromWhere} {orderBy} LIMIT @offset, @tamano", connection))
                {
                    cargarParametros(cmd);
                    cmd.Parameters.AddWithValue("@offset", offset);
                    cmd.Parameters.AddWithValue("@tamano", tamano);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            resultado.Items.Add(mapear(reader));
                        }
                    }
                }
            }

            return resultado;
        }
    }
}
