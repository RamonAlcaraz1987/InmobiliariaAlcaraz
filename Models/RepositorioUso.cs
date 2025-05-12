using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace InmobiliariaAlcaraz.Models
{
    public class RepositorioUso : RepositorioBase, IRepositorioUso
    {
        public RepositorioUso(IConfiguration configuration) : base(configuration) { }

        public int Alta(Uso uso)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"INSERT INTO usos (Descripcion) VALUES (@descripcion); SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(query, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@descripcion", uso.Descripcion);
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public int Baja(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"DELETE FROM usos WHERE IdUso = @id";
                using (var command = new MySqlCommand(query, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    return command.ExecuteNonQuery();
                }
            }
        }

        public int Modificacion(Uso uso)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"UPDATE usos SET Descripcion = @descripcion WHERE IdUso = @id";
                using (var command = new MySqlCommand(query, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@descripcion", uso.Descripcion);
                    command.Parameters.AddWithValue("@id", uso.IdUso);
                    return command.ExecuteNonQuery();
                }
            }
        }

        public IList<Uso> ObtenerTodos()
        {
            var usos = new List<Uso>();
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"SELECT IdUso, Descripcion FROM usos";
                using (var command = new MySqlCommand(query, (MySqlConnection)connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            usos.Add(new Uso
                            {
                                IdUso = reader.GetInt32("IdUso"),
                                Descripcion = reader.GetString("Descripcion")
                            });
                        }
                    }
                }
            }
            return usos;
        }

        public Uso ObtenerPorId(int id)
        {
            Uso uso = null;
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"SELECT IdUso, Descripcion FROM usos WHERE IdUso = @id";
                using (var command = new MySqlCommand(query, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            uso = new Uso
                            {
                                IdUso = reader.GetInt32("IdUso"),
                                Descripcion = reader.GetString("Descripcion")
                            };
                        }
                    }
                }
            }
            return uso;
        }

        public IList<Uso> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            var lista = new List<Uso>();
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @$"
                    SELECT IdUso, Descripcion
                    FROM usos
                    ORDER BY IdUso
                    LIMIT {tamPagina} OFFSET {(paginaNro - 1) * tamPagina}
                ";
                using (var command = new MySqlCommand(query, (MySqlConnection)connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Uso
                            {
                                IdUso = reader.GetInt32("IdUso"),
                                Descripcion = reader.GetString("Descripcion")
                            });
                        }
                    }
                }
            }
            return lista;
        }
    }
}