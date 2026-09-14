using MySqlConnector;

namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models
{
    public class RepositorioPropietario : RepositorioBase, IRepositorioPropietario
    {
        public RepositorioPropietario(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Propietario propietario)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                string sql = @"INSERT INTO Propietarios (Nombre, Apellido, Dni, Telefono, Email)
                               VALUES (@nombre, @apellido, @dni, @telefono, @email);
                               SELECT LAST_INSERT_ID();";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("@nombre", propietario.Nombre);
                    command.Parameters.AddWithValue("@apellido", propietario.Apellido);
                    command.Parameters.AddWithValue("@dni", propietario.Dni);
                    command.Parameters.AddWithValue("@telefono", propietario.Telefono ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@email", propietario.Email);

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
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
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
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                string sql = @"UPDATE Propietarios
                               SET Nombre = @nombre, Apellido = @apellido, Dni = @dni,
                                   Telefono = @telefono, Email = @email
                               WHERE IdPropietario = @id";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", propietario.Nombre);
                    command.Parameters.AddWithValue("@apellido", propietario.Apellido);
                    command.Parameters.AddWithValue("@dni", propietario.Dni);
                    command.Parameters.AddWithValue("@telefono", propietario.Telefono ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@email", propietario.Email);
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
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                string sql = "SELECT IdPropietario, Nombre, Apellido, Dni, Telefono, Email FROM Propietarios WHERE IdPropietario = @id AND Activo = 1";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            p = MapFromReader(reader);
                        }
                    }
                }
            }
            return p;
        }

        public IList<Propietario> ObtenerTodos()
        {
            var res = new List<Propietario>();
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                string sql = "SELECT IdPropietario, Nombre, Apellido, Dni, Telefono, Email FROM Propietarios WHERE Activo = 1";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            res.Add(MapFromReader(reader));
                        }
                    }
                }
            }
            return res;
        }

        public ListaPaginada<Propietario> ObtenerPaginado(string? filtro, int pagina, int tamano = 5)
        {
            string fromWhere = "FROM Propietarios WHERE Activo = 1";
            if (!string.IsNullOrWhiteSpace(filtro))
            {
                fromWhere += " AND (Nombre LIKE @filtro OR Apellido LIKE @filtro OR Dni LIKE @filtro OR Email LIKE @filtro)";
            }

            return ConsultarPaginado(
                "SELECT IdPropietario, Nombre, Apellido, Dni, Telefono, Email",
                fromWhere,
                "ORDER BY Apellido, Nombre",
                cmd =>
                {
                    if (!string.IsNullOrWhiteSpace(filtro))
                    {
                        cmd.Parameters.AddWithValue("@filtro", "%" + filtro.Trim() + "%");
                    }
                },
                MapFromReader,
                filtro,
                pagina,
                tamano);
        }

        private static Propietario MapFromReader(MySqlDataReader reader)
        {
            return new Propietario
            {
                IdPropietario = reader.GetInt32("IdPropietario"),
                Nombre = reader.GetString("Nombre"),
                Apellido = reader.GetString("Apellido"),
                Dni = reader.GetString("Dni"),
                Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString("Telefono"),
                Email = reader.GetString("Email")
            };
        }
    }
}
