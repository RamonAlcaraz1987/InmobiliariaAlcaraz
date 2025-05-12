using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InmobiliariaAlcaraz.Models;

namespace InmobiliariaAlcaraz.Models
{
    public interface IRepositorioPropietario : IRepositorio<Propietario>
    {
   

        IList<Propietario>ObtenerLista(int paginaNro, int tamPagina);
        int ObtenerCantidad();
        IList<Propietario> BuscarPorNombre(string nombre);

    }
}