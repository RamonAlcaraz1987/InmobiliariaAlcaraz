using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

namespace InmobiliariaAlcaraz.Models
{
    public class RepositorioImagen : RepositorioBase, IRepositorioImagen
    {
        public RepositorioImagen(IConfiguration configuration) : base(configuration) { }

        public int Alta(Imagen imagen)
        {
            using var conn = new MySqlConnection(connectionString);
            const string sql = @"
                INSERT INTO imagenes (PortadaUrl, IdInmueble)
                VALUES (@url, @idInmueble);
                SELECT LAST_INSERT_ID();";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@url", imagen.Url);
            cmd.Parameters.AddWithValue("@idInmueble", imagen.IdInmueble);
            conn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public int Baja(int id)
        {
            using var conn = new MySqlConnection(connectionString);
            const string sql = "DELETE FROM imagenes WHERE IdImagen = @id;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        public int Modificacion(Imagen imagen)
        {
            using var conn = new MySqlConnection(connectionString);
            const string sql = @"
                UPDATE imagenes
                SET PortadaUrl = @url, IdInmueble = @idInmueble
                WHERE IdImagen = @idImagen;";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@url", imagen.Url);
            cmd.Parameters.AddWithValue("@idInmueble", imagen.IdInmueble);
            cmd.Parameters.AddWithValue("@idImagen", imagen.IdImagen);
            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        public IList<Imagen> ObtenerTodos()
        {
            var lista = new List<Imagen>();
            using var conn = new MySqlConnection(connectionString);
            const string sql = "SELECT IdImagen, PortadaUrl, IdInmueble FROM imagenes;";

            using var cmd = new MySqlCommand(sql, conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Imagen
                {
                    IdImagen = reader.GetInt32("IdImagen"),
                    Url = reader.GetString("PortadaUrl"),
                    IdInmueble = reader.GetInt32("IdInmueble")
                });
            }
            return lista;
        }

        public Imagen ObtenerPorId(int id)
        {
            using var conn = new MySqlConnection(connectionString);
            const string sql = "SELECT IdImagen, PortadaUrl, IdInmueble FROM imagenes WHERE IdImagen = @id;";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;
            return new Imagen
            {
                IdImagen = reader.GetInt32("IdImagen"),
                Url = reader.GetString("PortadaUrl"),
                IdInmueble = reader.GetInt32("IdInmueble")
            };
        }

        public IList<Imagen> ObtenerPorInmueble(int idInmueble)
        {
            var lista = new List<Imagen>();
            using var conn = new MySqlConnection(connectionString);
            const string sql = "SELECT IdImagen, PortadaUrl, IdInmueble FROM imagenes WHERE IdInmueble = @id;";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", idInmueble);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Imagen
                {
                    IdImagen = reader.GetInt32("IdImagen"),
                    Url = reader.GetString("PortadaUrl"),
                    IdInmueble = reader.GetInt32("IdInmueble")
                });
            }
            return lista;
        }
    }
}
