using System.Data;
using MySql.Data.MySqlClient;
using InmobiliariaAlcaraz.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System;

namespace InmobiliariaAlcaraz.Models
{
    public class RepositorioContrato : RepositorioBase, IRepositorioContrato
    {
        public RepositorioContrato(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Contrato contrato)
        {
            int pagosEsperados = CalcularPagosEsperados(contrato.FechaInicio, contrato.FechaFin);
            if (pagosEsperados < 1)
            {
                throw new InvalidOperationException("El contrato debe tener una duración mínima de un mes (un pago esperado).");
            }

            if (TieneContratosSolapados(contrato.IdInmueble, contrato.FechaInicio, contrato.FechaFin))
            {
                throw new InvalidOperationException("No se puede crear el contrato: ya existe un contrato activo para el inmueble en las fechas especificadas.");
            }

            using (var connection = new MySqlConnection(connectionString))
            {
                var query = @"INSERT INTO contratos 
                            (IdInquilino, IdInmueble, MontoMensual, FechaInicio, FechaFin, 
                            IdUsuarioCreacion, FechaCreacion, Estado, PagosEsperados)
                            VALUES 
                            (@IdInquilino, @IdInmueble, @MontoMensual, @FechaInicio, @FechaFin, 
                            @IdUsuarioCreacion, @FechaCreacion, @Estado, @PagosEsperados);
                            SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdInquilino", contrato.IdInquilino);
                    command.Parameters.AddWithValue("@IdInmueble", contrato.IdInmueble);
                    command.Parameters.AddWithValue("@MontoMensual", contrato.MontoMensual);
                    command.Parameters.AddWithValue("@FechaInicio", contrato.FechaInicio);
                    command.Parameters.AddWithValue("@FechaFin", contrato.FechaFin);
                    command.Parameters.AddWithValue("@IdUsuarioCreacion", contrato.IdUsuarioCreacion);
                    command.Parameters.AddWithValue("@FechaCreacion", contrato.FechaCreacion);
                    command.Parameters.AddWithValue("@Estado", contrato.Estado);
                    command.Parameters.AddWithValue("@PagosEsperados", pagosEsperados);

                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public int Bajan(int id, int IdUsuarioFinalizacion)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var query = @"UPDATE contratos SET 
                            Estado = 0,
                            FechaFinAnticipado = CURRENT_DATE(),
                            IdUsuarioFinalizacion = @IdUsuarioFinalizacion
                            WHERE IdContrato = @Id";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@IdUsuarioFinalizacion", IdUsuarioFinalizacion);

                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public int Baja(int id)
        {
            return 1; 
        }

        public int Modificacion(Contrato contrato)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var query = @"UPDATE contratos SET 
                            IdInquilino = @IdInquilino, 
                            IdInmueble = @IdInmueble, 
                            MontoMensual = @MontoMensual, 
                            FechaInicio = @FechaInicio, 
                            FechaFin = @FechaFin,
                            Estado = @Estado,
                            PagosEsperados = @PagosEsperados,
                            FechaFinAnticipado = @FechaFinAnticipado,
                            IdUsuarioFinalizacion = @IdUsuarioFinalizacion
                            WHERE IdContrato = @IdContrato";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdContrato", contrato.IdContrato);
                    command.Parameters.AddWithValue("@IdInquilino", contrato.IdInquilino);
                    command.Parameters.AddWithValue("@IdInmueble", contrato.IdInmueble);
                    command.Parameters.AddWithValue("@MontoMensual", contrato.MontoMensual);
                    command.Parameters.AddWithValue("@FechaInicio", contrato.FechaInicio);
                    command.Parameters.AddWithValue("@FechaFin", contrato.FechaFin);
                    command.Parameters.AddWithValue("@Estado", contrato.Estado);
                    command.Parameters.AddWithValue("@PagosEsperados", contrato.PagosEsperados);
                    command.Parameters.AddWithValue("@FechaFinAnticipado", contrato.FechaFinAnticipado.HasValue ? contrato.FechaFinAnticipado : DBNull.Value);
                    command.Parameters.AddWithValue("@IdUsuarioFinalizacion", contrato.IdUsuarioFinalizacion.HasValue ? contrato.IdUsuarioFinalizacion : DBNull.Value);

                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public IList<Contrato> ObtenerTodos()
        {
            var contratos = new List<Contrato>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT c.*, 
                           i.Nombre AS InquilinoNombre, i.Apellido AS InquilinoApellido,
                           inm.Direccion, 
                           uc.Nombre AS UsuarioCreacionNombre, uc.Apellido AS UsuarioCreacionApellido,
                           uf.Nombre AS UsuarioFinalizacionNombre, uf.Apellido AS UsuarioFinalizacionApellido
                           FROM contratos c
                           INNER JOIN inquilinos i ON c.IdInquilino = i.IdInquilino
                           INNER JOIN inmuebles inm ON c.IdInmueble = inm.IdInmueble
                           INNER JOIN usuarios uc ON c.IdUsuarioCreacion = uc.IdUsuario
                           LEFT JOIN usuarios uf ON c.IdUsuarioFinalizacion = uf.IdUsuario";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            contratos.Add(new Contrato
                            {
                                IdContrato = reader.GetInt32("IdContrato"),
                                IdInquilino = reader.GetInt32("IdInquilino"),
                                IdInmueble = reader.GetInt32("IdInmueble"),
                                MontoMensual = reader.GetDecimal("MontoMensual"),
                                FechaInicio = reader.GetDateTime("FechaInicio"),
                                FechaFin = reader.GetDateTime("FechaFin"),
                                IdUsuarioCreacion = reader.GetInt32("IdUsuarioCreacion"),
                                FechaCreacion = reader.GetDateTime("FechaCreacion"),
                                FechaFinAnticipado = reader.IsDBNull("FechaFinAnticipado") ? 
                                    null : reader.GetDateTime("FechaFinAnticipado"),
                                IdUsuarioFinalizacion = reader.IsDBNull("IdUsuarioFinalizacion") ? 
                                    null : reader.GetInt32("IdUsuarioFinalizacion"),
                                Estado = reader.GetInt32("Estado"),
                                PagosEsperados = reader.GetInt32("PagosEsperados"),
                                Inquilino = new Inquilino
                                {
                                    IdInquilino = reader.GetInt32("IdInquilino"),
                                    Nombre = reader.GetString("InquilinoNombre"),
                                    Apellido = reader.GetString("InquilinoApellido")
                                },
                                Inmueble = new Inmueble
                                {
                                    IdInmueble = reader.GetInt32("IdInmueble"),
                                    Direccion = reader.GetString("Direccion")
                                },
                                UsuarioCreacion = new Usuario
                                {
                                    IdUsuario = reader.GetInt32("IdUsuarioCreacion"),
                                    Nombre = reader.GetString("UsuarioCreacionNombre"),
                                    Apellido = reader.GetString("UsuarioCreacionApellido")
                                },
                                UsuarioFinalizacion = reader.IsDBNull("IdUsuarioFinalizacion") ? 
                                    null : new Usuario
                                    {
                                        IdUsuario = reader.GetInt32("IdUsuarioFinalizacion"),
                                        Nombre = reader.GetString("UsuarioFinalizacionNombre"),
                                        Apellido = reader.GetString("UsuarioFinalizacionApellido")
                                    }
                            });
                        }
                    }
                }
            }

            return contratos;
        }

        public Contrato ObtenerPorId(int id)
        {
            Contrato contrato = null;

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT c.*, 
                           i.Nombre AS InquilinoNombre, i.Apellido AS InquilinoApellido,
                           inm.Direccion, 
                           uc.Nombre AS UsuarioCreacionNombre, uc.Apellido AS UsuarioCreacionApellido,
                           uf.Nombre AS UsuarioFinalizacionNombre, uf.Apellido AS UsuarioFinalizacionApellido
                           FROM contratos c
                           INNER JOIN inquilinos i ON c.IdInquilino = i.IdInquilino
                           INNER JOIN inmuebles inm ON c.IdInmueble = inm.IdInmueble
                           INNER JOIN usuarios uc ON c.IdUsuarioCreacion = uc.IdUsuario
                           LEFT JOIN usuarios uf ON c.IdUsuarioFinalizacion = uf.IdUsuario
                           WHERE c.IdContrato = @Id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            contrato = new Contrato
                            {
                                IdContrato = reader.GetInt32("IdContrato"),
                                IdInquilino = reader.GetInt32("IdInquilino"),
                                IdInmueble = reader.GetInt32("IdInmueble"),
                                MontoMensual = reader.GetDecimal("MontoMensual"),
                                FechaInicio = reader.GetDateTime("FechaInicio"),
                                FechaFin = reader.GetDateTime("FechaFin"),
                                IdUsuarioCreacion = reader.GetInt32("IdUsuarioCreacion"),
                                FechaCreacion = reader.GetDateTime("FechaCreacion"),
                                PagosEsperados = reader.GetInt32("PagosEsperados"),
                                FechaFinAnticipado = reader.IsDBNull("FechaFinAnticipado") ? 
                                    null : reader.GetDateTime("FechaFinAnticipado"),
                                IdUsuarioFinalizacion = reader.IsDBNull("IdUsuarioFinalizacion") ? 
                                    null : reader.GetInt32("IdUsuarioFinalizacion"),
                                Estado = reader.GetInt32("Estado"),
                                Inquilino = new Inquilino
                                {
                                    IdInquilino = reader.GetInt32("IdInquilino"),
                                    Nombre = reader.GetString("InquilinoNombre"),
                                    Apellido = reader.GetString("InquilinoApellido")
                                },
                                Inmueble = new Inmueble
                                {
                                    IdInmueble = reader.GetInt32("IdInmueble"),
                                    Direccion = reader.GetString("Direccion")
                                },
                                UsuarioCreacion = new Usuario
                                {
                                    IdUsuario = reader.GetInt32("IdUsuarioCreacion"),
                                    Nombre = reader.GetString("UsuarioCreacionNombre"),
                                    Apellido = reader.GetString("UsuarioCreacionApellido")
                                },
                                UsuarioFinalizacion = reader.IsDBNull("IdUsuarioFinalizacion") ? 
                                    null : new Usuario
                                    {
                                        IdUsuario = reader.GetInt32("IdUsuarioFinalizacion"),
                                        Nombre = reader.GetString("UsuarioFinalizacionNombre"),
                                        Apellido = reader.GetString("UsuarioFinalizacionApellido")
                                    }
                            };
                        }
                    }
                }
            }

            return contrato;
        }

        public IList<Contrato> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            IList<Contrato> res = new List<Contrato>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"
                    SELECT c.*, 
                        i.Nombre AS InquilinoNombre, i.Apellido AS InquilinoApellido,
                        inm.Direccion, 
                        uc.Nombre AS UsuarioCreacionNombre, uc.Apellido AS UsuarioCreacionApellido
                    FROM contratos c
                    INNER JOIN inquilinos i ON c.IdInquilino = i.IdInquilino
                    INNER JOIN inmuebles inm ON c.IdInmueble = inm.IdInmueble
                    INNER JOIN usuarios uc ON c.IdUsuarioCreacion = uc.IdUsuario
                    ORDER BY c.FechaInicio DESC
                    LIMIT {tamPagina} OFFSET {(paginaNro - 1) * tamPagina}";
                
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Contrato c = new Contrato
                        {
                            IdContrato = reader.GetInt32("IdContrato"),
                            IdInquilino = reader.GetInt32("IdInquilino"),
                            IdInmueble = reader.GetInt32("IdInmueble"),
                            MontoMensual = reader.GetDecimal("MontoMensual"),
                            FechaInicio = reader.GetDateTime("FechaInicio"),
                            FechaFin = reader.GetDateTime("FechaFin"),
                            PagosEsperados = reader.GetInt32("PagosEsperados"),
                            IdUsuarioCreacion = reader.GetInt32("IdUsuarioCreacion"),
                            FechaCreacion = reader.GetDateTime("FechaCreacion"),
                            FechaFinAnticipado = reader.IsDBNull("FechaFinAnticipado") ? 
                                null : reader.GetDateTime("FechaFinAnticipado"),
                            Estado = reader.GetInt32("Estado"),
                            Inquilino = new Inquilino
                            {
                                IdInquilino = reader.GetInt32("IdInquilino"),
                                Nombre = reader.GetString("InquilinoNombre"),
                                Apellido = reader.GetString("InquilinoApellido")
                            },
                            Inmueble = new Inmueble
                            {
                                IdInmueble = reader.GetInt32("IdInmueble"),
                                Direccion = reader.GetString("Direccion")
                            },
                            UsuarioCreacion = new Usuario
                            {
                                IdUsuario = reader.GetInt32("IdUsuarioCreacion"),
                                Nombre = reader.GetString("UsuarioCreacionNombre"),
                                Apellido = reader.GetString("UsuarioCreacionApellido")
                            }
                        };
                        res.Add(c);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public int ObtenerCantidad()
        {
            int cantidad = 0;
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT COUNT(*) FROM contratos";
                using (var command = new MySqlCommand(query, (MySqlConnection)connection))
                {
                    cantidad = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return cantidad;
        }

      public int CalcularPagosEsperados(DateTime fechaInicio, DateTime fechaFin)
        {
            if (fechaFin < fechaInicio)
            {
                return 0;
            }

            int months = ((fechaFin.Year - fechaInicio.Year) * 12) + fechaFin.Month - fechaInicio.Month;
            if (fechaFin.Day >= fechaInicio.Day)
            {
                months++; 
            }

            return Math.Max(1, months);
        }
        public IList<Contrato> ObtenerPorInmueble(int idInmueble, int paginaNro = 1, int tamPagina = 5)
        {
            IList<Contrato> res = new List<Contrato>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @$"
                    SELECT c.*, 
                        i.Nombre AS InquilinoNombre, i.Apellido AS InquilinoApellido,
                        inm.Direccion, 
                        uc.Nombre AS UsuarioCreacionNombre, uc.Apellido AS UsuarioCreacionApellido
                    FROM contratos c
                    INNER JOIN inquilinos i ON c.IdInquilino = i.IdInquilino
                    INNER JOIN inmuebles inm ON c.IdInmueble = inm.IdInmueble
                    INNER JOIN usuarios uc ON c.IdUsuarioCreacion = uc.IdUsuario
                    WHERE c.IdInmueble = @idInmueble
                    ORDER BY c.FechaInicio DESC
                    LIMIT {tamPagina} OFFSET {(paginaNro - 1) * tamPagina}";
                
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idInmueble", idInmueble);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Contrato c = new Contrato
                            {
                                IdContrato = reader.GetInt32("IdContrato"),
                                IdInquilino = reader.GetInt32("IdInquilino"),
                                IdInmueble = reader.GetInt32("IdInmueble"),
                                MontoMensual = reader.GetDecimal("MontoMensual"),
                                FechaInicio = reader.GetDateTime("FechaInicio"),
                                FechaFin = reader.GetDateTime("FechaFin"),
                                Estado = reader.GetInt32("Estado"),
                                PagosEsperados = reader.GetInt32("PagosEsperados"),
                                Inquilino = new Inquilino
                                {
                                    IdInquilino = reader.GetInt32("IdInquilino"),
                                    Nombre = reader.GetString("InquilinoNombre"),
                                    Apellido = reader.GetString("InquilinoApellido")
                                }
                            };
                            res.Add(c);
                        }
                    }
                }
            }
            return res;
        }

        public int ObtenerCantidadPorInmueble(int idInmueble)
        {
            int cantidad = 0;
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT COUNT(*) FROM contratos WHERE IdInmueble = @idInmueble";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idInmueble", idInmueble);
                    cantidad = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return cantidad;
        }

        public IList<Contrato> ObtenerContratosVigentes(DateTime fechaDesde, DateTime fechaHasta, int pagina = 1, int tamPagina = 10)
        {
            IList<Contrato> res = new List<Contrato>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @$"
                    SELECT c.*, 
                        i.Nombre AS InquilinoNombre, i.Apellido AS InquilinoApellido,
                        inm.Direccion, 
                        uc.Nombre AS UsuarioCreacionNombre, uc.Apellido AS UsuarioCreacionApellido
                    FROM contratos c
                    INNER JOIN inquilinos i ON c.IdInquilino = i.IdInquilino
                    INNER JOIN inmuebles inm ON c.IdInmueble = inm.IdInmueble
                    INNER JOIN usuarios uc ON c.IdUsuarioCreacion = uc.IdUsuario
                    WHERE c.Estado = 1 
                    AND c.FechaInicio <= @fechaHasta 
                    AND IFNULL(c.FechaFinAnticipado, c.FechaFin) >= @fechaDesde
                    ORDER BY c.FechaInicio DESC
                    LIMIT {tamPagina} OFFSET {(pagina - 1) * tamPagina}";
                
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@fechaDesde", fechaDesde);
                    command.Parameters.AddWithValue("@fechaHasta", fechaHasta);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Contrato c = new Contrato
                            {
                                IdContrato = reader.GetInt32("IdContrato"),
                                IdInquilino = reader.GetInt32("IdInquilino"),
                                IdInmueble = reader.GetInt32("IdInmueble"),
                                MontoMensual = reader.GetDecimal("MontoMensual"),
                                FechaInicio = reader.GetDateTime("FechaInicio"),
                                FechaFin = reader.GetDateTime("FechaFin"),
                                PagosEsperados = reader.GetInt32("PagosEsperados"),
                                Estado = reader.GetInt32("Estado"),
                                Inquilino = new Inquilino
                                {
                                    IdInquilino = reader.GetInt32("IdInquilino"),
                                    Nombre = reader.GetString("InquilinoNombre"),
                                    Apellido = reader.GetString("InquilinoApellido")
                                },
                                Inmueble = new Inmueble
                                {
                                    IdInmueble = reader.GetInt32("IdInmueble"),
                                    Direccion = reader.GetString("Direccion")
                                }
                            };
                            res.Add(c);
                        }
                    }
                }
            }
            return res;
        }

        public int ObtenerCantidadContratosVigentes(DateTime fechaDesde, DateTime fechaHasta)
        {
            int cantidad = 0;
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT COUNT(*) FROM contratos WHERE Estado = 1 AND FechaInicio <= @fechaHasta AND IFNULL(FechaFinAnticipado, FechaFin) >= @fechaDesde";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@fechaDesde", fechaDesde);
                    command.Parameters.AddWithValue("@fechaHasta", fechaHasta);
                    cantidad = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return cantidad;
        }

        public bool TieneContratosSolapados(int idInmueble, DateTime fechaInicio, DateTime fechaFin, int? idContratoActual = null)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var query = @"
                    SELECT COUNT(*) 
                    FROM contratos 
                    WHERE IdInmueble = @IdInmueble 
                    AND Estado = 1 
                    AND FechaInicio <= @FechaFin 
                    AND IFNULL(FechaFinAnticipado, FechaFin) >= @FechaInicio";

                if (idContratoActual.HasValue)
                {
                    query += " AND IdContrato != @IdContratoActual";
                }

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdInmueble", idInmueble);
                    command.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                    command.Parameters.AddWithValue("@FechaFin", fechaFin);
                    if (idContratoActual.HasValue)
                    {
                        command.Parameters.AddWithValue("@IdContratoActual", idContratoActual);
                    }

                    connection.Open();
                    var count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        public IList<Contrato> ObtenerContratosPorPlazo(int plazoDias, int pagina = 1, int tamPagina = 10)
        {
            IList<Contrato> res = new List<Contrato>();
            using (var connection = new MySqlConnection(connectionString))
            {
                var fechaObjetivoInicio = DateTime.Today;
                var fechaObjetivoFin = DateTime.Today.AddDays(plazoDias);

                string sql = @$"
                    SELECT c.*, 
                        i.Nombre AS InquilinoNombre, i.Apellido AS InquilinoApellido,
                        inm.Direccion, 
                        uc.Nombre AS UsuarioCreacionNombre, uc.Apellido AS UsuarioCreacionApellido
                    FROM contratos c
                    INNER JOIN inquilinos i ON c.IdInquilino = i.IdInquilino
                    INNER JOIN inmuebles inm ON c.IdInmueble = inm.IdInmueble
                    INNER JOIN usuarios uc ON c.IdUsuarioCreacion = uc.IdUsuario
                    WHERE c.Estado = 1 
                    AND IFNULL(c.FechaFinAnticipado, c.FechaFin) BETWEEN @fechaObjetivoInicio AND @fechaObjetivoFin
                    ORDER BY c.FechaFin ASC
                    LIMIT {tamPagina} OFFSET {(pagina - 1) * tamPagina}";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@fechaObjetivoInicio", fechaObjetivoInicio);
                    command.Parameters.AddWithValue("@fechaObjetivoFin", fechaObjetivoFin);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Contrato c = new Contrato
                            {
                                IdContrato = reader.GetInt32("IdContrato"),
                                IdInquilino = reader.GetInt32("IdInquilino"),
                                IdInmueble = reader.GetInt32("IdInmueble"),
                                MontoMensual = reader.GetDecimal("MontoMensual"),
                                FechaInicio = reader.GetDateTime("FechaInicio"),
                                FechaFin = reader.GetDateTime("FechaFin"),
                                PagosEsperados = reader.GetInt32("PagosEsperados"),
                                Estado = reader.GetInt32("Estado"),
                                Inquilino = new Inquilino
                                {
                                    IdInquilino = reader.GetInt32("IdInquilino"),
                                    Nombre = reader.GetString("InquilinoNombre"),
                                    Apellido = reader.GetString("InquilinoApellido")
                                },
                                Inmueble = new Inmueble
                                {
                                    IdInmueble = reader.GetInt32("IdInmueble"),
                                    Direccion = reader.GetString("Direccion")
                                }
                            };
                            res.Add(c);
                        }
                    }
                }
            }
            return res;
        }

        public int ObtenerCantidadContratosPorPlazo(int plazoDias)
        {
            int cantidad = 0;
            using (var connection = new MySqlConnection(connectionString))
            {
                var fechaObjetivoInicio = DateTime.Today;
                var fechaObjetivoFin = DateTime.Today.AddDays(plazoDias);

                var query = "SELECT COUNT(*) FROM contratos WHERE Estado = 1 AND IFNULL(FechaFinAnticipado, FechaFin) BETWEEN @fechaObjetivoInicio AND @fechaObjetivoFin";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@fechaObjetivoInicio", fechaObjetivoInicio);
                    command.Parameters.AddWithValue("@fechaObjetivoFin", fechaObjetivoFin);
                    connection.Open();
                    cantidad = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return cantidad;
        }

        public void AnularContratosVencidos()
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    var query = @"
                        UPDATE contratos 
                        SET Estado = 0,
                            FechaFinAnticipado = CURRENT_DATE()
                        WHERE Estado = 1 
                        AND IFNULL(FechaFinAnticipado, FechaFin) < CURRENT_DATE()";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
    }
}