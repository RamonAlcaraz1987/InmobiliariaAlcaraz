
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaAlcaraz.Models
{
    public interface IRepositorioInquilino : IRepositorio<Inquilino>
    {
        // Puedes agregar métodos específicos para Inquilino si son necesarios
        IList<Inquilino> ObtenerLista(int paginaNro, int tamPagina);
    }
}