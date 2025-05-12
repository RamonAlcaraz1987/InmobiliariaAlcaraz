
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using InmobiliariaAlcaraz.Models;

namespace InmobiliariaAlcaraz.Models
{
    public class RepositorioInquilino : RepositorioBase, IRepositorioInquilino
    {
        public RepositorioInquilino(IConfiguration configuration) : base(configuration) { }

        public int Alta(Inquilino i)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"INSERT INTO inquilinos (Nombre, Apellido, Dni, Telefono, Email, Direccion, FechaNacimiento) 
                            VALUES (@nombre, @apellido, @dni, @telefono, @email, @direccion, @fechaNacimiento);
                            SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(query, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@nombre", i.Nombre);
                    command.Parameters.AddWithValue("@apellido", i.Apellido);
                    command.Parameters.AddWithValue("@dni", i.Dni);
                    command.Parameters.AddWithValue("@telefono", i.Telefono);
                    command.Parameters.AddWithValue("@email", i.Email);
                    command.Parameters.AddWithValue("@direccion", i.Direccion);
                    command.Parameters.AddWithValue("@fechaNacimiento", i.FechaNacimiento);
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public int Baja(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"DELETE FROM inquilinos WHERE IdInquilino = @id";
                using (var command = new MySqlCommand(query, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    return command.ExecuteNonQuery();
                }
            }
        }

        public int Modificacion(Inquilino i)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"UPDATE inquilinos SET 
                            Nombre = @nombre, 
                            Apellido = @apellido, 
                            Dni = @dni, 
                            Telefono = @telefono, 
                            Email = @email, 
                            Direccion = @direccion, 
                            FechaNacimiento = @fechaNacimiento 
                            WHERE IdInquilino = @id";
                using (var command = new MySqlCommand(query, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@nombre", i.Nombre);
                    command.Parameters.AddWithValue("@apellido", i.Apellido);
                    command.Parameters.AddWithValue("@dni", i.Dni);
                    command.Parameters.AddWithValue("@telefono", i.Telefono);
                    command.Parameters.AddWithValue("@email", i.Email);
                    command.Parameters.AddWithValue("@direccion", i.Direccion);
                    command.Parameters.AddWithValue("@fechaNacimiento", i.FechaNacimiento);
                    command.Parameters.AddWithValue("@id", i.IdInquilino);
                    return command.ExecuteNonQuery();
                }
            }
        }

        public IList<Inquilino> ObtenerTodos()
        {
            var inquilinos = new List<Inquilino>();
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"SELECT IdInquilino, Nombre, Apellido, Dni, Telefono, Email, Direccion, FechaNacimiento 
                            FROM inquilinos";
                using (var command = new MySqlCommand(query, (MySqlConnection)connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            inquilinos.Add(new Inquilino
                            {
                                IdInquilino = reader.GetInt32("IdInquilino"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Dni = reader.GetString("Dni"),
                                Telefono = reader.GetString("Telefono"),
                                Email = reader.GetString("Email"),
                                Direccion = reader.GetString("Direccion"),
                                FechaNacimiento = reader.GetDateTime("FechaNacimiento")
                            });
                        }
                    }
                }
            }
            return inquilinos;
        }

        public Inquilino ObtenerPorId(int id)
        {
            Inquilino i = null;
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"SELECT IdInquilino, Nombre, Apellido, Dni, Telefono, Email, Direccion, FechaNacimiento 
                            FROM inquilinos 
                            WHERE IdInquilino = @id";
                using (var command = new MySqlCommand(query, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            i = new Inquilino
                            {
                                IdInquilino = reader.GetInt32("IdInquilino"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Dni = reader.GetString("Dni"),
                                Telefono = reader.GetString("Telefono"),
                                Email = reader.GetString("Email"),
                                Direccion = reader.GetString("Direccion"),
                                FechaNacimiento = reader.GetDateTime("FechaNacimiento")
                            };
                        }
                    }
                }
            }
            return i;
        }

        public IList<Inquilino> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            IList<Inquilino> res = new List<Inquilino>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"
                    SELECT IdInquilino, Nombre, Apellido, Dni, Telefono, Email, Direccion, FechaNacimiento
                    FROM Inquilinos
                    ORDER BY IdInquilino
                    LIMIT {tamPagina} OFFSET {(paginaNro - 1) * tamPagina}
                ";
                
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Inquilino i = new Inquilino
                        {
                            IdInquilino = reader.GetInt32("IdInquilino"),
                            Nombre = reader.GetString("Nombre"),
                            Apellido = reader.GetString("Apellido"),
                            Dni = reader.GetString("Dni"),
                            Telefono = reader.GetString("Telefono"),
                            Email = reader.GetString("Email"),
                            Direccion = reader.GetString("Direccion"),
                            FechaNacimiento = reader.GetDateTime("FechaNacimiento")
                        };
                        res.Add(i);
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
                var query = "SELECT COUNT(*) FROM inquilinos";
                using (var command = new MySqlCommand(query, (MySqlConnection)connection))
                {
                    cantidad = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return cantidad;
        }
       
        
    }
}