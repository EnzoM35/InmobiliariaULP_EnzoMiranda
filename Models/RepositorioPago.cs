using MySqlConnector;

namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models
{
    public class RepositorioPago : RepositorioBase, IRepositorioPago
    {
        public RepositorioPago(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Pago pago)
        {
            int res = 0;
            using (var connection = new MySqlConnection(ConnectionString))
            {
                if (pago.NumeroPago <= 0)
                {
                    pago.NumeroPago = ObtenerSiguienteNumeroPago(pago.IdReserva);
                }

                string sql = @"INSERT INTO Pagos 
                    (IdReserva, NumeroPago, FechaPago, Importe, Concepto, Estado, IdUsuarioCreador, Activo) 
                    VALUES 
                    (@idReserva, @numeroPago, @fechaPago, @importe, @concepto, @estado, @idUsuarioCreador, 1);
                    SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idReserva", pago.IdReserva);
                    command.Parameters.AddWithValue("@numeroPago", pago.NumeroPago);
                    command.Parameters.AddWithValue("@fechaPago", pago.FechaPago == default ? DateTime.Now : pago.FechaPago);
                    command.Parameters.AddWithValue("@importe", pago.Importe);
                    command.Parameters.AddWithValue("@concepto", pago.Concepto);
                    command.Parameters.AddWithValue("@estado", string.IsNullOrEmpty(pago.Estado) ? "Activo" : pago.Estado);
                    command.Parameters.AddWithValue("@idUsuarioCreador", (object?)pago.IdUsuarioCreador ?? DBNull.Value);

                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    pago.IdPago = res;
                }
            }
            return res;
        }

        public int Modificar(Pago pago)
        {
            // Regla de negocio: en la edicion solo se permite modificar el concepto
            int res = 0;
            using (var connection = new MySqlConnection(ConnectionString))
            {
                string sql = @"UPDATE Pagos 
                    SET Concepto = @concepto 
                    WHERE IdPago = @idPago AND Activo = 1;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@concepto", pago.Concepto);
                    command.Parameters.AddWithValue("@idPago", pago.IdPago);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public int Anular(int idPago, int idUsuarioAnulador)
        {
            // Regla de negocio: eliminacion logica / anulacion de pago con registro del usuario
            int res = 0;
            using (var connection = new MySqlConnection(ConnectionString))
            {
                string sql = @"UPDATE Pagos 
                    SET Estado = 'Anulado', 
                        Activo = 0, 
                        IdUsuarioAnulador = @idUsuarioAnulador, 
                        FechaAnulacion = NOW() 
                    WHERE IdPago = @idPago;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idPago", idPago);
                    command.Parameters.AddWithValue("@idUsuarioAnulador", idUsuarioAnulador > 0 ? (object)idUsuarioAnulador : DBNull.Value);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public Pago? ObtenerPorId(int id)
        {
            Pago? pago = null;
            using (var connection = new MySqlConnection(ConnectionString))
            {
                string sql = @"SELECT p.IdPago, p.IdReserva, p.NumeroPago, p.FechaPago, p.Importe, p.Concepto, p.Estado, 
                                      p.IdUsuarioCreador, p.IdUsuarioAnulador, p.FechaAnulacion, p.Activo,
                                      r.FechaDesde, r.FechaHasta, r.MontoTotal, r.Multa, r.Estado AS ReservaEstado,
                                      iq.Nombre AS InquilinoNombre, iq.Apellido AS InquilinoApellido,
                                      im.Direccion AS InmuebleDireccion,
                                      uc.Nombre AS CreadorNombre, uc.Apellido AS CreadorApellido, uc.Email AS CreadorEmail,
                                      ua.Nombre AS AnuladorNombre, ua.Apellido AS AnuladorApellido, ua.Email AS AnuladorEmail
                               FROM Pagos p
                               INNER JOIN Reservas r ON p.IdReserva = r.IdReserva
                               INNER JOIN Inquilinos iq ON r.IdInquilino = iq.IdInquilino
                               INNER JOIN Inmuebles im ON r.IdInmueble = im.IdInmueble
                               LEFT JOIN Usuarios uc ON p.IdUsuarioCreador = uc.IdUsuario
                               LEFT JOIN Usuarios ua ON p.IdUsuarioAnulador = ua.IdUsuario
                               WHERE p.IdPago = @id;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            pago = MapearPago(reader);
                        }
                    }
                }
            }
            return pago;
        }

        public IList<Pago> ObtenerPorReserva(int idReserva)
        {
            var lista = new List<Pago>();
            using (var connection = new MySqlConnection(ConnectionString))
            {
                string sql = @"SELECT p.IdPago, p.IdReserva, p.NumeroPago, p.FechaPago, p.Importe, p.Concepto, p.Estado, 
                                      p.IdUsuarioCreador, p.IdUsuarioAnulador, p.FechaAnulacion, p.Activo,
                                      r.FechaDesde, r.FechaHasta, r.MontoTotal, r.Multa, r.Estado AS ReservaEstado,
                                      iq.Nombre AS InquilinoNombre, iq.Apellido AS InquilinoApellido,
                                      im.Direccion AS InmuebleDireccion,
                                      uc.Nombre AS CreadorNombre, uc.Apellido AS CreadorApellido, uc.Email AS CreadorEmail,
                                      ua.Nombre AS AnuladorNombre, ua.Apellido AS AnuladorApellido, ua.Email AS AnuladorEmail
                               FROM Pagos p
                               INNER JOIN Reservas r ON p.IdReserva = r.IdReserva
                               INNER JOIN Inquilinos iq ON r.IdInquilino = iq.IdInquilino
                               INNER JOIN Inmuebles im ON r.IdInmueble = im.IdInmueble
                               LEFT JOIN Usuarios uc ON p.IdUsuarioCreador = uc.IdUsuario
                               LEFT JOIN Usuarios ua ON p.IdUsuarioAnulador = ua.IdUsuario
                               WHERE p.IdReserva = @idReserva
                               ORDER BY p.NumeroPago ASC, p.FechaPago ASC;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idReserva", idReserva);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapearPago(reader));
                        }
                    }
                }
            }
            return lista;
        }

        public ListaPaginada<Pago> ObtenerPaginado(string? filtro, int pagina, int tamano = 10)
        {
            var resultado = new ListaPaginada<Pago>
            {
                Pagina = Math.Max(1, pagina),
                Tamano = tamano,
                Filtro = filtro ?? string.Empty
            };

            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                string whereClause = "WHERE 1=1";
                if (!string.IsNullOrWhiteSpace(filtro))
                {
                    whereClause += @" AND (
                        p.Concepto LIKE @f OR 
                        p.Estado LIKE @f OR 
                        iq.Nombre LIKE @f OR 
                        iq.Apellido LIKE @f OR 
                        im.Direccion LIKE @f OR
                        CAST(p.IdReserva AS CHAR) = @exactF
                    )";
                }

                string countSql = $@"SELECT COUNT(*) 
                                    FROM Pagos p
                                    INNER JOIN Reservas r ON p.IdReserva = r.IdReserva
                                    INNER JOIN Inquilinos iq ON r.IdInquilino = iq.IdInquilino
                                    INNER JOIN Inmuebles im ON r.IdInmueble = im.IdInmueble
                                    {whereClause};";

                using (var countCmd = new MySqlCommand(countSql, connection))
                {
                    if (!string.IsNullOrWhiteSpace(filtro))
                    {
                        countCmd.Parameters.AddWithValue("@f", $"%{filtro}%");
                        countCmd.Parameters.AddWithValue("@exactF", filtro.Trim());
                    }
                    resultado.Total = Convert.ToInt32(countCmd.ExecuteScalar());
                }

                int offset = (resultado.Pagina - 1) * resultado.Tamano;
                string sql = $@"SELECT p.IdPago, p.IdReserva, p.NumeroPago, p.FechaPago, p.Importe, p.Concepto, p.Estado, 
                                      p.IdUsuarioCreador, p.IdUsuarioAnulador, p.FechaAnulacion, p.Activo,
                                      r.FechaDesde, r.FechaHasta, r.MontoTotal, r.Multa, r.Estado AS ReservaEstado,
                                      iq.Nombre AS InquilinoNombre, iq.Apellido AS InquilinoApellido,
                                      im.Direccion AS InmuebleDireccion,
                                      uc.Nombre AS CreadorNombre, uc.Apellido AS CreadorApellido, uc.Email AS CreadorEmail,
                                      ua.Nombre AS AnuladorNombre, ua.Apellido AS AnuladorApellido, ua.Email AS AnuladorEmail
                               FROM Pagos p
                               INNER JOIN Reservas r ON p.IdReserva = r.IdReserva
                               INNER JOIN Inquilinos iq ON r.IdInquilino = iq.IdInquilino
                               INNER JOIN Inmuebles im ON r.IdInmueble = im.IdInmueble
                               LEFT JOIN Usuarios uc ON p.IdUsuarioCreador = uc.IdUsuario
                               LEFT JOIN Usuarios ua ON p.IdUsuarioAnulador = ua.IdUsuario
                               {whereClause}
                               ORDER BY p.FechaPago DESC, p.IdPago DESC
                               LIMIT @limit OFFSET @offset;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    if (!string.IsNullOrWhiteSpace(filtro))
                    {
                        command.Parameters.AddWithValue("@f", $"%{filtro}%");
                        command.Parameters.AddWithValue("@exactF", filtro.Trim());
                    }
                    command.Parameters.AddWithValue("@limit", resultado.Tamano);
                    command.Parameters.AddWithValue("@offset", offset);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            resultado.Items.Add(MapearPago(reader));
                        }
                    }
                }
            }

            return resultado;
        }

        public int ObtenerSiguienteNumeroPago(int idReserva)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                string sql = "SELECT COALESCE(MAX(NumeroPago), 0) + 1 FROM Pagos WHERE IdReserva = @idReserva;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idReserva", idReserva);
                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public decimal ObtenerTotalAbonadoPorReserva(int idReserva)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                string sql = "SELECT COALESCE(SUM(Importe), 0) FROM Pagos WHERE IdReserva = @idReserva AND Estado = 'Activo';";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idReserva", idReserva);
                    connection.Open();
                    return Convert.ToDecimal(command.ExecuteScalar());
                }
            }
        }

        private static Pago MapearPago(MySqlDataReader reader)
        {
            var pago = new Pago
            {
                IdPago = reader.GetInt32("IdPago"),
                IdReserva = reader.GetInt32("IdReserva"),
                NumeroPago = reader.GetInt32("NumeroPago"),
                FechaPago = reader.GetDateTime("FechaPago"),
                Importe = reader.GetDecimal("Importe"),
                Concepto = reader.GetString("Concepto"),
                Estado = reader.GetString("Estado"),
                IdUsuarioCreador = reader.IsDBNull(reader.GetOrdinal("IdUsuarioCreador")) ? null : reader.GetInt32("IdUsuarioCreador"),
                IdUsuarioAnulador = reader.IsDBNull(reader.GetOrdinal("IdUsuarioAnulador")) ? null : reader.GetInt32("IdUsuarioAnulador"),
                FechaAnulacion = reader.IsDBNull(reader.GetOrdinal("FechaAnulacion")) ? null : reader.GetDateTime("FechaAnulacion"),
                Activo = reader.GetBoolean("Activo"),
                Reserva = new Reserva
                {
                    IdReserva = reader.GetInt32("IdReserva"),
                    FechaDesde = reader.GetDateTime("FechaDesde"),
                    FechaHasta = reader.GetDateTime("FechaHasta"),
                    MontoTotal = reader.GetDecimal("MontoTotal"),
                    Multa = reader.GetDecimal("Multa"),
                    Estado = reader.GetString("ReservaEstado"),
                    Inquilino = new Inquilino
                    {
                        Nombre = reader.GetString("InquilinoNombre"),
                        Apellido = reader.GetString("InquilinoApellido")
                    },
                    Inmueble = new Inmueble
                    {
                        Direccion = reader.GetString("InmuebleDireccion")
                    }
                }
            };

            if (pago.IdUsuarioCreador.HasValue && !reader.IsDBNull(reader.GetOrdinal("CreadorNombre")))
            {
                pago.UsuarioCreador = new Usuario
                {
                    IdUsuario = pago.IdUsuarioCreador.Value,
                    Nombre = reader.GetString("CreadorNombre"),
                    Apellido = reader.GetString("CreadorApellido"),
                    Email = reader.GetString("CreadorEmail")
                };
            }

            if (pago.IdUsuarioAnulador.HasValue && !reader.IsDBNull(reader.GetOrdinal("AnuladorNombre")))
            {
                pago.UsuarioAnulador = new Usuario
                {
                    IdUsuario = pago.IdUsuarioAnulador.Value,
                    Nombre = reader.GetString("AnuladorNombre"),
                    Apellido = reader.GetString("AnuladorApellido"),
                    Email = reader.GetString("AnuladorEmail")
                };
            }

            return pago;
        }
    }
}
