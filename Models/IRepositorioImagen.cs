using System.Collections.Generic;

namespace InmobiliariaAlcaraz.Models
{
    public interface IRepositorioImagen : IRepositorio<Imagen>
    {
        IList<Imagen> ObtenerPorInmueble(int idInmueble);
    }
}