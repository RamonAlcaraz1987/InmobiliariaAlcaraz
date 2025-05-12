using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

namespace InmobiliariaAlcaraz.Models
{
    public class RepositorioPago : RepositorioBase, IRepositorioPago
    {
        public RepositorioPago(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Pago pago)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var query = @"INSERT INTO pagos 
                            (IdContrato, Monto, FechaPago, NumeroPago, Detalle, Anulado, 
                             IdUsuarioCreacion, IdUsuarioAnulacion, FechaCreacion, FechaAnulacion, EsMulta)
                            VALUES 
                            (@IdContrato, @Monto, @FechaPago, @NumeroPago, @Detalle, @Anulado, 
                             @IdUsuarioCreacion, @IdUsuarioAnulacion, @FechaCreacion, @FechaAnulacion, @EsMulta);
                            SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdContrato", pago.IdContrato);
                    command.Parameters.AddWithValue("@Monto", pago.Monto);
                    command.Parameters.AddWithValue("@FechaPago", pago.FechaPago.HasValue ? pago.FechaPago : DBNull.Value);
                    command.Parameters.AddWithValue("@NumeroPago", pago.NumeroPago);
                    command.Parameters.AddWithValue("@Detalle", pago.Detalle);
                    command.Parameters.AddWithValue("@Anulado", pago.Anulado);
                    command.Parameters.AddWithValue("@IdUsuarioCreacion", pago.IdUsuarioCreacion);
                    command.Parameters.AddWithValue("@IdUsuarioAnulacion", pago.IdUsuarioAnulacion.HasValue ? pago.IdUsuarioAnulacion : DBNull.Value);
                    command.Parameters.AddWithValue("@FechaCreacion", pago.FechaCreacion);
                    command.Parameters.AddWithValue("@FechaAnulacion", pago.FechaAnulacion.HasValue ? pago.FechaAnulacion : DBNull.Value);
                    command.Parameters.AddWithValue("@EsMulta", pago.EsMulta);

                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public int Baja(int id)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var query = @"UPDATE pagos SET 
                             Anulado = 1,
                             FechaAnulacion = CURRENT_TIMESTAMP()
                             WHERE IdPago = @Id";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public int Modificacion(Pago pago)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var query = @"UPDATE pagos SET 
                             IdContrato = @IdContrato,
                             Monto = @Monto,
                             FechaPago = @FechaPago,
                             NumeroPago = @NumeroPago,
                             Detalle = @Detalle,
                             Anulado = @Anulado,
                             IdUsuarioCreacion = @IdUsuarioCreacion,
                             IdUsuarioAnulacion = @IdUsuarioAnulacion,
                             FechaCreacion = @FechaCreacion,
                             FechaAnulacion = @FechaAnulacion,
                             EsMulta = @EsMulta
                             WHERE IdPago = @IdPago";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdPago", pago.IdPago);
                    command.Parameters.AddWithValue("@IdContrato", pago.IdContrato);
                    command.Parameters.AddWithValue("@Monto", pago.Monto);
                    command.Parameters.AddWithValue("@FechaPago", pago.FechaPago.HasValue ? pago.FechaPago : DBNull.Value);
                    command.Parameters.AddWithValue("@NumeroPago", pago.NumeroPago);
                    command.Parameters.AddWithValue("@Detalle", pago.Detalle);
                    command.Parameters.AddWithValue("@Anulado", pago.Anulado);
                    command.Parameters.AddWithValue("@IdUsuarioCreacion", pago.IdUsuarioCreacion);
                    command.Parameters.AddWithValue("@IdUsuarioAnulacion", pago.IdUsuarioAnulacion.HasValue ? pago.IdUsuarioAnulacion : DBNull.Value);
                    command.Parameters.AddWithValue("@FechaCreacion", pago.FechaCreacion);
                    command.Parameters.AddWithValue("@FechaAnulacion", pago.FechaAnulacion.HasValue ? pago.FechaAnulacion : DBNull.Value);
                    command.Parameters.AddWithValue("@EsMulta", pago.EsMulta);

                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public IList<Pago> ObtenerTodos()
        {
            return ObtenerTodos(1, 10);
        }

