using MySqlConnector;

namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models
{
    public class RepositorioReserva : RepositorioBase, IRepositorioReserva
    {
        public RepositorioReserva(IConfiguration configuration) : base(configuration) { }

        public int Alta(Reserva reserva)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                string sql = @"INSERT INTO Reservas 
                               (IdInquilino, IdInmueble, FechaDesde, FechaHasta, PrecioPorDia, MontoTotal, FechaTerminacion, Multa, Estado, IdUsuarioCreador, IdReservaOrigen, Activo) 
                               VALUES 
                               (@idInquilino, @idInmueble, @fechaDesde, @fechaHasta, @precioPorDia, @montoTotal, @fechaTerminacion, @multa, @estado, @idUsuarioCreador, @idReservaOrigen, 1);
                               SELECT LAST_INSERT_ID();";
                
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idInquilino", reserva.IdInquilino);
                    command.Parameters.AddWithValue("@idInmueble", reserva.IdInmueble);
                    command.Parameters.AddWithValue("@fechaDesde", reserva.FechaDesde);
                    command.Parameters.AddWithValue("@fechaHasta", reserva.FechaHasta);
                    command.Parameters.AddWithValue("@precioPorDia", reserva.PrecioPorDia);
                    command.Parameters.AddWithValue("@montoTotal", reserva.MontoTotal);
                    command.Parameters.AddWithValue("@fechaTerminacion", reserva.FechaTerminacion.HasValue ? (object)reserva.FechaTerminacion.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@multa", reserva.Multa);
                    command.Parameters.AddWithValue("@estado", string.IsNullOrEmpty(reserva.Estado) ? "Vigente" : reserva.Estado);
                    command.Parameters.AddWithValue("@idUsuarioCreador", (object?)reserva.IdUsuarioCreador ?? DBNull.Value);
                    command.Parameters.AddWithValue("@idReservaOrigen", (object?)reserva.IdReservaOrigen ?? DBNull.Value);

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
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
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
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
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
                                   Estado = @estado,
                                   IdUsuarioTerminador = @idUsuarioTerminador
                               WHERE IdReserva = @id";
                
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idInquilino", reserva.IdInquilino);
                    command.Parameters.AddWithValue("@idInmueble", reserva.IdInmueble);
                    command.Parameters.AddWithValue("@fechaDesde", reserva.FechaDesde);
                    command.Parameters.AddWithValue("@fechaHasta", reserva.FechaHasta);
                    command.Parameters.AddWithValue("@precioPorDia", reserva.PrecioPorDia);
                    command.Parameters.AddWithValue("@montoTotal", reserva.MontoTotal);
                    command.Parameters.AddWithValue("@fechaTerminacion", reserva.FechaTerminacion.HasValue ? (object)reserva.FechaTerminacion.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@multa", reserva.Multa);
                    command.Parameters.AddWithValue("@estado", string.IsNullOrEmpty(reserva.Estado) ? "Vigente" : reserva.Estado);
                    command.Parameters.AddWithValue("@idUsuarioTerminador", (object?)reserva.IdUsuarioTerminador ?? DBNull.Value);
                    command.Parameters.AddWithValue("@id", reserva.IdReserva);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public int TerminarReserva(int idReserva, DateTime fechaTerminacion, decimal multa, int idUsuarioTerminador)
        {
            int res = 0;
            using (var connection = new MySqlConnection(ConnectionString))
            {
                string sql = @"UPDATE Reservas 
                               SET FechaTerminacion = @fechaTerminacion,
                                   Multa = @multa,
                                   Estado = 'Terminada anticipadamente',
                                   IdUsuarioTerminador = @idUsuarioTerminador
                               WHERE IdReserva = @idReserva;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@fechaTerminacion", fechaTerminacion);
                    command.Parameters.AddWithValue("@multa", multa);
                    command.Parameters.AddWithValue("@idUsuarioTerminador", idUsuarioTerminador > 0 ? (object)idUsuarioTerminador : DBNull.Value);
                    command.Parameters.AddWithValue("@idReserva", idReserva);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public Reserva? ObtenerPorId(int id)
        {
            Reserva? reserva = null;
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                string sql = @"SELECT r.IdReserva, r.IdInquilino, r.IdInmueble, r.FechaDesde, r.FechaHasta, 
                                      r.PrecioPorDia, r.MontoTotal, r.FechaTerminacion, r.Multa, r.Estado, r.Activo,
                                      r.IdUsuarioCreador, r.IdUsuarioTerminador, r.IdReservaOrigen,
                                      iq.Nombre AS InquilinoNombre, iq.Apellido AS InquilinoApellido, iq.Dni AS InquilinoDni,
                                      iq.Telefono AS InquilinoTelefono, iq.Email AS InquilinoEmail,
                                      im.Direccion AS InmuebleDireccion, im.PrecioDia AS InmueblePrecioDia,
                                      im.Cupo AS InmuebleCupo, im.Disponible AS InmuebleDisponible,
                                      im.PorcentajeReserva AS InmueblePorcentajeReserva,
                                      p.Nombre AS DuenioNombre, p.Apellido AS DuenioApellido,
                                      t.Descripcion AS TipoDescripcion,
                                      uc.Nombre AS CreadorNombre, uc.Apellido AS CreadorApellido, uc.Email AS CreadorEmail,
                                      ut.Nombre AS TerminadorNombre, ut.Apellido AS TerminadorApellido, ut.Email AS TerminadorEmail
                               FROM Reservas r
                               INNER JOIN Inquilinos iq ON r.IdInquilino = iq.IdInquilino
                               INNER JOIN Inmuebles im ON r.IdInmueble = im.IdInmueble
                               INNER JOIN Propietarios p ON im.IdPropietario = p.IdPropietario
                               INNER JOIN TiposInmueble t ON im.IdTipoInmueble = t.IdTipoInmueble
                               LEFT JOIN Usuarios uc ON r.IdUsuarioCreador = uc.IdUsuario
                               LEFT JOIN Usuarios ut ON r.IdUsuarioTerminador = ut.IdUsuario
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
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                string sql = @"SELECT r.IdReserva, r.IdInquilino, r.IdInmueble, r.FechaDesde, r.FechaHasta, 
                                      r.PrecioPorDia, r.MontoTotal, r.FechaTerminacion, r.Multa, r.Estado, r.Activo,
                                      r.IdUsuarioCreador, r.IdUsuarioTerminador, r.IdReservaOrigen,
                                      iq.Nombre AS InquilinoNombre, iq.Apellido AS InquilinoApellido, iq.Dni AS InquilinoDni,
                                      iq.Telefono AS InquilinoTelefono, iq.Email AS InquilinoEmail,
                                      im.Direccion AS InmuebleDireccion, im.PrecioDia AS InmueblePrecioDia,
                                      im.Cupo AS InmuebleCupo, im.Disponible AS InmuebleDisponible,
                                      im.PorcentajeReserva AS InmueblePorcentajeReserva,
                                      p.Nombre AS DuenioNombre, p.Apellido AS DuenioApellido,
                                      t.Descripcion AS TipoDescripcion,
                                      uc.Nombre AS CreadorNombre, uc.Apellido AS CreadorApellido, uc.Email AS CreadorEmail,
                                      ut.Nombre AS TerminadorNombre, ut.Apellido AS TerminadorApellido, ut.Email AS TerminadorEmail
                               FROM Reservas r
                               INNER JOIN Inquilinos iq ON r.IdInquilino = iq.IdInquilino
                               INNER JOIN Inmuebles im ON r.IdInmueble = im.IdInmueble
                               INNER JOIN Propietarios p ON im.IdPropietario = p.IdPropietario
                               INNER JOIN TiposInmueble t ON im.IdTipoInmueble = t.IdTipoInmueble
                               LEFT JOIN Usuarios uc ON r.IdUsuarioCreador = uc.IdUsuario
                               LEFT JOIN Usuarios ut ON r.IdUsuarioTerminador = ut.IdUsuario
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
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                string sql = @"SELECT r.IdReserva, r.IdInquilino, r.IdInmueble, r.FechaDesde, r.FechaHasta, 
                                      r.PrecioPorDia, r.MontoTotal, r.FechaTerminacion, r.Multa, r.Estado, r.Activo,
                                      r.IdUsuarioCreador, r.IdUsuarioTerminador, r.IdReservaOrigen,
                                      iq.Nombre AS InquilinoNombre, iq.Apellido AS InquilinoApellido, iq.Dni AS InquilinoDni,
                                      iq.Telefono AS InquilinoTelefono, iq.Email AS InquilinoEmail,
                                      im.Direccion AS InmuebleDireccion, im.PrecioDia AS InmueblePrecioDia,
                                      im.Cupo AS InmuebleCupo, im.Disponible AS InmuebleDisponible,
                                      im.PorcentajeReserva AS InmueblePorcentajeReserva,
                                      p.Nombre AS DuenioNombre, p.Apellido AS DuenioApellido,
                                      t.Descripcion AS TipoDescripcion,
                                      uc.Nombre AS CreadorNombre, uc.Apellido AS CreadorApellido, uc.Email AS CreadorEmail,
                                      ut.Nombre AS TerminadorNombre, ut.Apellido AS TerminadorApellido, ut.Email AS TerminadorEmail
                               FROM Reservas r
                               INNER JOIN Inquilinos iq ON r.IdInquilino = iq.IdInquilino
                               INNER JOIN Inmuebles im ON r.IdInmueble = im.IdInmueble
                               INNER JOIN Propietarios p ON im.IdPropietario = p.IdPropietario
                               INNER JOIN TiposInmueble t ON im.IdTipoInmueble = t.IdTipoInmueble
                               LEFT JOIN Usuarios uc ON r.IdUsuarioCreador = uc.IdUsuario
                               LEFT JOIN Usuarios ut ON r.IdUsuarioTerminador = ut.IdUsuario
                               WHERE r.IdInmueble = @idInmueble AND r.Activo = 1
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
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                string sql = @"SELECT r.IdReserva, r.IdInquilino, r.IdInmueble, r.FechaDesde, r.FechaHasta, 
                                      r.PrecioPorDia, r.MontoTotal, r.FechaTerminacion, r.Multa, r.Estado, r.Activo,
                                      r.IdUsuarioCreador, r.IdUsuarioTerminador, r.IdReservaOrigen,
                                      iq.Nombre AS InquilinoNombre, iq.Apellido AS InquilinoApellido, iq.Dni AS InquilinoDni,
                                      iq.Telefono AS InquilinoTelefono, iq.Email AS InquilinoEmail,
                                      im.Direccion AS InmuebleDireccion, im.PrecioDia AS InmueblePrecioDia,
                                      im.Cupo AS InmuebleCupo, im.Disponible AS InmuebleDisponible,
                                      im.PorcentajeReserva AS InmueblePorcentajeReserva,
                                      p.Nombre AS DuenioNombre, p.Apellido AS DuenioApellido,
                                      t.Descripcion AS TipoDescripcion,
                                      uc.Nombre AS CreadorNombre, uc.Apellido AS CreadorApellido, uc.Email AS CreadorEmail,
                                      ut.Nombre AS TerminadorNombre, ut.Apellido AS TerminadorApellido, ut.Email AS TerminadorEmail
                               FROM Reservas r
                               INNER JOIN Inquilinos iq ON r.IdInquilino = iq.IdInquilino
                               INNER JOIN Inmuebles im ON r.IdInmueble = im.IdInmueble
                               INNER JOIN Propietarios p ON im.IdPropietario = p.IdPropietario
                               INNER JOIN TiposInmueble t ON im.IdTipoInmueble = t.IdTipoInmueble
                               LEFT JOIN Usuarios uc ON r.IdUsuarioCreador = uc.IdUsuario
                               LEFT JOIN Usuarios ut ON r.IdUsuarioTerminador = ut.IdUsuario
                               WHERE r.IdInquilino = @idInquilino AND r.Activo = 1
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

        public IList<Reserva> ObtenerRenovaciones(int idReservaOrigen)
        {
            var res = new List<Reserva>();
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                string sql = @"SELECT r.IdReserva, r.IdInquilino, r.IdInmueble, r.FechaDesde, r.FechaHasta, 
                                      r.PrecioPorDia, r.MontoTotal, r.FechaTerminacion, r.Multa, r.Estado, r.Activo,
                                      r.IdUsuarioCreador, r.IdUsuarioTerminador, r.IdReservaOrigen,
                                      iq.Nombre AS InquilinoNombre, iq.Apellido AS InquilinoApellido, iq.Dni AS InquilinoDni,
                                      iq.Telefono AS InquilinoTelefono, iq.Email AS InquilinoEmail,
                                      im.Direccion AS InmuebleDireccion, im.PrecioDia AS InmueblePrecioDia,
                                      im.Cupo AS InmuebleCupo, im.Disponible AS InmuebleDisponible,
                                      im.PorcentajeReserva AS InmueblePorcentajeReserva,
                                      p.Nombre AS DuenioNombre, p.Apellido AS DuenioApellido,
                                      t.Descripcion AS TipoDescripcion,
                                      uc.Nombre AS CreadorNombre, uc.Apellido AS CreadorApellido, uc.Email AS CreadorEmail,
                                      ut.Nombre AS TerminadorNombre, ut.Apellido AS TerminadorApellido, ut.Email AS TerminadorEmail
                               FROM Reservas r
                               INNER JOIN Inquilinos iq ON r.IdInquilino = iq.IdInquilino
                               INNER JOIN Inmuebles im ON r.IdInmueble = im.IdInmueble
                               INNER JOIN Propietarios p ON im.IdPropietario = p.IdPropietario
                               INNER JOIN TiposInmueble t ON im.IdTipoInmueble = t.IdTipoInmueble
                               LEFT JOIN Usuarios uc ON r.IdUsuarioCreador = uc.IdUsuario
                               LEFT JOIN Usuarios ut ON r.IdUsuarioTerminador = ut.IdUsuario
                               WHERE r.IdReservaOrigen = @idReservaOrigen AND r.Activo = 1
                               ORDER BY r.FechaDesde ASC";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idReservaOrigen", idReservaOrigen);
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
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                string sql = @"SELECT COUNT(*) FROM Reservas 
                               WHERE IdInmueble = @idInmueble 
                                 AND Activo = 1 
                                 AND Estado NOT IN ('Anulada', 'Cancelada')
                                 AND (@desde < FechaHasta AND @hasta > FechaDesde)";
                
                if (idReservaExcluir.HasValue)
                {
                    sql += " AND IdReserva != @idExcluir";
                }

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idInmueble", idInmueble);
                    command.Parameters.AddWithValue("@desde", desde.Date);
                    command.Parameters.AddWithValue("@hasta", hasta.Date);
                    if (idReservaExcluir.HasValue)
                    {
                        command.Parameters.AddWithValue("@idExcluir", idReservaExcluir.Value);
                    }

                    connection.Open();
                    long count = Convert.ToInt64(command.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        public ListaPaginada<Reserva> ObtenerPaginado(string? filtro, int pagina, int tamano = 5)
        {
            var resultado = new ListaPaginada<Reserva>
            {
                Pagina = Math.Max(1, pagina),
                Tamano = tamano,
                Filtro = filtro ?? string.Empty
            };

            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                string whereClause = "WHERE r.Activo = 1";
                if (!string.IsNullOrWhiteSpace(filtro))
                {
                    whereClause += @" AND (
                        iq.Nombre LIKE @f OR 
                        iq.Apellido LIKE @f OR 
                        im.Direccion LIKE @f OR 
                        r.Estado LIKE @f
                    )";
                }

                string countSql = $@"SELECT COUNT(*) 
                                    FROM Reservas r
                                    INNER JOIN Inquilinos iq ON r.IdInquilino = iq.IdInquilino
                                    INNER JOIN Inmuebles im ON r.IdInmueble = im.IdInmueble
                                    {whereClause};";

                using (var countCmd = new MySqlCommand(countSql, connection))
                {
                    if (!string.IsNullOrWhiteSpace(filtro))
                    {
                        countCmd.Parameters.AddWithValue("@f", $"%{filtro}%");
                    }
                    resultado.Total = Convert.ToInt32(countCmd.ExecuteScalar());
                }

                int offset = (resultado.Pagina - 1) * resultado.Tamano;
                string sql = $@"SELECT r.IdReserva, r.IdInquilino, r.IdInmueble, r.FechaDesde, r.FechaHasta, 
                                      r.PrecioPorDia, r.MontoTotal, r.FechaTerminacion, r.Multa, r.Estado, r.Activo,
                                      r.IdUsuarioCreador, r.IdUsuarioTerminador, r.IdReservaOrigen,
                                      iq.Nombre AS InquilinoNombre, iq.Apellido AS InquilinoApellido, iq.Dni AS InquilinoDni,
                                      iq.Telefono AS InquilinoTelefono, iq.Email AS InquilinoEmail,
                                      im.Direccion AS InmuebleDireccion, im.PrecioDia AS InmueblePrecioDia,
                                      im.Cupo AS InmuebleCupo, im.Disponible AS InmuebleDisponible,
                                      im.PorcentajeReserva AS InmueblePorcentajeReserva,
                                      p.Nombre AS DuenioNombre, p.Apellido AS DuenioApellido,
                                      t.Descripcion AS TipoDescripcion,
                                      uc.Nombre AS CreadorNombre, uc.Apellido AS CreadorApellido, uc.Email AS CreadorEmail,
                                      ut.Nombre AS TerminadorNombre, ut.Apellido AS TerminadorApellido, ut.Email AS TerminadorEmail
                               FROM Reservas r
                               INNER JOIN Inquilinos iq ON r.IdInquilino = iq.IdInquilino
                               INNER JOIN Inmuebles im ON r.IdInmueble = im.IdInmueble
                               INNER JOIN Propietarios p ON im.IdPropietario = p.IdPropietario
                               INNER JOIN TiposInmueble t ON im.IdTipoInmueble = t.IdTipoInmueble
                               LEFT JOIN Usuarios uc ON r.IdUsuarioCreador = uc.IdUsuario
                               LEFT JOIN Usuarios ut ON r.IdUsuarioTerminador = ut.IdUsuario
                               {whereClause}
                               ORDER BY r.FechaDesde DESC
                               LIMIT @limit OFFSET @offset;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    if (!string.IsNullOrWhiteSpace(filtro))
                    {
                        command.Parameters.AddWithValue("@f", $"%{filtro}%");
                    }
                    command.Parameters.AddWithValue("@limit", resultado.Tamano);
                    command.Parameters.AddWithValue("@offset", offset);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            resultado.Items.Add(MapFromReader(reader));
                        }
                    }
                }
            }

            return resultado;
        }

        
        // ==========================================
        // MÓDULO 5: CONSULTAS DE INFORMES
        // ==========================================
        public IList<Reserva> ObtenerVigentes(DateTime? desde, DateTime? hasta)
        {
            var res = new List<Reserva>();
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                string sql = @"SELECT r.IdReserva, r.IdInquilino, r.IdInmueble, r.FechaDesde, r.FechaHasta, 
                                      r.PrecioPorDia, r.MontoTotal, r.FechaTerminacion, r.Multa, r.Estado, r.Activo,
                                      r.IdUsuarioCreador, r.IdUsuarioTerminador, r.IdReservaOrigen,
                                      iq.Nombre AS InquilinoNombre, iq.Apellido AS InquilinoApellido, iq.Dni AS InquilinoDni,
                                      iq.Telefono AS InquilinoTelefono, iq.Email AS InquilinoEmail,
                                      im.Direccion AS InmuebleDireccion, im.PrecioDia AS InmueblePrecioDia,
                                      im.Cupo AS InmuebleCupo, im.Disponible AS InmuebleDisponible,
                                      im.PorcentajeReserva AS InmueblePorcentajeReserva,
                                      p.Nombre AS DuenioNombre, p.Apellido AS DuenioApellido,
                                      t.Descripcion AS TipoDescripcion,
                                      uc.Nombre AS CreadorNombre, uc.Apellido AS CreadorApellido, uc.Email AS CreadorEmail,
                                      ut.Nombre AS TerminadorNombre, ut.Apellido AS TerminadorApellido, ut.Email AS TerminadorEmail
                               FROM Reservas r
                               INNER JOIN Inquilinos iq ON r.IdInquilino = iq.IdInquilino
                               INNER JOIN Inmuebles im ON r.IdInmueble = im.IdInmueble
                               INNER JOIN Propietarios p ON im.IdPropietario = p.IdPropietario
                               INNER JOIN TiposInmueble t ON im.IdTipoInmueble = t.IdTipoInmueble
                               LEFT JOIN Usuarios uc ON r.IdUsuarioCreador = uc.IdUsuario
                               LEFT JOIN Usuarios ut ON r.IdUsuarioTerminador = ut.IdUsuario
                               WHERE r.Activo = 1 
                                 AND r.Estado = 'Vigente'";

                if (desde.HasValue)
                {
                    sql += " AND r.FechaHasta >= @desde";
                }
                if (hasta.HasValue)
                {
                    sql += " AND r.FechaDesde <= @hasta";
                }

                sql += " ORDER BY r.FechaDesde ASC";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    if (desde.HasValue)
                    {
                        command.Parameters.AddWithValue("@desde", desde.Value.Date);
                    }
                    if (hasta.HasValue)
                    {
                        command.Parameters.AddWithValue("@hasta", hasta.Value.Date);
                    }

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

        public IList<Reserva> ObtenerPorVencer(int dias)
        {
            var res = new List<Reserva>();
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                // Reservas vigentes cuya FechaHasta esté entre hoy y hoy + dias
                string sql = @"SELECT r.IdReserva, r.IdInquilino, r.IdInmueble, r.FechaDesde, r.FechaHasta, 
                                      r.PrecioPorDia, r.MontoTotal, r.FechaTerminacion, r.Multa, r.Estado, r.Activo,
                                      r.IdUsuarioCreador, r.IdUsuarioTerminador, r.IdReservaOrigen,
                                      iq.Nombre AS InquilinoNombre, iq.Apellido AS InquilinoApellido, iq.Dni AS InquilinoDni,
                                      iq.Telefono AS InquilinoTelefono, iq.Email AS InquilinoEmail,
                                      im.Direccion AS InmuebleDireccion, im.PrecioDia AS InmueblePrecioDia,
                                      im.Cupo AS InmuebleCupo, im.Disponible AS InmuebleDisponible,
                                      im.PorcentajeReserva AS InmueblePorcentajeReserva,
                                      p.Nombre AS DuenioNombre, p.Apellido AS DuenioApellido,
                                      t.Descripcion AS TipoDescripcion,
                                      uc.Nombre AS CreadorNombre, uc.Apellido AS CreadorApellido, uc.Email AS CreadorEmail,
                                      ut.Nombre AS TerminadorNombre, ut.Apellido AS TerminadorApellido, ut.Email AS TerminadorEmail
                               FROM Reservas r
                               INNER JOIN Inquilinos iq ON r.IdInquilino = iq.IdInquilino
                               INNER JOIN Inmuebles im ON r.IdInmueble = im.IdInmueble
                               INNER JOIN Propietarios p ON im.IdPropietario = p.IdPropietario
                               INNER JOIN TiposInmueble t ON im.IdTipoInmueble = t.IdTipoInmueble
                               LEFT JOIN Usuarios uc ON r.IdUsuarioCreador = uc.IdUsuario
                               LEFT JOIN Usuarios ut ON r.IdUsuarioTerminador = ut.IdUsuario
                               WHERE r.Activo = 1 
                                 AND r.Estado = 'Vigente'
                                 AND r.FechaHasta >= CURDATE()
                                 AND r.FechaHasta <= DATE_ADD(CURDATE(), INTERVAL @dias DAY)
                               ORDER BY r.FechaHasta ASC";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@dias", Math.Max(1, dias));
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
        private static Reserva MapFromReader(MySqlDataReader reader)
        {
            var reserva = new Reserva
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
                IdUsuarioCreador = reader.IsDBNull(reader.GetOrdinal("IdUsuarioCreador")) ? null : reader.GetInt32("IdUsuarioCreador"),
                IdUsuarioTerminador = reader.IsDBNull(reader.GetOrdinal("IdUsuarioTerminador")) ? null : reader.GetInt32("IdUsuarioTerminador"),
                IdReservaOrigen = reader.IsDBNull(reader.GetOrdinal("IdReservaOrigen")) ? null : reader.GetInt32("IdReservaOrigen"),
                Inquilino = new Inquilino
                {
                    IdInquilino = reader.GetInt32("IdInquilino"),
                    Nombre = reader.GetString("InquilinoNombre"),
                    Apellido = reader.GetString("InquilinoApellido"),
                    Dni = reader.GetString("InquilinoDni"),
                    Telefono = reader.IsDBNull(reader.GetOrdinal("InquilinoTelefono")) ? "" : reader.GetString("InquilinoTelefono"),
                    Email = reader.GetString("InquilinoEmail")
                },
                Inmueble = new Inmueble
                {
                    IdInmueble = reader.GetInt32("IdInmueble"),
                    Direccion = reader.GetString("InmuebleDireccion"),
                    PrecioDia = reader.GetDecimal("InmueblePrecioDia"),
                    Cupo = reader.GetInt32("InmuebleCupo"),
                    Disponible = reader.GetBoolean("InmuebleDisponible"),
                    PorcentajeReserva = reader.IsDBNull(reader.GetOrdinal("InmueblePorcentajeReserva")) ? 10m : reader.GetDecimal("InmueblePorcentajeReserva"),
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

            if (reserva.IdUsuarioCreador.HasValue && !reader.IsDBNull(reader.GetOrdinal("CreadorNombre")))
            {
                reserva.UsuarioCreador = new Usuario
                {
                    IdUsuario = reserva.IdUsuarioCreador.Value,
                    Nombre = reader.GetString("CreadorNombre"),
                    Apellido = reader.GetString("CreadorApellido"),
                    Email = reader.GetString("CreadorEmail")
                };
            }

            if (reserva.IdUsuarioTerminador.HasValue && !reader.IsDBNull(reader.GetOrdinal("TerminadorNombre")))
            {
                reserva.UsuarioTerminador = new Usuario
                {
                    IdUsuario = reserva.IdUsuarioTerminador.Value,
                    Nombre = reader.GetString("TerminadorNombre"),
                    Apellido = reader.GetString("TerminadorApellido"),
                    Email = reader.GetString("TerminadorEmail")
                };
            }

            return reserva;
        }
    }
}

