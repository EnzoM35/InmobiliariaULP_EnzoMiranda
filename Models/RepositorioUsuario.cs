using MySqlConnector;

namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models
{
    public class RepositorioUsuario : RepositorioBase, IRepositorioUsuario
    {
        public RepositorioUsuario(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Usuario usuario)
        {
            int res = -1;
            using (var connection = new MySqlConnection(ConnectionString))
            {
                string sql = @"INSERT INTO Usuarios 
                               (Nombre, Apellido, Email, PasswordHash, Rol, AvatarUrl, Activo) 
                               VALUES 
                               (@nombre, @apellido, @email, @passwordHash, @rol, @avatarUrl, 1);
                               SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", usuario.Nombre);
                    command.Parameters.AddWithValue("@apellido", usuario.Apellido);
                    command.Parameters.AddWithValue("@email", usuario.Email);
                    command.Parameters.AddWithValue("@passwordHash", usuario.PasswordHash);
                    command.Parameters.AddWithValue("@rol", usuario.Rol);
                    command.Parameters.AddWithValue("@avatarUrl", (object?)usuario.AvatarUrl ?? DBNull.Value);

                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    usuario.IdUsuario = res;
                }
            }
            return res;
        }

        public int Baja(int id)
        {
            int res = -1;
            using (var connection = new MySqlConnection(ConnectionString))
            {
                string sql = "UPDATE Usuarios SET Activo = 0 WHERE IdUsuario = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public int Modificacion(Usuario usuario)
        {
            int res = -1;
            using (var connection = new MySqlConnection(ConnectionString))
            {
                string sql = @"UPDATE Usuarios 
                               SET Nombre = @nombre, Apellido = @apellido, Email = @email, 
                                   Rol = @rol, AvatarUrl = @avatarUrl
                               WHERE IdUsuario = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", usuario.Nombre);
                    command.Parameters.AddWithValue("@apellido", usuario.Apellido);
                    command.Parameters.AddWithValue("@email", usuario.Email);
                    command.Parameters.AddWithValue("@rol", usuario.Rol);
                    command.Parameters.AddWithValue("@avatarUrl", (object?)usuario.AvatarUrl ?? DBNull.Value);
                    command.Parameters.AddWithValue("@id", usuario.IdUsuario);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public Usuario? ObtenerPorId(int id)
        {
            Usuario? u = null;
            using (var connection = new MySqlConnection(ConnectionString))
            {
                string sql = "SELECT IdUsuario, Nombre, Apellido, Email, PasswordHash, Rol, AvatarUrl, Activo FROM Usuarios WHERE IdUsuario = @id AND Activo = 1";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            u = MapFromReader(reader);
                        }
                    }
                }
            }
            return u;
        }

        public Usuario? ObtenerPorEmail(string email)
        {
            Usuario? u = null;
            using (var connection = new MySqlConnection(ConnectionString))
            {
                string sql = "SELECT IdUsuario, Nombre, Apellido, Email, PasswordHash, Rol, AvatarUrl, Activo FROM Usuarios WHERE Email = @email AND Activo = 1";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@email", email);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            u = MapFromReader(reader);
                        }
                    }
                }
            }
            return u;
        }

        public IList<Usuario> ObtenerTodos()
        {
            var lista = new List<Usuario>();
            using (var connection = new MySqlConnection(ConnectionString))
            {
                string sql = "SELECT IdUsuario, Nombre, Apellido, Email, PasswordHash, Rol, AvatarUrl, Activo FROM Usuarios WHERE Activo = 1 ORDER BY Apellido, Nombre";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapFromReader(reader));
                        }
                    }
                }
            }
            return lista;
        }

        public ListaPaginada<Usuario> ObtenerPaginado(string? filtro, int pagina, int tamano = 5)
        {
            string fromWhere = "FROM Usuarios WHERE Activo = 1";
            if (!string.IsNullOrWhiteSpace(filtro))
            {
                fromWhere += " AND (Nombre LIKE @filtro OR Apellido LIKE @filtro OR Email LIKE @filtro OR Rol LIKE @filtro)";
            }

            return ConsultarPaginado(
                "SELECT IdUsuario, Nombre, Apellido, Email, PasswordHash, Rol, AvatarUrl, Activo",
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

        public int ActualizarPerfil(int id, string nombre, string apellido, string email, string? avatarUrl)
        {
            int res = -1;
            using (var connection = new MySqlConnection(ConnectionString))
            {
                string sql = @"UPDATE Usuarios 
                               SET Nombre = @nombre, Apellido = @apellido, Email = @email, AvatarUrl = @avatarUrl 
                               WHERE IdUsuario = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", nombre);
                    command.Parameters.AddWithValue("@apellido", apellido);
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@avatarUrl", (object?)avatarUrl ?? DBNull.Value);
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public int CambiarPassword(int id, string nuevoPasswordHash)
        {
            int res = -1;
            using (var connection = new MySqlConnection(ConnectionString))
            {
                string sql = "UPDATE Usuarios SET PasswordHash = @passwordHash WHERE IdUsuario = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@passwordHash", nuevoPasswordHash);
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        private static Usuario MapFromReader(MySqlDataReader reader)
        {
            return new Usuario
            {
                IdUsuario = reader.GetInt32("IdUsuario"),
                Nombre = reader.GetString("Nombre"),
                Apellido = reader.GetString("Apellido"),
                Email = reader.GetString("Email"),
                PasswordHash = reader.GetString("PasswordHash"),
                Rol = reader.GetString("Rol"),
                AvatarUrl = reader.IsDBNull(reader.GetOrdinal("AvatarUrl")) ? null : reader.GetString("AvatarUrl"),
                Activo = reader.GetBoolean("Activo")
            };
        }
    }
}