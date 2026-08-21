using MySqlConnector;

namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models
{
    public class RepositorioInquilino : IRepositorioInquilino
    {
        private readonly string _connectionString;

        public RepositorioInquilino(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no fue encontrada.");
        }

        public int Alta(Inquilino inquilino)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                string sql = @"INSERT INTO Inquilinos (Nombre, Apellido, Dni, Telefono, Email) 
                               VALUES (@nombre, @apellido, @dni, @telefono, @email);
                               SELECT LAST_INSERT_ID();";
                
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("@nombre", inquilino.Nombre);
                    command.Parameters.AddWithValue("@apellido", inquilino.Apellido);
                    command.Parameters.AddWithValue("@dni", inquilino.Dni);
                    command.Parameters.AddWithValue("@telefono", inquilino.Telefono ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@email", inquilino.Email);

                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    inquilino.IdInquilino = res;
                }
            }
            return res;
        }

        public int Baja(int id)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                string sql = "UPDATE Inquilinos SET Activo = 0 WHERE IdInquilino = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public int Modificacion(Inquilino inquilino)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                string sql = @"UPDATE Inquilinos 
                               SET Nombre = @nombre, Apellido = @apellido, Dni = @dni, 
                                   Telefono = @telefono, Email = @email 
                               WHERE IdInquilino = @id";
                
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", inquilino.Nombre);
                    command.Parameters.AddWithValue("@apellido", inquilino.Apellido);
                    command.Parameters.AddWithValue("@dni", inquilino.Dni);
                    command.Parameters.AddWithValue("@telefono", inquilino.Telefono ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@email", inquilino.Email);
                    command.Parameters.AddWithValue("@id", inquilino.IdInquilino);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public Inquilino? ObtenerPorId(int id)
        {
            Inquilino? i = null;
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                string sql = "SELECT IdInquilino, Nombre, Apellido, Dni, Telefono, Email FROM Inquilinos WHERE IdInquilino = @id AND Activo = 1";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            i = new Inquilino
                            {
                                IdInquilino = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Apellido = reader.GetString(2),
                                Dni = reader.GetString(3),
                                Telefono = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Email = reader.GetString(5)
                            };
                        }
                    }
                }
            }
            return i;
        }

        public IList<Inquilino> ObtenerTodos()
        {
            var res = new List<Inquilino>();
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                string sql = "SELECT IdInquilino, Nombre, Apellido, Dni, Telefono, Email FROM Inquilinos WHERE Activo = 1";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            res.Add(new Inquilino
                            {
                                IdInquilino = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Apellido = reader.GetString(2),
                                Dni = reader.GetString(3),
                                Telefono = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Email = reader.GetString(5)
                            });
                        }
                    }
                }
            }
            return res;
        }
    }
}
