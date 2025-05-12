using System;
using System.Collections.Generic;
using System.Linq;

using InmobiliariaAlcaraz.Models;
using System.Threading.Tasks;

namespace InmobiliariaAlcaraz.Models
{
	public interface IRepositorioUsuario : IRepositorio<Usuario>
	{
		Usuario ObtenerPorEmail(string email);
		IList<Usuario> ObtenerTodos(int pagina, int tamPagina);

	}
}
