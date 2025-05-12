using System.Data;
using MySql.Data.MySqlClient;
using InmobiliariaAlcaraz.Models;
using System.Collections.Generic;

namespace InmobiliariaAlcaraz.Models
{
    public class RepositorioInmueble : RepositorioBase, IRepositorioInmueble
    {
        public RepositorioInmueble(IConfiguration configuration) : base(configuration)
        {
        }

        // Método para obtener todos los inmuebles
        public IList<Inmueble> ObtenerTodos()
        {
            var inmuebles = new List<Inmueble>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT i.IdInmueble, i.Direccion, i.IdTipo, i.IdUso, i.IdPropietario, i.Ambientes,
                                   i.Latitud, i.Longitud, i.Superficie, i.Disponible, i.Precio,
                                   p.Nombre, p.Apellido
                            FROM inmuebles i
                            INNER JOIN propietarios p ON i.IdPropietario = p.IdPropietario;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            inmuebles.Add(new Inmueble
                            {
                                IdInmueble = reader.GetInt32("IdInmueble"),
                                Direccion = reader.GetString("Direccion"),
                                IdTipo = reader.GetInt32("IdTipo"),
                                IdUso = reader.GetInt32("IdUso"),
                                IdPropietario = reader.GetInt32("IdPropietario"),
                                Ambientes = reader.GetInt32("Ambientes"),
                                Latitud = reader.GetDecimal("Latitud"),
                                Longitud = reader.GetDecimal("Longitud"),
                                Superficie = reader.GetInt32("Superficie"),
                                Disponible = reader.GetInt32("Disponible"),
                                Precio = reader.GetDecimal("Precio"),
                                Duenio = new Propietario
                                {
                                    IdPropietario = reader.GetInt32("IdPropietario"),
                                    Nombre = reader.GetString("Nombre"),
                                    Apellido = reader.GetString("Apellido")
                                }
                            });
                        }
                    }
                }
            }

            return inmuebles;
        }

        // Método para obtener un inmueble por su ID
        public Inmueble ObtenerPorId(int id)
            {
                Inmueble inmueble = null;

                using (var connection = new MySqlConnection(connectionString))
                {
                    // Consulta para obtener los detalles del inmueble
                    var sql = @"SELECT i.IdInmueble, i.Direccion, i.IdTipo, i.IdUso, i.IdPropietario, i.Ambientes,
                                    i.Latitud, i.Longitud, i.Superficie, i.Disponible, i.Precio,
                                    p.Nombre, p.Apellido
                                FROM inmuebles i
                                INNER JOIN propietarios p ON i.IdPropietario = p.IdPropietario
                                WHERE i.IdInmueble = @id";

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        connection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                inmueble = new Inmueble
                                {
                                    IdInmueble = reader.GetInt32("IdInmueble"),
                                    Direccion = reader.GetString("Direccion"),
                                    IdTipo = reader.GetInt32("IdTipo"),
                                    IdUso = reader.GetInt32("IdUso"),
                                    IdPropietario = reader.GetInt32("IdPropietario"),
                                    Ambientes = reader.GetInt32("Ambientes"),
                                    Latitud = reader.GetDecimal("Latitud"),
                                    Longitud = reader.GetDecimal("Longitud"),
                                    Superficie = reader.GetInt32("Superficie"),
                                    Disponible = reader.GetInt32("Disponible"),
                                    Precio = reader.GetDecimal("Precio"),
                                    Duenio = new Propietario
                                    {
                                        IdPropietario = reader.GetInt32("IdPropietario"),
                                        Nombre = reader.GetString("Nombre"),
                                        Apellido = reader.GetString("Apellido")
                                    },
                                    // Inicializamos la lista de imágenes
                                    Imagenes = new List<Imagen>()
                                };
                            }
                        }
                    }

                    // Consulta para obtener las imágenes asociadas al inmueble
                    var sqlImagenes = @"SELECT IdImagen, PortadaUrl FROM imagenes WHERE IdInmueble = @id";
                    using (var command = new MySqlCommand(sqlImagenes, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                inmueble.Imagenes.Add(new Imagen
                                {
                                    IdImagen = reader.GetInt32("IdImagen"),
                                    Url = reader.GetString("PortadaUrl"),
                                    IdInmueble = id
                                });
                            }
                        }
                    }
                }

                return inmueble;
            }

        // Método para obtener una lista de inmuebles con paginación
        public IList<Inmueble> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            IList<Inmueble> inmuebles = new List<Inmueble>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @$"
                    SELECT i.IdInmueble, i.Direccion, i.IdTipo, i.IdUso, i.IdPropietario, i.Ambientes, 
                           i.Latitud, i.Longitud, i.Superficie, i.Disponible, i.Precio,
                           p.Nombre, p.Apellido
                    FROM inmuebles i
                    INNER JOIN propietarios p ON i.IdPropietario = p.IdPropietario
                    ORDER BY i.IdInmueble
                    LIMIT {tamPagina} OFFSET {(paginaNro - 1) * tamPagina}";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            inmuebles.Add(new Inmueble
                            {
                                IdInmueble = reader.GetInt32("IdInmueble"),
                                Direccion = reader.GetString("Direccion"),
                                IdTipo = reader.GetInt32("IdTipo"),
                                IdUso = reader.GetInt32("IdUso"),
                                IdPropietario = reader.GetInt32("IdPropietario"),
                                Ambientes = reader.GetInt32("Ambientes"),
                                Latitud = reader.GetDecimal("Latitud"),
                                Longitud = reader.GetDecimal("Longitud"),
                                Superficie = reader.GetInt32("Superficie"),
                                Disponible = reader.GetInt32("Disponible"),
                                Precio = reader.GetDecimal("Precio"),
                                Duenio = new Propietario
                                {
                                    IdPropietario = reader.GetInt32("IdPropietario"),
                                    Nombre = reader.GetString("Nombre"),
                                    Apellido = reader.GetString("Apellido")
                                }
                            });
                        }
                    }
                }
            }

            return inmuebles;
        }

        // Método para agregar un nuevo inmueble
        public int Alta(Inmueble inmueble)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var query = @"INSERT INTO inmuebles (Direccion, IdTipo, IdUso, IdPropietario, Ambientes, Latitud, Longitud, Superficie, Disponible, Precio)
                              VALUES (@direccion, @idTipo, @idUso, @idPropietario, @ambientes, @latitud, @longitud, @superficie, @disponible, @precio);
                              SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@direccion", inmueble.Direccion);
                    command.Parameters.AddWithValue("@idTipo", inmueble.IdTipo);
                    command.Parameters.AddWithValue("@idUso", inmueble.IdUso);
                    command.Parameters.AddWithValue("@idPropietario", inmueble.IdPropietario);
                    command.Parameters.AddWithValue("@ambientes", inmueble.Ambientes);
                    command.Parameters.AddWithValue("@latitud", inmueble.Latitud);
                    command.Parameters.AddWithValue("@longitud", inmueble.Longitud);
                    command.Parameters.AddWithValue("@superficie", inmueble.Superficie);
                    command.Parameters.AddWithValue("@disponible", inmueble.Disponible);
                    command.Parameters.AddWithValue("@precio", inmueble.Precio);
                    
                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        // Método para eliminar un inmueble
        public int Baja(int id)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var query = @"DELETE FROM inmuebles WHERE IdInmueble = @id";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        // Método para modificar un inmueble
        public int Modificacion(Inmueble inmueble)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var query = @"UPDATE inmuebles
                              SET Direccion = @direccion, IdTipo = @idTipo, IdUso = @idUso, IdPropietario = @idPropietario, 
                                  Ambientes = @ambientes, Latitud = @latitud, Longitud = @longitud, Superficie = @superficie, 
                                  Disponible = @disponible, Precio = @precio
                              WHERE IdInmueble = @idInmueble";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@direccion", inmueble.Direccion);
                    command.Parameters.AddWithValue("@idTipo", inmueble.IdTipo);
                    command.Parameters.AddWithValue("@idUso", inmueble.IdUso);
                    command.Parameters.AddWithValue("@idPropietario", inmueble.IdPropietario);
                    command.Parameters.AddWithValue("@ambientes", inmueble.Ambientes);
                    command.Parameters.AddWithValue("@latitud", inmueble.Latitud);
                    command.Parameters.AddWithValue("@longitud", inmueble.Longitud);
                    command.Parameters.AddWithValue("@superficie", inmueble.Superficie);
                    command.Parameters.AddWithValue("@disponible", inmueble.Disponible);
                    command.Parameters.AddWithValue("@precio", inmueble.Precio);
                    command.Parameters.AddWithValue("@idInmueble", inmueble.IdInmueble);

                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }
        public int Contar()
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    var sql = "SELECT COUNT(*) FROM inmuebles;";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        connection.Open();
                        return Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            

            public IList<Inmueble> ObtenerPorDisponibilidad(int disponible, int paginaNro = 1, int tamPagina = 10)
            {
                IList<Inmueble> inmuebles = new List<Inmueble>();

                using (var connection = new MySqlConnection(connectionString))
                {
                    var sql = @$"
                        SELECT i.IdInmueble, i.Direccion, i.IdTipo, i.IdUso, i.IdPropietario, i.Ambientes, 
                            i.Latitud, i.Longitud, i.Superficie, i.Disponible, i.Precio,
                            p.Nombre, p.Apellido
                        FROM inmuebles i
                        INNER JOIN propietarios p ON i.IdPropietario = p.IdPropietario
                        WHERE i.Disponible = @disponible
                        ORDER BY i.IdInmueble
                        LIMIT {tamPagina} OFFSET {(paginaNro - 1) * tamPagina}";

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@disponible", disponible);
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                inmuebles.Add(new Inmueble
                                {
                                    IdInmueble = reader.GetInt32("IdInmueble"),
                                    Direccion = reader.GetString("Direccion"),
                                    IdTipo = reader.GetInt32("IdTipo"),
                                    IdUso = reader.GetInt32("IdUso"),
                                    IdPropietario = reader.GetInt32("IdPropietario"),
                                    Ambientes = reader.GetInt32("Ambientes"),
                                    Latitud = reader.GetDecimal("Latitud"),
                                    Longitud = reader.GetDecimal("Longitud"),
                                    Superficie = reader.GetInt32("Superficie"),
                                    Disponible = reader.GetInt32("Disponible"),
                                    Precio = reader.GetDecimal("Precio"),
                                    Duenio = new Propietario
                                    {
                                        IdPropietario = reader.GetInt32("IdPropietario"),
                                        Nombre = reader.GetString("Nombre"),
                                        Apellido = reader.GetString("Apellido")
                                    }
                                });
                            }
                        }
                    }
                }

                return inmuebles;
            }

            public int ContarPorDisponibilidad(int disponible)
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    var sql = "SELECT COUNT(*) FROM inmuebles WHERE Disponible = @disponible";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@disponible", disponible);
                        connection.Open();
                        return Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }

            public IList<Inmueble> ObtenerPorPropietario(int idPropietario, int paginaNro = 1, int tamPagina = 10)
            {
                IList<Inmueble> inmuebles = new List<Inmueble>();

                using (var connection = new MySqlConnection(connectionString))
                {
                    var sql = @$"
                        SELECT i.IdInmueble, i.Direccion, i.IdTipo, i.IdUso, i.IdPropietario, i.Ambientes, 
                            i.Latitud, i.Longitud, i.Superficie, i.Disponible, i.Precio,
                            p.Nombre, p.Apellido
                        FROM inmuebles i
                        INNER JOIN propietarios p ON i.IdPropietario = p.IdPropietario
                        WHERE i.IdPropietario = @idPropietario
                        ORDER BY i.IdInmueble
                        LIMIT {tamPagina} OFFSET {(paginaNro - 1) * tamPagina}";

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@idPropietario", idPropietario);
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                inmuebles.Add(new Inmueble
                                {
                                    IdInmueble = reader.GetInt32("IdInmueble"),
                                    Direccion = reader.GetString("Direccion"),
                                    IdTipo = reader.GetInt32("IdTipo"),
                                    IdUso = reader.GetInt32("IdUso"),
                                    IdPropietario = reader.GetInt32("IdPropietario"),
                                    Ambientes = reader.GetInt32("Ambientes"),
                                    Latitud = reader.GetDecimal("Latitud"),
                                    Longitud = reader.GetDecimal("Longitud"),
                                    Superficie = reader.GetInt32("Superficie"),
                                    Disponible = reader.GetInt32("Disponible"),
                                    Precio = reader.GetDecimal("Precio"),
                                    Duenio = new Propietario
                                    {
                                        IdPropietario = reader.GetInt32("IdPropietario"),
                                        Nombre = reader.GetString("Nombre"),
                                        Apellido = reader.GetString("Apellido")
                                    }
                                });
                            }
                        }
                    }
                }

                return inmuebles;
            }

            public int ContarPorPropietario(int idPropietario)
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    var sql = "SELECT COUNT(*) FROM inmuebles WHERE IdPropietario = @idPropietario";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@idPropietario", idPropietario);
                        connection.Open();
                        return Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
       public IList<Inmueble> ObtenerPorFechasDisponibles(DateTime fechaDesde, DateTime fechaHasta, int pagina = 1, int tamPagina = 10)
        {
            var inmuebles = new List<Inmueble>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @$"
                    SELECT i.IdInmueble, i.Direccion, i.IdTipo, i.IdUso, i.IdPropietario, i.Ambientes, 
                           i.Latitud, i.Longitud, i.Superficie, i.Disponible, i.Precio,
                           p.Nombre, p.Apellido
                    FROM inmuebles i
                    INNER JOIN propietarios p ON i.IdPropietario = p.IdPropietario
                    WHERE i.IdInmueble NOT IN (
                        SELECT c.IdInmueble
                        FROM contratos c
                        WHERE c.Estado = 1
                        AND c.FechaInicio <= @fechaHasta
                        AND (
                            CASE 
                                WHEN c.FechaFinAnticipado IS NOT NULL THEN c.FechaFinAnticipado
                                ELSE c.FechaFin
                            END >= @fechaDesde
                        )
                    )
                    ORDER BY i.Direccion
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
                            inmuebles.Add(new Inmueble
                            {
                                IdInmueble = reader.GetInt32("IdInmueble"),
                                Direccion = reader.GetString("Direccion"),
                                IdTipo = reader.GetInt32("IdTipo"),
                                IdUso = reader.GetInt32("IdUso"),
                                IdPropietario = reader.GetInt32("IdPropietario"),
                                Ambientes = reader.GetInt32("Ambientes"),
                                Latitud = reader.GetDecimal("Latitud"),
                                Longitud = reader.GetDecimal("Longitud"),
                                Superficie = reader.GetInt32("Superficie"),
                                Disponible = reader.GetInt32("Disponible"),
                                Precio = reader.GetDecimal("Precio"),
                                Duenio = new Propietario
                                {
                                    IdPropietario = reader.GetInt32("IdPropietario"),
                                    Nombre = reader.GetString("Nombre"),
                                    Apellido = reader.GetString("Apellido")
                                }
                            });
                        }
                    }
                }
            }

            return inmuebles;
        }

        public int ContarPorFechasDisponibles(DateTime fechaDesde, DateTime fechaHasta)
        {
            int cantidad = 0;
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"
                    SELECT COUNT(*)
                    FROM inmuebles i
                    WHERE i.IdInmueble NOT IN (
                        SELECT c.IdInmueble
                        FROM contratos c
                        WHERE c.Estado = 1
                        AND c.FechaInicio <= @fechaHasta
                        AND (
                            CASE 
                                WHEN c.FechaFinAnticipado IS NOT NULL THEN c.FechaFinAnticipado
                                ELSE c.FechaFin
                            END >= @fechaDesde
                        )
                    )";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@fechaDesde", fechaDesde);
                    command.Parameters.AddWithValue("@fechaHasta", fechaHasta);
                    connection.Open();
                    cantidad = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return cantidad;
        }
    }
}