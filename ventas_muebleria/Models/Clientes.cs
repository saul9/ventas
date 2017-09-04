using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ventas_muebleria.Models
{
    public class Clientes
    {
        public Clientes()
        {
        }


        public ListadoClientesVenta listaClientesVent { get; set; }
    }
    public class ClientesVenta
    {

        
        public int IdCliente { get; set; }
        public string RFC { get; set; }
        public string NombreCompleto { get; set; }      
      
    }
    public class ListadoClientesVenta : List<ClientesVenta> { }
}