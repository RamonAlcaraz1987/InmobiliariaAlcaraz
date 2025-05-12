using System.Collections.Generic;

namespace InmobiliariaAlcaraz.Models
{
    public interface IRepositorioInmueble : IRepositorio<Inmueble>
    {
        int Contar();
        IList<Inmueble> ObtenerLista(int paginaNro = 1, int tamPagina = 10);
        IList<Inmueble> ObtenerPorDisponibilidad(int disponible, int pagina, int cantidadPorPagina);
        int ContarPorDisponibilidad(int disponible);
        IList<Inmueble> ObtenerPorPropietario(int idPropietario, int pagina, int cantidadPorPagina);
        int ContarPorPropietario(int idPropietario);
        IList<Inmueble> ObtenerPorFechasDisponibles(DateTime fechaDesde, DateTime fechaHasta, int pagina = 1, int tamPagina = 10);
        int ContarPorFechasDisponibles(DateTime fechaDesde, DateTime fechaHasta);
            }
}