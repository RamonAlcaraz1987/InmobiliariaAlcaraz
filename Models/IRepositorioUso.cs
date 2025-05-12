using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InmobiliariaAlcaraz.Models;


namespace InmobiliariaAlcaraz.Models
{
    public interface IRepositorioUso:IRepositorio<Uso>
    {
    
        IList<Uso> ObtenerLista(int paginaNro = 1, int tamPagina = 10);
      
    }
}