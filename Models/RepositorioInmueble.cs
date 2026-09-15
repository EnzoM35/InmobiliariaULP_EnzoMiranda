using MySqlConnector;

namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models
{
    public class RepositorioInmueble : RepositorioBase, IRepositorioInmueble
    {
        public RepositorioInmueble(IConfiguration configuration) : base(configuration) { }

        public int Alta(Inmueble inmueble)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
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
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
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
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
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
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
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

            if (inmueble != null)
            {
                inmueble.Imagenes = ObtenerImagenesPorInmueble(inmueble.IdInmueble);
            }

            return inmueble;
        }

        public IList<Inmueble> ObtenerTodos()
        {
            var res = new List<Inmueble>();
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                string sql = @"SELECT i.IdInmueble, i.Direccion, i.Cupo, i.Latitud, i.Longitud, i.PrecioDia, 
                                      i.PorcentajeReserva, i.Disponible, i.Portada, i.IdTipoInmueble, i.IdPropietario, i.Activo,
                                      t.Descripcion AS TipoDescripcion,
                                      p.Nombre AS PropietarioNombre, p.Apellido AS PropietarioApellido, p.Dni AS PropietarioDni,
                                      p.Telefono AS PropietarioTelefono, p.Email AS PropietarioEmail
                               FROM Inmuebles i
                               INNER JOIN TiposInmueble t ON i.IdTipoInmueble = t.IdTipoInmueble
                               INNER JOIN Propietarios p ON i.IdPropietario = p.IdPropietario
                               WHERE i.Activo = 1
                               ORDER BY i.Direccion";
                
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
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                string sql = @"SELECT i.IdInmueble, i.Direccion, i.Cupo, i.Latitud, i.Longitud, i.PrecioDia, 
                                      i.PorcentajeReserva, i.Disponible, i.Portada, i.IdTipoInmueble, i.IdPropietario, i.Activo,
                                      t.Descripcion AS TipoDescripcion,
                                      p.Nombre AS PropietarioNombre, p.Apellido AS PropietarioApellido, p.Dni AS PropietarioDni,
                                      p.Telefono AS PropietarioTelefono, p.Email AS PropietarioEmail
                               FROM Inmuebles i
                               INNER JOIN TiposInmueble t ON i.IdTipoInmueble = t.IdTipoInmueble
                               INNER JOIN Propietarios p ON i.IdPropietario = p.IdPropietario
                               WHERE i.Activo = 1 AND i.Disponible = 1
                               ORDER BY i.Direccion";
                
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
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                string sql = @"SELECT i.IdInmueble, i.Direccion, i.Cupo, i.Latitud, i.Longitud, i.PrecioDia, 
                                      i.PorcentajeReserva, i.Disponible, i.Portada, i.IdTipoInmueble, i.IdPropietario, i.Activo,
                                      t.Descripcion AS TipoDescripcion,
                                      p.Nombre AS PropietarioNombre, p.Apellido AS PropietarioApellido, p.Dni AS PropietarioDni,
                                      p.Telefono AS PropietarioTelefono, p.Email AS PropietarioEmail
                               FROM Inmuebles i
                               INNER JOIN TiposInmueble t ON i.IdTipoInmueble = t.IdTipoInmueble
                               INNER JOIN Propietarios p ON i.IdPropietario = p.IdPropietario
                               WHERE i.IdPropietario = @idPropietario AND i.Activo = 1
                               ORDER BY i.Direccion";
                
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

        public ListaPaginada<Inmueble> ObtenerPaginado(string? filtro, int pagina, int tamano = 5)
        {
            var resultado = new ListaPaginada<Inmueble>
            {
                Pagina = Math.Max(1, pagina),
                Tamano = tamano,
                Filtro = filtro ?? string.Empty
            };

            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                string whereClause = "WHERE i.Activo = 1";
                if (!string.IsNullOrWhiteSpace(filtro))
                {
                    whereClause += @" AND (
                        i.Direccion LIKE @f OR 
                        t.Descripcion LIKE @f OR 
                        p.Nombre LIKE @f OR 
                        p.Apellido LIKE @f
                    )";
                }

                string countSql = $@"SELECT COUNT(*) 
                                    FROM Inmuebles i
                                    INNER JOIN TiposInmueble t ON i.IdTipoInmueble = t.IdTipoInmueble
                                    INNER JOIN Propietarios p ON i.IdPropietario = p.IdPropietario
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
                string sql = $@"SELECT i.IdInmueble, i.Direccion, i.Cupo, i.Latitud, i.Longitud, i.PrecioDia, 
                                      i.PorcentajeReserva, i.Disponible, i.Portada, i.IdTipoInmueble, i.IdPropietario, i.Activo,
                                      t.Descripcion AS TipoDescripcion,
                                      p.Nombre AS PropietarioNombre, p.Apellido AS PropietarioApellido, p.Dni AS PropietarioDni,
                                      p.Telefono AS PropietarioTelefono, p.Email AS PropietarioEmail
                               FROM Inmuebles i
                               INNER JOIN TiposInmueble t ON i.IdTipoInmueble = t.IdTipoInmueble
                               INNER JOIN Propietarios p ON i.IdPropietario = p.IdPropietario
                               {whereClause}
                               ORDER BY i.IdInmueble DESC
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
        // MÓDULO 4: GESTIÓN DE GALERÍA DE IMÁGENES
        // ==========================================
        public int AltaImagen(ImagenInmueble imagen)
        {
            int res = 0;
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                // Si se marca como portada, desmarcar anteriores
                if (imagen.EsPortada)
                {
                    using (var desmarcarCmd = new MySqlCommand("UPDATE ImagenesInmueble SET EsPortada = 0 WHERE IdInmueble = @idInmueble", connection))
                    {
                        desmarcarCmd.Parameters.AddWithValue("@idInmueble", imagen.IdInmueble);
                        desmarcarCmd.ExecuteNonQuery();
                    }

                    using (var actPortada = new MySqlCommand("UPDATE Inmuebles SET Portada = @url WHERE IdInmueble = @idInmueble", connection))
                    {
                        actPortada.Parameters.AddWithValue("@url", imagen.Url);
                        actPortada.Parameters.AddWithValue("@idInmueble", imagen.IdInmueble);
                        actPortada.ExecuteNonQuery();
                    }
                }

                string sql = @"INSERT INTO ImagenesInmueble (IdInmueble, Url, EsPortada) 
                               VALUES (@idInmueble, @url, @esPortada);
                               SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idInmueble", imagen.IdInmueble);
                    command.Parameters.AddWithValue("@url", imagen.Url);
                    command.Parameters.AddWithValue("@esPortada", imagen.EsPortada ? 1 : 0);

                    res = Convert.ToInt32(command.ExecuteScalar());
                    imagen.IdImagen = res;
                }
            }
            return res;
        }

        public int EliminarImagen(int idImagen)
        {
            int res = 0;
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                var img = ObtenerImagenPorId(idImagen);
                if (img == null) return 0;

                string sql = "DELETE FROM ImagenesInmueble WHERE IdImagen = @idImagen";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idImagen", idImagen);
                    res = command.ExecuteNonQuery();
                }

                // Si era portada, asignar otra imagen como portada o limpiar
                if (img.EsPortada)
                {
                    string sqlSiguiente = "SELECT IdImagen, Url FROM ImagenesInmueble WHERE IdInmueble = @idInmueble LIMIT 1";
                    using (var cmdSig = new MySqlCommand(sqlSiguiente, connection))
                    {
                        cmdSig.Parameters.AddWithValue("@idInmueble", img.IdInmueble);
                        using (var reader = cmdSig.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int sigId = reader.GetInt32("IdImagen");
                                string sigUrl = reader.GetString("Url");
                                reader.Close();

                                using (var setPort = new MySqlCommand("UPDATE ImagenesInmueble SET EsPortada = 1 WHERE IdImagen = @sigId; UPDATE Inmuebles SET Portada = @sigUrl WHERE IdInmueble = @idInmueble;", connection))
                                {
                                    setPort.Parameters.AddWithValue("@sigId", sigId);
                                    setPort.Parameters.AddWithValue("@sigUrl", sigUrl);
                                    setPort.Parameters.AddWithValue("@idInmueble", img.IdInmueble);
                                    setPort.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                reader.Close();
                                using (var limpiarPort = new MySqlCommand("UPDATE Inmuebles SET Portada = NULL WHERE IdInmueble = @idInmueble;", connection))
                                {
                                    limpiarPort.Parameters.AddWithValue("@idInmueble", img.IdInmueble);
                                    limpiarPort.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
            }
            return res;
        }

        public IList<ImagenInmueble> ObtenerImagenesPorInmueble(int idInmueble)
        {
            var list = new List<ImagenInmueble>();
            using (var connection = new MySqlConnection(ConnectionString))
            {
                string sql = @"SELECT IdImagen, IdInmueble, Url, EsPortada 
                               FROM ImagenesInmueble 
                               WHERE IdInmueble = @idInmueble 
                               ORDER BY EsPortada DESC, IdImagen ASC";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idInmueble", idInmueble);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new ImagenInmueble
                            {
                                IdImagen = reader.GetInt32("IdImagen"),
                                IdInmueble = reader.GetInt32("IdInmueble"),
                                Url = reader.GetString("Url"),
                                EsPortada = reader.GetBoolean("EsPortada")
                            });
                        }
                    }
                }
            }
            return list;
        }

        public ImagenInmueble? ObtenerImagenPorId(int idImagen)
        {
            ImagenInmueble? img = null;
            using (var connection = new MySqlConnection(ConnectionString))
            {
                string sql = "SELECT IdImagen, IdInmueble, Url, EsPortada FROM ImagenesInmueble WHERE IdImagen = @idImagen";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idImagen", idImagen);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            img = new ImagenInmueble
                            {
                                IdImagen = reader.GetInt32("IdImagen"),
                                IdInmueble = reader.GetInt32("IdInmueble"),
                                Url = reader.GetString("Url"),
                                EsPortada = reader.GetBoolean("EsPortada")
                            };
                        }
                    }
                }
            }
            return img;
        }

        public int EstablecerPortada(int idInmueble, int idImagen)
        {
            int res = 0;
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                string sql = @"UPDATE ImagenesInmueble 
                               SET EsPortada = CASE WHEN IdImagen = @idImagen THEN 1 ELSE 0 END 
                               WHERE IdInmueble = @idInmueble;

                               UPDATE Inmuebles 
                               SET Portada = (SELECT Url FROM ImagenesInmueble WHERE IdImagen = @idImagen) 
                               WHERE IdInmueble = @idInmueble;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idImagen", idImagen);
                    command.Parameters.AddWithValue("@idInmueble", idInmueble);
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        // ==========================================
        // MÓDULO 5: CONSULTAS DE INFORMES
        // ==========================================
        public IList<Inmueble> ObtenerPorDisponibilidad(bool? disponible)
        {
            var res = new List<Inmueble>();
            using (var connection = new MySqlConnection(ConnectionString))
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

                if (disponible.HasValue)
                {
                    sql += " AND i.Disponible = @disponible";
                }
                sql += " ORDER BY i.Direccion";

                using (var command = new MySqlCommand(sql, connection))
                {
                    if (disponible.HasValue)
                    {
                        command.Parameters.AddWithValue("@disponible", disponible.Value ? 1 : 0);
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

        public IList<Inmueble> ObtenerSinReservas(int dias)
        {
            var res = new List<Inmueble>();
            using (var connection = new MySqlConnection(ConnectionString))
            {
                // Inmuebles activos sin ninguna reserva en los últimos X días
                string sql = @"SELECT i.IdInmueble, i.Direccion, i.Cupo, i.Latitud, i.Longitud, i.PrecioDia, 
                                      i.PorcentajeReserva, i.Disponible, i.Portada, i.IdTipoInmueble, i.IdPropietario, i.Activo,
                                      t.Descripcion AS TipoDescripcion,
                                      p.Nombre AS PropietarioNombre, p.Apellido AS PropietarioApellido, p.Dni AS PropietarioDni,
                                      p.Telefono AS PropietarioTelefono, p.Email AS PropietarioEmail
                               FROM Inmuebles i
                               INNER JOIN TiposInmueble t ON i.IdTipoInmueble = t.IdTipoInmueble
                               INNER JOIN Propietarios p ON i.IdPropietario = p.IdPropietario
                               WHERE i.Activo = 1
                                 AND i.IdInmueble NOT IN (
                                     SELECT DISTINCT r.IdInmueble 
                                     FROM Reservas r 
                                     WHERE r.Activo = 1 
                                       AND r.Estado NOT IN ('Anulada', 'Cancelada')
                                       AND r.FechaDesde >= DATE_SUB(CURDATE(), INTERVAL @dias DAY)
                                 )
                               ORDER BY i.Direccion;";

                using (var command = new MySqlCommand(sql, connection))
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

        public IList<Inmueble> ObtenerLibresEntreFechas(DateTime desde, DateTime hasta)
        {
            var res = new List<Inmueble>();
            using (var connection = new MySqlConnection(ConnectionString))
            {
                string sql = @"SELECT i.IdInmueble, i.Direccion, i.Cupo, i.Latitud, i.Longitud, i.PrecioDia, 
                                      i.PorcentajeReserva, i.Disponible, i.Portada, i.IdTipoInmueble, i.IdPropietario, i.Activo,
                                      t.Descripcion AS TipoDescripcion,
                                      p.Nombre AS PropietarioNombre, p.Apellido AS PropietarioApellido, p.Dni AS PropietarioDni,
                                      p.Telefono AS PropietarioTelefono, p.Email AS PropietarioEmail
                               FROM Inmuebles i
                               INNER JOIN TiposInmueble t ON i.IdTipoInmueble = t.IdTipoInmueble
                               INNER JOIN Propietarios p ON i.IdPropietario = p.IdPropietario
                               WHERE i.Activo = 1 
                                 AND i.Disponible = 1
                                 AND i.IdInmueble NOT IN (
                                     SELECT DISTINCT r.IdInmueble 
                                     FROM Reservas r 
                                     WHERE r.Activo = 1 
                                       AND r.Estado NOT IN ('Anulada', 'Cancelada')
                                       AND (@desde < r.FechaHasta AND @hasta > r.FechaDesde)
                                 )
                               ORDER BY i.PrecioDia ASC, i.Direccion ASC;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@desde", desde.Date);
                    command.Parameters.AddWithValue("@hasta", hasta.Date);
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

        public IList<InmuebleRankingDTO> ObtenerMasReservados(int dias = 365)
        {
            var res = new List<InmuebleRankingDTO>();
            using (var connection = new MySqlConnection(ConnectionString))
            {
                string sql = @"SELECT i.IdInmueble, i.Direccion, i.Portada, i.PrecioDia,
                                      t.Descripcion AS TipoDescripcion,
                                      CONCAT(p.Nombre, ' ', p.Apellido) AS DuenioCompleto,
                                      COUNT(r.IdReserva) AS CantidadReservas,
                                      COALESCE(SUM(DATEDIFF(r.FechaHasta, r.FechaDesde)), 0) AS TotalDiasReservados,
                                      COALESCE(SUM(r.MontoTotal), 0) AS TotalRecaudado
                               FROM Inmuebles i
                               INNER JOIN TiposInmueble t ON i.IdTipoInmueble = t.IdTipoInmueble
                               INNER JOIN Propietarios p ON i.IdPropietario = p.IdPropietario
                               INNER JOIN Reservas r ON i.IdInmueble = r.IdInmueble AND r.Activo = 1 AND r.Estado NOT IN ('Anulada', 'Cancelada')
                               WHERE i.Activo = 1
                                 AND r.FechaDesde >= DATE_SUB(CURDATE(), INTERVAL @dias DAY)
                               GROUP BY i.IdInmueble, i.Direccion, i.Portada, i.PrecioDia, t.Descripcion, p.Nombre, p.Apellido
                               ORDER BY CantidadReservas DESC, TotalDiasReservados DESC;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@dias", Math.Max(1, dias));
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            res.Add(new InmuebleRankingDTO
                            {
                                IdInmueble = reader.GetInt32("IdInmueble"),
                                Direccion = reader.GetString("Direccion"),
                                TipoDescripcion = reader.GetString("TipoDescripcion"),
                                DuenioCompleto = reader.GetString("DuenioCompleto"),
                                Portada = reader.IsDBNull(reader.GetOrdinal("Portada")) ? null : reader.GetString("Portada"),
                                PrecioDia = reader.GetDecimal("PrecioDia"),
                                CantidadReservas = reader.GetInt32("CantidadReservas"),
                                TotalDiasReservados = Convert.ToInt32(reader.GetInt64("TotalDiasReservados")),
                                TotalRecaudado = reader.GetDecimal("TotalRecaudado")
                            });
                        }
                    }
                }
            }
            return res;
        }

        private static Inmueble MapFromReader(MySqlDataReader reader)
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
