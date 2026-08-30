using MySqlConnector;

namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models
{
    public class RepositorioInmueble : IRepositorioInmueble
    {
        private readonly string _connectionString;

        public RepositorioInmueble(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no fue encontrada.");
        }

        public int Alta(Inmueble inmueble)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                string sql = @"INSERT INTO Inmuebles 
                               (Direccion, Cupo, Latitud, Longitud, PrecioDia, PorcentajeReserva, Disponible, Portada, IdTipoInmueble, IdPropietario, Activo) 
                               VALUES 
                               (@direccion, @cupo, @latitud, @longitud, @precioDia, @porcentajeReserva, @disponible, @portada, @idTipoInmueble, @idPropietario, 1);
                               SELECT LAST_INSERT_ID();";
                
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("@direccion", inmueble.Direccion);
                    command.Parameters.AddWithValue("@cupo", inmueble.Cupo);
                    command.Parameters.AddWithValue("@latitud", inmueble.Latitud.HasValue ? inmueble.Latitud.Value : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@longitud", inmueble.Longitud.HasValue ? inmueble.Longitud.Value : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@precioDia", inmueble.PrecioDia);
                    command.Parameters.AddWithValue("@porcentajeReserva", inmueble.PorcentajeReserva);
                    command.Parameters.AddWithValue("@disponible", inmueble.Disponible ? 1 : 0);
                    command.Parameters.AddWithValue("@portada", string.IsNullOrEmpty(inmueble.Portada) ? (object)DBNull.Value : inmueble.Portada);
                    command.Parameters.AddWithValue("@idTipoInmueble", inmueble.IdTipoInmueble);
                    command.Parameters.AddWithValue("@idPropietario", inmueble.IdPropietario);

                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    inmueble.IdInmueble = res;
                }
            }
            return res;
        }

        public int Baja(int id)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                string sql = "UPDATE Inmuebles SET Activo = 0 WHERE IdInmueble = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public int Modificacion(Inmueble inmueble)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                string sql = @"UPDATE Inmuebles 
                               SET Direccion = @direccion,
                                   Cupo = @cupo,
                                   Latitud = @latitud,
                                   Longitud = @longitud,
                                   PrecioDia = @precioDia,
                                   PorcentajeReserva = @porcentajeReserva,
                                   Disponible = @disponible,
                                   Portada = @portada,
                                   IdTipoInmueble = @idTipoInmueble,
                                   IdPropietario = @idPropietario 
                               WHERE IdInmueble = @id";
                
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@direccion", inmueble.Direccion);
                    command.Parameters.AddWithValue("@cupo", inmueble.Cupo);
                    command.Parameters.AddWithValue("@latitud", inmueble.Latitud.HasValue ? inmueble.Latitud.Value : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@longitud", inmueble.Longitud.HasValue ? inmueble.Longitud.Value : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@precioDia", inmueble.PrecioDia);
                    command.Parameters.AddWithValue("@porcentajeReserva", inmueble.PorcentajeReserva);
                    command.Parameters.AddWithValue("@disponible", inmueble.Disponible ? 1 : 0);
                    command.Parameters.AddWithValue("@portada", string.IsNullOrEmpty(inmueble.Portada) ? (object)DBNull.Value : inmueble.Portada);
                    command.Parameters.AddWithValue("@idTipoInmueble", inmueble.IdTipoInmueble);
                    command.Parameters.AddWithValue("@idPropietario", inmueble.IdPropietario);
                    command.Parameters.AddWithValue("@id", inmueble.IdInmueble);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public Inmueble? ObtenerPorId(int id)
        {
            Inmueble? inmueble = null;
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                string sql = @"SELECT i.IdInmueble, i.Direccion, i.Cupo, i.Latitud, i.Longitud, i.PrecioDia, 
                                      i.PorcentajeReserva, i.Disponible, i.Portada, i.IdTipoInmueble, i.IdPropietario, i.Activo,
                                      t.Descripcion AS TipoDescripcion,
                                      p.Nombre AS PropietarioNombre, p.Apellido AS PropietarioApellido, p.Dni AS PropietarioDni,
                                      p.Telefono AS PropietarioTelefono, p.Email AS PropietarioEmail
                               FROM Inmuebles i
                               INNER JOIN TiposInmueble t ON i.IdTipoInmueble = t.IdTipoInmueble
                               INNER JOIN Propietarios p ON i.IdPropietario = p.IdPropietario
                               WHERE i.IdInmueble = @id AND i.Activo = 1";
                
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            inmueble = MapFromReader(reader);
                        }
                    }
                }
            }
            return inmueble;
        }

        public IList<Inmueble> ObtenerTodos()
        {
            var res = new List<Inmueble>();
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                string sql = @"SELECT i.IdInmueble, i.Direccion, i.Cupo, i.Latitud, i.Longitud, i.PrecioDia, 
                                      i.PorcentajeReserva, i.Disponible, i.Portada, i.IdTipoInmueble, i.IdPropietario, i.Activo,
                                      t.Descripcion AS TipoDescripcion,
                                      p.Nombre AS PropietarioNombre, p.Apellido AS PropietarioApellido, p.Dni AS PropietarioDni,
                                      p.Telefono AS PropietarioTelefono, p.Email AS PropietarioEmail
                               FROM Inmuebles i
                               INNER JOIN TiposInmueble t ON i.IdTipoInmueble = t.IdTipoInmueble
                               INNER JOIN Propietarios p ON i.IdPropietario = p.IdPropietario
                               WHERE i.Activo = 1";
                
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

        public IList<Inmueble> ObtenerDisponibles()
        {
            var res = new List<Inmueble>();
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                string sql = @"SELECT i.IdInmueble, i.Direccion, i.Cupo, i.Latitud, i.Longitud, i.PrecioDia, 
                                      i.PorcentajeReserva, i.Disponible, i.Portada, i.IdTipoInmueble, i.IdPropietario, i.Activo,
                                      t.Descripcion AS TipoDescripcion,
                                      p.Nombre AS PropietarioNombre, p.Apellido AS PropietarioApellido, p.Dni AS PropietarioDni,
                                      p.Telefono AS PropietarioTelefono, p.Email AS PropietarioEmail
                               FROM Inmuebles i
                               INNER JOIN TiposInmueble t ON i.IdTipoInmueble = t.IdTipoInmueble
                               INNER JOIN Propietarios p ON i.IdPropietario = p.IdPropietario
                               WHERE i.Activo = 1 AND i.Disponible = 1";
                
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

        public IList<Inmueble> ObtenerPorPropietario(int idPropietario)
        {
            var res = new List<Inmueble>();
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                string sql = @"SELECT i.IdInmueble, i.Direccion, i.Cupo, i.Latitud, i.Longitud, i.PrecioDia, 
                                      i.PorcentajeReserva, i.Disponible, i.Portada, i.IdTipoInmueble, i.IdPropietario, i.Activo,
                                      t.Descripcion AS TipoDescripcion,
                                      p.Nombre AS PropietarioNombre, p.Apellido AS PropietarioApellido, p.Dni AS PropietarioDni,
                                      p.Telefono AS PropietarioTelefono, p.Email AS PropietarioEmail
                               FROM Inmuebles i
                               INNER JOIN TiposInmueble t ON i.IdTipoInmueble = t.IdTipoInmueble
                               INNER JOIN Propietarios p ON i.IdPropietario = p.IdPropietario
                               WHERE i.Activo = 1 AND i.IdPropietario = @idPropietario";
                
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idPropietario", idPropietario);
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

        private Inmueble MapFromReader(MySqlDataReader reader)
        {
            return new Inmueble
            {
                IdInmueble = reader.GetInt32("IdInmueble"),
                Direccion = reader.GetString("Direccion"),
                Cupo = reader.GetInt32("Cupo"),
                Latitud = reader.IsDBNull(reader.GetOrdinal("Latitud")) ? null : reader.GetDecimal("Latitud"),
                Longitud = reader.IsDBNull(reader.GetOrdinal("Longitud")) ? null : reader.GetDecimal("Longitud"),
                PrecioDia = reader.GetDecimal("PrecioDia"),
                PorcentajeReserva = reader.GetDecimal("PorcentajeReserva"),
                Disponible = reader.GetBoolean("Disponible"),
                Portada = reader.IsDBNull(reader.GetOrdinal("Portada")) ? null : reader.GetString("Portada"),
                IdTipoInmueble = reader.GetInt32("IdTipoInmueble"),
                IdPropietario = reader.GetInt32("IdPropietario"),
                Activo = reader.GetBoolean("Activo"),
                Tipo = new TipoInmueble
                {
                    IdTipoInmueble = reader.GetInt32("IdTipoInmueble"),
                    Descripcion = reader.GetString("TipoDescripcion")
                },
                Duenio = new Propietario
                {
                    IdPropietario = reader.GetInt32("IdPropietario"),
                    Nombre = reader.GetString("PropietarioNombre"),
                    Apellido = reader.GetString("PropietarioApellido"),
                    Dni = reader.GetString("PropietarioDni"),
                    Telefono = reader.IsDBNull(reader.GetOrdinal("PropietarioTelefono")) ? null : reader.GetString("PropietarioTelefono"),
                    Email = reader.GetString("PropietarioEmail")
                }
            };
        }
    }
}
