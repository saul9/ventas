using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ventas_muebleria.Models
{
    public class Configuracion
    {
        public Configuracion()
        {
        }

       public ListadoConfiguracionVenta lisadoConfiguracion { get; set; }
    }
    public class ConfiguracionVenta
    {


        public int PlazoMaximo { get; set; }
        public double TasaFinanciamiento { get; set; }
        public int PorcentajeEnganche { get; set; }


    }
    public class ListadoConfiguracionVenta : List<ConfiguracionVenta> { }
}