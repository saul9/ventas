using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ventas_muebleria.Models
{
    public class Articulos
    {
        public Articulos()
        {


        }

        public ListadoArticulosVenta listaArticulosVenta { get; set; }
    }
    public class ArticulosVenta
    {


        public int IdArticulo { get; set; }
        public string Descripcion { get; set; }
        public string Modelo { get; set; }
        public double Precio { get; set; }
        public int Existencia { get; set; }
        public int Cantidad { get; set; }
        public double Importe { get; set; }
       

    }
    public class ListadoArticulosVenta : List<ArticulosVenta> { }
}