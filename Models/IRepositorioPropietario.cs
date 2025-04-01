using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InmobiliariaAlcaraz.Models;

namespace InmobiliariaAlcaraz.Models
{
    public interface IRepositorioPropietario : IRepositorio<Propietario>
    {
       // Propietario ObtenerPorEmail(string email);
       // IList<Propietario> BuscarPorNombre(string nombre);

        IList<Propietario>ObtenerLista(int paginaNro, int tamPagina);
    }
}