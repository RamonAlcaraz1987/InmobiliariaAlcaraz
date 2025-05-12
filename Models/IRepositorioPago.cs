using System.Collections.Generic;

namespace InmobiliariaAlcaraz.Models
{
    public interface IRepositorioPago : IRepositorio<Pago>
    {
       
       IList<Pago> ObtenerTodos(int paginaNro = 1, int tamPagina = 10);
        int Contar();
        IList<Pago> ObtenerPorContrato(int idContrato, int paginaNro = 1, int tamPagina = 10);
        int ContarPorContrato(int idContrato);
       
        

    }
}