using MySqlConnector;

namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models
{
    public class RepositorioPropietario : IRepositorioPropietario
    {
        private readonly string _connectionString;

        public RepositorioPropietario(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no fue encontrada.");
        }

        public int Alta(Propietario propietario)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                string sql = @"INSERT INTO Propietarios (Nombre, Apellido, Dni, Telefono, Email, Clave) 
                               VALUES (@nombre, @apellido, @dni, @telefono, @email, @clave);
                               SELECT LAST_INSERT_ID();";
                
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("@nombre", propietario.Nombre);
                    command.Parameters.AddWithValue("@apellido", propietario.Apellido);
                    command.Parameters.AddWithValue("@dni", propietario.Dni);
                    command.Parameters.AddWithValue("@telefono", propietario.Telefono ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@email", propietario.Email);
                    command.Parameters.AddWithValue("@clave", propietario.Clave);

                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    propietario.IdPropietario = res;
                }
            }
            return res;
        }

        public int Baja(int id)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                string sql = "UPDATE Propietarios SET Activo = 0 WHERE IdPropietario = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public int Modificacion(Propietario propietario)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                string sql = @"UPDATE Propietarios 
                               SET Nombre = @nombre, Apellido = @apellido, Dni = @dni, 
                                   Telefono = @telefono, Email = @email, Clave = @clave 
                               WHERE IdPropietario = @id";
                
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", propietario.Nombre);
                    command.Parameters.AddWithValue("@apellido", propietario.Apellido);
                    command.Parameters.AddWithValue("@dni", propietario.Dni);
                    command.Parameters.AddWithValue("@telefono", propietario.Telefono ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@email", propietario.Email);
                    command.Parameters.AddWithValue("@clave", propietario.Clave);
                    command.Parameters.AddWithValue("@id", propietario.IdPropietario);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public Propietario? ObtenerPorId(int id)
        {
            Propietario? p = null;
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                string sql = "SELECT IdPropietario, Nombre, Apellido, Dni, Telefono, Email, Clave FROM Propietarios WHERE IdPropietario = @id AND Activo = 1";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            p = new Propietario
                            {
                                IdPropietario = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Apellido = reader.GetString(2),
                                Dni = reader.GetString(3),
                                Telefono = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Email = reader.GetString(5),
                                Clave = reader.GetString(6)
                            };
                        }
                    }
                }
            }
            return p;
        }

        public IList<Propietario> ObtenerTodos()
        {
            var res = new List<Propietario>();
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                string sql = "SELECT IdPropietario, Nombre, Apellido, Dni, Telefono, Email, Clave FROM Propietarios WHERE Activo = 1";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            res.Add(new Propietario
                            {
                                IdPropietario = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Apellido = reader.GetString(2),
                                Dni = reader.GetString(3),
                                Telefono = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Email = reader.GetString(5),
                                Clave = reader.GetString(6)
                            });
                        }
                    }
                }
            }
            return res;
        }
    }
}
