using MySqlConnector;

namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models
{
    public class RepositorioReserva : IRepositorioReserva
    {
        private readonly string _connectionString;

        public RepositorioReserva(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no fue encontrada.");
        }

        public int Alta(Reserva reserva)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                string sql = @"INSERT INTO Reservas 
                               (IdInquilino, IdInmueble, FechaDesde, FechaHasta, PrecioPorDia, MontoTotal, FechaTerminacion, Multa, Estado, Activo) 
                               VALUES 
                               (@idInquilino, @idInmueble, @fechaDesde, @fechaHasta, @precioPorDia, @montoTotal, @fechaTerminacion, @multa, @estado, 1);
                               SELECT LAST_INSERT_ID();";
                
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("@idInquilino", reserva.IdInquilino);
                    command.Parameters.AddWithValue("@idInmueble", reserva.IdInmueble);
                    command.Parameters.AddWithValue("@fechaDesde", reserva.FechaDesde);
                    command.Parameters.AddWithValue("@fechaHasta", reserva.FechaHasta);
                    command.Parameters.AddWithValue("@precioPorDia", reserva.PrecioPorDia);
                    command.Parameters.AddWithValue("@montoTotal", reserva.MontoTotal);
                    command.Parameters.AddWithValue("@fechaTerminacion", reserva.FechaTerminacion.HasValue ? reserva.FechaTerminacion.Value : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@multa", reserva.Multa);
                    command.Parameters.AddWithValue("@estado", string.IsNullOrEmpty(reserva.Estado) ? "Vigente" : reserva.Estado);

                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    reserva.IdReserva = res;
                }
            }
            return res;
        }

        public int Baja(int id)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                string sql = "UPDATE Reservas SET Activo = 0, Estado = 'Anulada' WHERE IdReserva = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public int Modificacion(Reserva reserva)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                string sql = @"UPDATE Reservas 
                               SET IdInquilino = @idInquilino,
                                   IdInmueble = @idInmueble,
                                   FechaDesde = @fechaDesde,
                                   FechaHasta = @fechaHasta,
                                   PrecioPorDia = @precioPorDia,
                                   MontoTotal = @montoTotal,
                                   FechaTerminacion = @fechaTerminacion,
                                   Multa = @multa,
                                   Estado = @estado 
                               WHERE IdReserva = @id";
                
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idInquilino", reserva.IdInquilino);
                    command.Parameters.AddWithValue("@idInmueble", reserva.IdInmueble);
                    command.Parameters.AddWithValue("@fechaDesde", reserva.FechaDesde);
                    command.Parameters.AddWithValue("@fechaHasta", reserva.FechaHasta);
                    command.Parameters.AddWithValue("@precioPorDia", reserva.PrecioPorDia);
                    command.Parameters.AddWithValue("@montoTotal", reserva.MontoTotal);
                    command.Parameters.AddWithValue("@fechaTerminacion", reserva.FechaTerminacion.HasValue ? reserva.FechaTerminacion.Value : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@multa", reserva.Multa);
                    command.Parameters.AddWithValue("@estado", string.IsNullOrEmpty(reserva.Estado) ? "Vigente" : reserva.Estado);
                    command.Parameters.AddWithValue("@id", reserva.IdReserva);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public Reserva? ObtenerPorId(int id)
        {
            Reserva? reserva = null;
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                string sql = @"SELECT r.IdReserva, r.IdInquilino, r.IdInmueble, r.FechaDesde, r.FechaHasta, 
                                      r.PrecioPorDia, r.MontoTotal, r.FechaTerminacion, r.Multa, r.Estado, r.Activo,
                                      iq.Nombre AS InquilinoNombre, iq.Apellido AS InquilinoApellido, iq.Dni AS InquilinoDni,
                                      iq.Telefono AS InquilinoTelefono, iq.Email AS InquilinoEmail,
                                      im.Direccion AS InmuebleDireccion, im.PrecioDia AS InmueblePrecioDia,
                                      im.Cupo AS InmuebleCupo, im.Disponible AS InmuebleDisponible,
                                      p.Nombre AS DuenioNombre, p.Apellido AS DuenioApellido,
                                      t.Descripcion AS TipoDescripcion
                               FROM Reservas r
                               INNER JOIN Inquilinos iq ON r.IdInquilino = iq.IdInquilino
                               INNER JOIN Inmuebles im ON r.IdInmueble = im.IdInmueble
                               INNER JOIN Propietarios p ON im.IdPropietario = p.IdPropietario
                               INNER JOIN TiposInmueble t ON im.IdTipoInmueble = t.IdTipoInmueble
                               WHERE r.IdReserva = @id AND r.Activo = 1";
                
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            reserva = MapFromReader(reader);
                        }
                    }
                }
            }
            return reserva;
        }

        public IList<Reserva> ObtenerTodos()
        {
            var res = new List<Reserva>();
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                string sql = @"SELECT r.IdReserva, r.IdInquilino, r.IdInmueble, r.FechaDesde, r.FechaHasta, 
                                      r.PrecioPorDia, r.MontoTotal, r.FechaTerminacion, r.Multa, r.Estado, r.Activo,
                                      iq.Nombre AS InquilinoNombre, iq.Apellido AS InquilinoApellido, iq.Dni AS InquilinoDni,
                                      iq.Telefono AS InquilinoTelefono, iq.Email AS InquilinoEmail,
                                      im.Direccion AS InmuebleDireccion, im.PrecioDia AS InmueblePrecioDia,
                                      im.Cupo AS InmuebleCupo, im.Disponible AS InmuebleDisponible,
                                      p.Nombre AS DuenioNombre, p.Apellido AS DuenioApellido,
                                      t.Descripcion AS TipoDescripcion
                               FROM Reservas r
                               INNER JOIN Inquilinos iq ON r.IdInquilino = iq.IdInquilino
                               INNER JOIN Inmuebles im ON r.IdInmueble = im.IdInmueble
                               INNER JOIN Propietarios p ON im.IdPropietario = p.IdPropietario
                               INNER JOIN TiposInmueble t ON im.IdTipoInmueble = t.IdTipoInmueble
                               WHERE r.Activo = 1
                               ORDER BY r.FechaDesde DESC";
                
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

        public IList<Reserva> ObtenerPorInmueble(int idInmueble)
        {
            var res = new List<Reserva>();
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                string sql = @"SELECT r.IdReserva, r.IdInquilino, r.IdInmueble, r.FechaDesde, r.FechaHasta, 
                                      r.PrecioPorDia, r.MontoTotal, r.FechaTerminacion, r.Multa, r.Estado, r.Activo,
                                      iq.Nombre AS InquilinoNombre, iq.Apellido AS InquilinoApellido, iq.Dni AS InquilinoDni,
                                      iq.Telefono AS InquilinoTelefono, iq.Email AS InquilinoEmail,
                                      im.Direccion AS InmuebleDireccion, im.PrecioDia AS InmueblePrecioDia,
                                      im.Cupo AS InmuebleCupo, im.Disponible AS InmuebleDisponible,
                                      p.Nombre AS DuenioNombre, p.Apellido AS DuenioApellido,
                                      t.Descripcion AS TipoDescripcion
                               FROM Reservas r
                               INNER JOIN Inquilinos iq ON r.IdInquilino = iq.IdInquilino
                               INNER JOIN Inmuebles im ON r.IdInmueble = im.IdInmueble
                               INNER JOIN Propietarios p ON im.IdPropietario = p.IdPropietario
                               INNER JOIN TiposInmueble t ON im.IdTipoInmueble = t.IdTipoInmueble
                               WHERE r.Activo = 1 AND r.IdInmueble = @idInmueble
                               ORDER BY r.FechaDesde DESC";
                
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idInmueble", idInmueble);
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

        public IList<Reserva> ObtenerPorInquilino(int idInquilino)
        {
            var res = new List<Reserva>();
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                string sql = @"SELECT r.IdReserva, r.IdInquilino, r.IdInmueble, r.FechaDesde, r.FechaHasta, 
                                      r.PrecioPorDia, r.MontoTotal, r.FechaTerminacion, r.Multa, r.Estado, r.Activo,
                                      iq.Nombre AS InquilinoNombre, iq.Apellido AS InquilinoApellido, iq.Dni AS InquilinoDni,
                                      iq.Telefono AS InquilinoTelefono, iq.Email AS InquilinoEmail,
                                      im.Direccion AS InmuebleDireccion, im.PrecioDia AS InmueblePrecioDia,
                                      im.Cupo AS InmuebleCupo, im.Disponible AS InmuebleDisponible,
                                      p.Nombre AS DuenioNombre, p.Apellido AS DuenioApellido,
                                      t.Descripcion AS TipoDescripcion
                               FROM Reservas r
                               INNER JOIN Inquilinos iq ON r.IdInquilino = iq.IdInquilino
                               INNER JOIN Inmuebles im ON r.IdInmueble = im.IdInmueble
                               INNER JOIN Propietarios p ON im.IdPropietario = p.IdPropietario
                               INNER JOIN TiposInmueble t ON im.IdTipoInmueble = t.IdTipoInmueble
                               WHERE r.Activo = 1 AND r.IdInquilino = @idInquilino
                               ORDER BY r.FechaDesde DESC";
                
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idInquilino", idInquilino);
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

        public bool ExisteSuperposicion(int idInmueble, DateTime desde, DateTime hasta, int? idReservaExcluir = null)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                string sql = @"SELECT COUNT(*) 
                               FROM Reservas 
                               WHERE IdInmueble = @idInmueble 
                                 AND Activo = 1 
                                 AND Estado != 'Anulada' 
                                 AND Estado != 'Cancelada'
                                 AND FechaDesde <= @hasta 
                                 AND FechaHasta >= @desde";
                
                if (idReservaExcluir.HasValue)
                {
                    sql += " AND IdReserva != @idReservaExcluir";
                }

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idInmueble", idInmueble);
                    command.Parameters.AddWithValue("@desde", desde.Date);
                    command.Parameters.AddWithValue("@hasta", hasta.Date);
                    if (idReservaExcluir.HasValue)
                    {
                        command.Parameters.AddWithValue("@idReservaExcluir", idReservaExcluir.Value);
                    }

                    connection.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        private Reserva MapFromReader(MySqlDataReader reader)
        {
            return new Reserva
            {
                IdReserva = reader.GetInt32("IdReserva"),
                IdInquilino = reader.GetInt32("IdInquilino"),
                IdInmueble = reader.GetInt32("IdInmueble"),
                FechaDesde = reader.GetDateTime("FechaDesde"),
                FechaHasta = reader.GetDateTime("FechaHasta"),
                PrecioPorDia = reader.GetDecimal("PrecioPorDia"),
                MontoTotal = reader.GetDecimal("MontoTotal"),
                FechaTerminacion = reader.IsDBNull(reader.GetOrdinal("FechaTerminacion")) ? null : reader.GetDateTime("FechaTerminacion"),
                Multa = reader.GetDecimal("Multa"),
                Estado = reader.GetString("Estado"),
                Activo = reader.GetBoolean("Activo"),
                Inquilino = new Inquilino
                {
                    IdInquilino = reader.GetInt32("IdInquilino"),
                    Nombre = reader.GetString("InquilinoNombre"),
                    Apellido = reader.GetString("InquilinoApellido"),
                    Dni = reader.GetString("InquilinoDni"),
                    Telefono = reader.IsDBNull(reader.GetOrdinal("InquilinoTelefono")) ? null : reader.GetString("InquilinoTelefono"),
                    Email = reader.GetString("InquilinoEmail")
                },
                Inmueble = new Inmueble
                {
                    IdInmueble = reader.GetInt32("IdInmueble"),
                    Direccion = reader.GetString("InmuebleDireccion"),
                    PrecioDia = reader.GetDecimal("InmueblePrecioDia"),
                    Cupo = reader.GetInt32("InmuebleCupo"),
                    Disponible = reader.GetBoolean("InmuebleDisponible"),
                    Duenio = new Propietario
                    {
                        Nombre = reader.GetString("DuenioNombre"),
                        Apellido = reader.GetString("DuenioApellido")
                    },
                    Tipo = new TipoInmueble
                    {
                        Descripcion = reader.GetString("TipoDescripcion")
                    }
                }
            };
        }
    }
}
