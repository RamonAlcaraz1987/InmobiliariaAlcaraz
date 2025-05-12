using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using InmobiliariaAlcaraz.Models;

namespace InmobiliariaAlcaraz.Models
{
    public class RepositorioPropietario : RepositorioBase, IRepositorioPropietario
    {

        public RepositorioPropietario(IConfiguration configuration) : base(configuration) { }

        public int Alta(Propietario p)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"INSERT INTO propietarios (Nombre,Apellido,Dni,Email,Telefono,Direccion) VALUES (@nombre,@apellido,@dni,@email,@telefono,@direccion);SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(query, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@dni", p.Dni);
                    command.Parameters.AddWithValue("@apellido", p.Apellido);
                    command.Parameters.AddWithValue("@telefono", p.Telefono);
                    command.Parameters.AddWithValue("@direccion", p.Direccion);
                    command.Parameters.AddWithValue("@email", p.Email);
                    command.Parameters.AddWithValue("@nombre", p.Nombre);
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
            
        }
        public int Baja(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"DELETE FROM propietarios WHERE IdPropietario = @id";
                using (var command = new MySqlCommand(query, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    return command.ExecuteNonQuery();
                }
            }
        }

        public int Modificacion(Propietario p)  
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"UPDATE propietarios SET Dni = @dni,Apellido = @apellido,Telefono = @telefono,Direccion = @direccion,Email = @email,Nombre = @nombre WHERE IdPropietario = @idPropietario";
                using (var command = new MySqlCommand(query, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@dni", p.Dni);
                    command.Parameters.AddWithValue("@apellido", p.Apellido);
                    command.Parameters.AddWithValue("@telefono", p.Telefono);
                    command.Parameters.AddWithValue("@direccion", p.Direccion);
                    command.Parameters.AddWithValue("@email", p.Email);
                    command.Parameters.AddWithValue("@nombre", p.Nombre);
                    command.Parameters.AddWithValue("@idPropietario", p.IdPropietario);
                    return command.ExecuteNonQuery();
                }
            }
        } 

        public IList<Propietario> ObtenerTodos()
        {
            var propietarios = new List<Propietario>();
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"SELECT IdPropietario,Dni,Apellido,Telefono,Direccion,Email,Nombre FROM propietarios";
                using (var command = new MySqlCommand(query, (MySqlConnection)connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            propietarios.Add(new Propietario
                            {
                                IdPropietario = reader.GetInt32("IdPropietario"),
                                Dni = reader.GetString("Dni"),
                                Apellido = reader.GetString("Apellido"),
                                Telefono = reader.GetString("Telefono"),
                                Direccion = reader.GetString("Direccion"),
                                Email = reader.GetString("Email"),
                                Nombre = reader.GetString("Nombre")
                            });
                        }
                    }
                }
            }
            return propietarios;

        } 

        public Propietario ObtenerPorId(int id)
        {
            Propietario p = null;
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"SELECT IdPropietario,Dni,Apellido,Telefono,Direccion,Email,Nombre FROM propietarios WHERE IdPropietario = @id";
                using (var command = new MySqlCommand(query, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            p = new Propietario
                            {
                                IdPropietario = reader.GetInt32("IdPropietario"),
                                Dni = reader.GetString("Dni"),
                                Apellido = reader.GetString("Apellido"),
                                Telefono = reader.GetString("Telefono"),
                                Direccion = reader.GetString("Direccion"),
                                Email = reader.GetString("Email"),
                                Nombre = reader.GetString("Nombre")
                            };
                        }                       
                    }
                }
            }    
            return p;
        } 

        public IList<Propietario> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            IList<Propietario> res = new List<Propietario>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"
                    SELECT IdPropietario, Dni, Nombre, Apellido, Direccion, Telefono, Email
                    FROM Propietarios
                    ORDER BY IdPropietario
                    LIMIT {tamPagina} OFFSET {(paginaNro - 1) * tamPagina}
                ";
                
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Propietario p = new Propietario
                        {
                            IdPropietario = reader.GetInt32("IdPropietario"),
                              Dni = reader.GetString("Dni"),
                            Nombre = reader.GetString("Nombre"),
                            Apellido = reader.GetString("Apellido"),
                            Direccion = reader.GetString("Direccion"),
                            Telefono = reader.GetString("Telefono"),
                            Email = reader.GetString("Email"),

                        };
                        res.Add(p);
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
                var query = "SELECT COUNT(*) FROM propietarios";
                using (var command = new MySqlCommand(query, (MySqlConnection)connection))
                {
                    cantidad = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return cantidad;
        } 
        public IList<Propietario> BuscarPorNombre(string nombre)
            {
                List<Propietario> res = new List<Propietario>();
                Propietario? p = null;
                nombre = "%" + nombre + "%";
                using (var connection = new MySqlConnection(connectionString))
                {
                    string sql = @"SELECT IdPropietario, Nombre, Apellido, Dni, Telefono, Email, Direccion 
                                FROM Propietarios
                                WHERE Nombre LIKE @nombre OR Apellido LIKE @nombre";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.Add("@nombre", MySqlDbType.VarChar).Value = nombre;
                        command.CommandType = CommandType.Text;
                        connection.Open();
                        var reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            p = new Propietario
                            {
                                IdPropietario = reader.GetInt32("IdPropietario"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Dni = reader.GetString("Dni"),
                                Telefono = reader.GetString("Telefono"),
                                Email = reader.GetString("Email"),
                                Direccion = reader.GetString("Direccion"),
                            };
                            res.Add(p);
                        }
                        connection.Close();
                    }
                }
                return res;
            }
    }
}  


    
