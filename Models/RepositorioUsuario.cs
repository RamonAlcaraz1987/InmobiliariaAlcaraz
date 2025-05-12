using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace InmobiliariaAlcaraz.Models
{
    public class RepositorioUsuario : RepositorioBase, IRepositorioUsuario
    {
        public RepositorioUsuario(IConfiguration configuration) : base(configuration) { }

        public int Alta(Usuario e)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"INSERT INTO usuarios (Nombre, Apellido, Avatar, Email, Clave, Rol) 
                              VALUES (@nombre, @apellido, @avatar, @email, @clave, @rol);
                              SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(query, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@nombre", e.Nombre);
                    command.Parameters.AddWithValue("@apellido", e.Apellido);
                    command.Parameters.AddWithValue("@avatar", string.IsNullOrEmpty(e.Avatar) ? DBNull.Value : (object)e.Avatar);
                    command.Parameters.AddWithValue("@email", e.Email);
                    command.Parameters.AddWithValue("@clave", e.Clave);
                    command.Parameters.AddWithValue("@rol", e.Rol);
                    e.IdUsuario = Convert.ToInt32(command.ExecuteScalar());
                    return e.IdUsuario;
                }
            }
        }

        public int Baja(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"DELETE FROM usuarios WHERE IdUsuario = @id";
                using (var command = new MySqlCommand(query, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    return command.ExecuteNonQuery();
                }
            }
        }

    public int Modificacion(Usuario e)
    {
        using (var connection = GetConnection())
        {
            connection.Open();
            
            var query = @"UPDATE usuarios 
                        SET Nombre = @nombre, Apellido = @apellido, 
                            Avatar = NULLIF(@avatar, ''), Email = @email, 
                            Rol = @rol";

            if (!string.IsNullOrEmpty(e.Clave))
            {
                query += ", Clave = @clave";
            }

            query += " WHERE IdUsuario = @id";
            
            using (var command = new MySqlCommand(query, (MySqlConnection)connection))
            {
                command.Parameters.AddWithValue("@nombre", e.Nombre);
                command.Parameters.AddWithValue("@apellido", e.Apellido);
                command.Parameters.AddWithValue("@avatar", string.IsNullOrEmpty(e.Avatar) ? "" : e.Avatar);
                command.Parameters.AddWithValue("@email", e.Email);
                command.Parameters.AddWithValue("@rol", e.Rol);
                command.Parameters.AddWithValue("@id", e.IdUsuario);
                
                if (!string.IsNullOrEmpty(e.Clave))
                {
                    command.Parameters.AddWithValue("@clave", e.Clave);
                }

                return command.ExecuteNonQuery();
            }
        }
    }
        public Usuario ObtenerPorId(int id)
        {
            Usuario? e = null;
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"SELECT IdUsuario, Nombre, Apellido, Avatar, Email, Clave, Rol 
                              FROM usuarios 
                              WHERE IdUsuario = @id";
                using (var command = new MySqlCommand(query, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            e = new Usuario
                            {
                                IdUsuario = reader.GetInt32("IdUsuario"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Avatar = reader.IsDBNull(reader.GetOrdinal("Avatar")) ? null : reader.GetString("Avatar"),
                                Email = reader.GetString("Email"),
                                Clave = reader.GetString("Clave"),
                                Rol = reader.GetInt32("Rol")
                            };
                        }
                    }
                }
            }
            return e;
        }

        public Usuario ObtenerPorEmail(string email)
        {
            Usuario? e = null;
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"SELECT IdUsuario, Nombre, Apellido, Avatar, Email, Clave, Rol 
                              FROM usuarios 
                              WHERE Email = @Email";
                using (var command = new MySqlCommand(query, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@Email", email.Trim());
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            e = new Usuario
                            {
                                IdUsuario = reader.GetInt32("IdUsuario"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Avatar = reader.IsDBNull(reader.GetOrdinal("Avatar")) ? null : reader.GetString("Avatar"),
                                Email = reader.GetString("Email"),
                                Clave = reader.GetString("Clave").Trim(),
                                Rol = reader.GetInt32("Rol")
                            };
                        }
                    }
                }
            }
            return e;
        }

        public IList<Usuario> ObtenerTodos()
        {
            var usuarios = new List<Usuario>();
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"SELECT IdUsuario, Nombre, Apellido, Avatar, Email, Clave, Rol FROM usuarios";
                using (var command = new MySqlCommand(query, (MySqlConnection)connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            usuarios.Add(new Usuario
                            {
                                IdUsuario = reader.GetInt32("IdUsuario"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Avatar = reader.IsDBNull(reader.GetOrdinal("Avatar")) ? null : reader.GetString("Avatar"),
                                Email = reader.GetString("Email"),
                                Clave = reader.GetString("Clave"),
                                Rol = reader.GetInt32("Rol")
                            });
                        }
                    }
                }
            }
            return usuarios;
        }

        public IList<Usuario> ObtenerTodos(int pagina, int tamPagina)
        {
            var usuarios = new List<Usuario>();
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"SELECT IdUsuario, Nombre, Apellido, Avatar, Email, Clave, Rol 
                              FROM usuarios 
                              ORDER BY Apellido, Nombre 
                              LIMIT @tamPagina OFFSET @offset";
                using (var command = new MySqlCommand(query, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@tamPagina", tamPagina);
                    command.Parameters.AddWithValue("@offset", (pagina - 1) * tamPagina);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            usuarios.Add(new Usuario
                            {
                                IdUsuario = reader.GetInt32("IdUsuario"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Avatar = reader.IsDBNull(reader.GetOrdinal("Avatar")) ? null : reader.GetString("Avatar"),
                                Email = reader.GetString("Email"),
                                Clave = reader.GetString("Clave"),
                                Rol = reader.GetInt32("Rol")
                            });
                        }
                    }
                }
            }
            return usuarios;
        }
    }
}