        public IList<Pago> ObtenerTodos(int paginaNro = 1, int tamPagina = 10)
        {
            var pagos = new List<Pago>();
            using (var connection = new MySqlConnection(connectionString))
            {
                var query = @$"
                            SELECT 
                                p.IdPago,
                                p.IdContrato,
                                p.Monto,
                                p.FechaPago,
                                p.NumeroPago,
                                p.Detalle,
                                p.Anulado,
                                p.IdUsuarioCreacion,
                                p.IdUsuarioAnulacion,
                                p.FechaCreacion,
                                p.FechaAnulacion,
                                p.EsMulta,
                                c.IdContrato AS ContratoId,
                                uc.Nombre AS UsuarioCreacionNombre,
                                uc.Apellido AS UsuarioCreacionApellido,
                                ua.Nombre AS UsuarioAnulacionNombre,
                                ua.Apellido AS UsuarioAnulacionApellido
                            FROM pagos p
                            INNER JOIN contratos c ON p.IdContrato = c.IdContrato
                            INNER JOIN usuarios uc ON p.IdUsuarioCreacion = uc.IdUsuario
                            LEFT JOIN usuarios ua ON p.IdUsuarioAnulacion = ua.IdUsuario
                            ORDER BY p.IdPago
                            LIMIT {tamPagina} OFFSET {(paginaNro - 1) * tamPagina}";

                using (var command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pagos.Add(new Pago
                            {
                                IdPago = reader.GetInt32(reader.GetOrdinal("IdPago")),
                                IdContrato = reader.GetInt32(reader.GetOrdinal("IdContrato")),
                                Monto = reader.GetDecimal(reader.GetOrdinal("Monto")),
                                FechaPago = reader.IsDBNull(reader.GetOrdinal("FechaPago")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaPago")),
                                NumeroPago = reader.GetInt32(reader.GetOrdinal("NumeroPago")),
                                Detalle = reader.IsDBNull(reader.GetOrdinal("Detalle")) ? null : reader.GetString(reader.GetOrdinal("Detalle")),
                                Anulado = reader.GetBoolean(reader.GetOrdinal("Anulado")),
                                IdUsuarioCreacion = reader.GetInt32(reader.GetOrdinal("IdUsuarioCreacion")),
                                IdUsuarioAnulacion = reader.IsDBNull(reader.GetOrdinal("IdUsuarioAnulacion")) ? null : reader.GetInt32(reader.GetOrdinal("IdUsuarioAnulacion")),
                                FechaCreacion = reader.GetDateTime(reader.GetOrdinal("FechaCreacion")),
                                FechaAnulacion = reader.IsDBNull(reader.GetOrdinal("FechaAnulacion")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaAnulacion")),
                                EsMulta = reader.GetInt32(reader.GetOrdinal("EsMulta")),
                                Contrato = new Contrato { IdContrato = reader.GetInt32(reader.GetOrdinal("ContratoId")) },
                                UsuarioCreacion = new Usuario
                                {
                                    IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuarioCreacion")),
                                    Nombre = reader.GetString(reader.GetOrdinal("UsuarioCreacionNombre")),
                                    Apellido = reader.GetString(reader.GetOrdinal("UsuarioCreacionApellido"))
                                },
                                UsuarioAnulacion = reader.IsDBNull(reader.GetOrdinal("IdUsuarioAnulacion")) ? null : new Usuario
                                {
                                    IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuarioAnulacion")),
                                    Nombre = reader.GetString(reader.GetOrdinal("UsuarioAnulacionNombre")),
                                    Apellido = reader.GetString(reader.GetOrdinal("UsuarioAnulacionApellido"))
                                }
                            });
                        }
                    }
                }
            }
            return pagos;
        }

