using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaAlcaraz.Models
{
    public interface IRepositorio<T> {


        int Alta(T p);
        int Baja(int d);
        int Modificacion(T p);

        IList<T> ObtenerTodos();

        T ObtenerPorId(int id);


    }
}