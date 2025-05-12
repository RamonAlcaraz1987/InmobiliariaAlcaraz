using System.Collections.Generic;

namespace InmobiliariaAlcaraz.Models
{
    public interface IRepositorioContrato : IRepositorio<Contrato>
    {
        
        IList<Contrato> ObtenerLista(int paginaNro, int tamPagina);
        int ObtenerCantidad();
        int Bajan(int id, int idUsuarioFinalizacion);
        int CalcularPagosEsperados(DateTime fechaInicio, DateTime fechaFin);
        IList<Contrato> ObtenerPorInmueble(int idInmueble, int pagina, int tamPagina);
        public int ObtenerCantidadPorInmueble(int idInmueble);
        IList<Contrato> ObtenerContratosVigentes(DateTime fechaDesde, DateTime fechaHasta, int pagina = 1, int tamPagina = 10);
        int ObtenerCantidadContratosVigentes(DateTime fechaDesde, DateTime fechaHasta);
        bool TieneContratosSolapados(int idInmueble, DateTime fechaInicio, DateTime fechaFin, int? idContratoActual = null);
        IList<Contrato> ObtenerContratosPorPlazo(int plazoDias, int pagina = 1, int tamPagina = 10);
        int ObtenerCantidadContratosPorPlazo(int plazoDias);
        
        void AnularContratosVencidos();
        //IList<Contrato> ObtenerContratosActivosPorInmueble(int idInmueble);
        //bool TieneContratosSolapados(int idInmueble, DateTime fechaInicio, DateTime fechaFin, int? idContratoActual = null);
    }
}