        public int Contar()
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "SELECT COUNT(*) FROM pagos;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }
        public Pago ObtenerPorId(int id)
        {
            Pago pago = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                var query = @"SELECT 
                            p.IdPago,
                            p.IdContrato,
                            p.Monto,
                            p.FechaPago,
                            p.NumeroPago,
                            p.Detalle,
                            p.Anulado,
                            p.IdUsuarioCreacion,
                            p.IdUsuarioAnulacion,
                            p.FechaCreacion,
                            p.FechaAnulacion,
                            p.EsMulta,
                            c.IdContrato AS ContratoId,
                            uc.Nombre AS UsuarioCreacionNombre,
                            uc.Apellido AS UsuarioCreacionApellido,
                            ua.Nombre AS UsuarioAnulacionNombre,
                            ua.Apellido AS UsuarioAnulacionApellido
                        FROM pagos p
                        INNER JOIN contratos c ON p.IdContrato = c.IdContrato
                        INNER JOIN usuarios uc ON p.IdUsuarioCreacion = uc.IdUsuario
                        LEFT JOIN usuarios ua ON p.IdUsuarioAnulacion = ua.IdUsuario
                        WHERE p.IdPago = @Id";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            pago = new Pago
                            {
                                IdPago = reader.GetInt32(reader.GetOrdinal("IdPago")),
                                IdContrato = reader.GetInt32(reader.GetOrdinal("IdContrato")),
                                Monto = reader.GetDecimal(reader.GetOrdinal("Monto")),
                                FechaPago = reader.IsDBNull(reader.GetOrdinal("FechaPago")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaPago")),
                                NumeroPago = reader.GetInt32(reader.GetOrdinal("NumeroPago")),
                                Detalle = reader.IsDBNull(reader.GetOrdinal("Detalle")) ? string.Empty : reader.GetString(reader.GetOrdinal("Detalle")),
                                Anulado = reader.GetBoolean(reader.GetOrdinal("Anulado")),
                                IdUsuarioCreacion = reader.GetInt32(reader.GetOrdinal("IdUsuarioCreacion")),
                                IdUsuarioAnulacion = reader.IsDBNull(reader.GetOrdinal("IdUsuarioAnulacion")) ? null : reader.GetInt32(reader.GetOrdinal("IdUsuarioAnulacion")),
                                FechaCreacion = reader.GetDateTime(reader.GetOrdinal("FechaCreacion")),
                                FechaAnulacion = reader.IsDBNull(reader.GetOrdinal("FechaAnulacion")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaAnulacion")),
                                EsMulta = reader.GetInt32(reader.GetOrdinal("EsMulta")),
                                Contrato = new Contrato { IdContrato = reader.GetInt32(reader.GetOrdinal("ContratoId")) },
                                UsuarioCreacion = new Usuario
                                {
                                    IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuarioCreacion")),
                                    Nombre = reader.IsDBNull(reader.GetOrdinal("UsuarioCreacionNombre")) ? string.Empty : reader.GetString(reader.GetOrdinal("UsuarioCreacionNombre")),
                                    Apellido = reader.IsDBNull(reader.GetOrdinal("UsuarioCreacionApellido")) ? string.Empty : reader.GetString(reader.GetOrdinal("UsuarioCreacionApellido"))
                                },
                                UsuarioAnulacion = reader.IsDBNull(reader.GetOrdinal("IdUsuarioAnulacion")) ? null : new Usuario
                                {
                                    IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuarioAnulacion")),
                                    Nombre = reader.IsDBNull(reader.GetOrdinal("UsuarioAnulacionNombre")) ? string.Empty : reader.GetString(reader.GetOrdinal("UsuarioAnulacionNombre")),
                                    Apellido = reader.IsDBNull(reader.GetOrdinal("UsuarioAnulacionApellido")) ? string.Empty : reader.GetString(reader.GetOrdinal("UsuarioAnulacionApellido"))
                                }
                            };
                        }
                    }
                }
                return pago;
            }
        }
        public IList<Pago> ObtenerPorContrato(int idContrato, int paginaNro = 1, int tamPagina = 10)
        {
            var pagos = new List<Pago>();
            using (var connection = new MySqlConnection(connectionString))
            {
                var query = @$"
                            SELECT 
                                p.IdPago,
                                p.IdContrato,
                                p.Monto,
                                p.FechaPago,
                                p.NumeroPago,
                                p.Detalle,
                                p.Anulado,
                                p.IdUsuarioCreacion,
                                p.IdUsuarioAnulacion,
                                p.FechaCreacion,
                                p.FechaAnulacion,
                                p.EsMulta,
                                c.IdContrato AS ContratoId,
                                uc.Nombre AS UsuarioCreacionNombre,
                                uc.Apellido AS UsuarioCreacionApellido,
                                ua.Nombre AS UsuarioAnulacionNombre,
                                ua.Apellido AS UsuarioAnulacionApellido
                            FROM pagos p
                            INNER JOIN contratos c ON p.IdContrato = c.IdContrato
                            INNER JOIN usuarios uc ON p.IdUsuarioCreacion = uc.IdUsuario
                            LEFT JOIN usuarios ua ON p.IdUsuarioAnulacion = ua.IdUsuario
                            WHERE p.IdContrato = @IdContrato
                            ORDER BY p.IdPago
                            LIMIT {tamPagina} OFFSET {(paginaNro - 1) * tamPagina}";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdContrato", idContrato);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pagos.Add(new Pago
                            {
                                IdPago = reader.GetInt32(reader.GetOrdinal("IdPago")),
                                IdContrato = reader.GetInt32(reader.GetOrdinal("IdContrato")),
                                Monto = reader.GetDecimal(reader.GetOrdinal("Monto")),
                                FechaPago = reader.IsDBNull(reader.GetOrdinal("FechaPago")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaPago")),
                                NumeroPago = reader.GetInt32(reader.GetOrdinal("NumeroPago")),
                                Detalle = reader.IsDBNull(reader.GetOrdinal("Detalle")) ? null : reader.GetString(reader.GetOrdinal("Detalle")),
                                Anulado = reader.GetBoolean(reader.GetOrdinal("Anulado")),
                                IdUsuarioCreacion = reader.GetInt32(reader.GetOrdinal("IdUsuarioCreacion")),
                                IdUsuarioAnulacion = reader.IsDBNull(reader.GetOrdinal("IdUsuarioAnulacion")) ? null : reader.GetInt32(reader.GetOrdinal("IdUsuarioAnulacion")),
                                FechaCreacion = reader.GetDateTime(reader.GetOrdinal("FechaCreacion")),
                                FechaAnulacion = reader.IsDBNull(reader.GetOrdinal("FechaAnulacion")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaAnulacion")),
                                EsMulta = reader.GetInt32(reader.GetOrdinal("EsMulta")),
                                Contrato = new Contrato { IdContrato = reader.GetInt32(reader.GetOrdinal("ContratoId")) },
                                UsuarioCreacion = new Usuario
                                {
                                    IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuarioCreacion")),
                                    Nombre = reader.GetString(reader.GetOrdinal("UsuarioCreacionNombre")),
                                    Apellido = reader.GetString(reader.GetOrdinal("UsuarioCreacionApellido"))
                                },
                                UsuarioAnulacion = reader.IsDBNull(reader.GetOrdinal("IdUsuarioAnulacion")) ? null : new Usuario
                                {
                                    IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuarioAnulacion")),
                                    Nombre = reader.GetString(reader.GetOrdinal("UsuarioAnulacionNombre")),
                                    Apellido = reader.GetString(reader.GetOrdinal("UsuarioAnulacionApellido"))
                                }
                            });
                        }
                    }
                }
            }
            return pagos;
        }

        public int ContarPorContrato(int idContrato)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "SELECT COUNT(*) FROM pagos WHERE IdContrato = @IdContrato";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdContrato", idContrato);
                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }
    }
}