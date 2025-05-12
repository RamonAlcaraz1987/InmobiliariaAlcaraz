using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace InmobiliariaAlcaraz.Models
{
    public class RepositorioTipo : RepositorioBase, IRepositorioTipo
    {
        public RepositorioTipo(IConfiguration configuration) : base(configuration) { }

        public int Alta(Tipo tipo)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"INSERT INTO tipos (Descripcion) VALUES (@descripcion); SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(query, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@descripcion", tipo.Descripcion);
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public int Baja(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"DELETE FROM tipos WHERE IdTipo = @id";
                using (var command = new MySqlCommand(query, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    return command.ExecuteNonQuery();
                }
            }
        }

        public int Modificacion(Tipo tipo)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"UPDATE tipos SET Descripcion = @descripcion WHERE IdTipo = @id";
                using (var command = new MySqlCommand(query, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@descripcion", tipo.Descripcion);
                    command.Parameters.AddWithValue("@id", tipo.IdTipo);
                    return command.ExecuteNonQuery();
                }
            }
        }

        public IList<Tipo> ObtenerTodos()
        {
            var tipos = new List<Tipo>();
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"SELECT IdTipo, Descripcion FROM tipos";
                using (var command = new MySqlCommand(query, (MySqlConnection)connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tipos.Add(new Tipo
                            {
                                IdTipo = reader.GetInt32("IdTipo"),
                                Descripcion = reader.GetString("Descripcion")
                            });
                        }
                    }
                }
            }
            return tipos;
        }

        public Tipo ObtenerPorId(int id)
        {
            Tipo tipo = null;
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"SELECT IdTipo, Descripcion FROM tipos WHERE IdTipo = @id";
                using (var command = new MySqlCommand(query, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            tipo = new Tipo
                            {
                                IdTipo = reader.GetInt32("IdTipo"),
                                Descripcion = reader.GetString("Descripcion")
                            };
                        }
                    }
                }
            }
            return tipo;
        }

        public IList<Tipo> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            var lista = new List<Tipo>();
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @$"
                    SELECT IdTipo, Descripcion
                    FROM tipos
                    ORDER BY IdTipo
                    LIMIT {tamPagina} OFFSET {(paginaNro - 1) * tamPagina}
                ";
                using (var command = new MySqlCommand(query, (MySqlConnection)connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Tipo
                            {
                                IdTipo = reader.GetInt32("IdTipo"),
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