using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ventas_muebleria.Models
{
    public class Ventas
    {
        public Ventas()
        {
        }
     
        public ListadoVentasDetalle listaVentasDetalle { get; set; }
       
    }

    public class VentasDetalle
    {
       
        public int FolioVta { get; set; } 
        public int IdCliente { get; set; }
        public int IdArticulo { get; set; } 
        public DateTime Fecha { get; set; }
        public string TipoVenta { get; set; }   
        public int Cantidad { get; set; }
        public string IdClienteNombre { get; set; }
        public double Importe { get; set; }
      


        public string Activo { get; set; }
    }
    public class ListadoVentasDetalle : List<VentasDetalle> { }
}