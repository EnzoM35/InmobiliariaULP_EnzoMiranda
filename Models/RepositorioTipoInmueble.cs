using MySqlConnector;

namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models
{
    public class RepositorioTipoInmueble : IRepositorioTipoInmueble
    {
        private readonly string _connectionString;

        public RepositorioTipoInmueble(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no fue encontrada.");
        }

        public int Alta(TipoInmueble tipo)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                string sql = @"INSERT INTO TiposInmueble (Descripcion, Activo) 
                               VALUES (@descripcion, 1);
                               SELECT LAST_INSERT_ID();";
                
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("@descripcion", tipo.Descripcion);

                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    tipo.IdTipoInmueble = res;
                }
            }
            return res;
        }

        public int Baja(int id)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                string sql = "UPDATE TiposInmueble SET Activo = 0 WHERE IdTipoInmueble = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public int Modificacion(TipoInmueble tipo)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                string sql = @"UPDATE TiposInmueble 
                               SET Descripcion = @descripcion 
                               WHERE IdTipoInmueble = @id";
                
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@descripcion", tipo.Descripcion);
                    command.Parameters.AddWithValue("@id", tipo.IdTipoInmueble);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public TipoInmueble? ObtenerPorId(int id)
        {
            TipoInmueble? tipo = null;
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                string sql = "SELECT IdTipoInmueble, Descripcion, Activo FROM TiposInmueble WHERE IdTipoInmueble = @id AND Activo = 1";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            tipo = new TipoInmueble
                            {
                                IdTipoInmueble = reader.GetInt32("IdTipoInmueble"),
                                Descripcion = reader.GetString("Descripcion"),
                                Activo = reader.GetBoolean("Activo")
                            };
                        }
                    }
                }
            }
            return tipo;
        }

        public IList<TipoInmueble> ObtenerTodos()
        {
            var res = new List<TipoInmueble>();
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                string sql = "SELECT IdTipoInmueble, Descripcion, Activo FROM TiposInmueble WHERE Activo = 1";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            res.Add(new TipoInmueble
                            {
                                IdTipoInmueble = reader.GetInt32("IdTipoInmueble"),
                                Descripcion = reader.GetString("Descripcion"),
                                Activo = reader.GetBoolean("Activo")
                            });
                        }
                    }
                }
            }
            return res;
        }
    }
}
