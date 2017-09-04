using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Kendo.Mvc.UI.Fluent;
using System.Data.SqlClient;
using ventas_muebleria.Models;
using System.Data;
using System.Configuration;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;
using Z.BulkOperations;

namespace ventas_muebleria.Controllers
{
    public class VentasController : Controller
    {
        public ListadoVentasDetalle listaVentas { get; set; }
        public ListadoVentasDetalle resultado { get; set; }
        public ListadoClientesVenta listaClientes { get; set; }
        public ListadoArticulosVenta listadoArticulosVenta { get; set; }

        // GET: Ventas
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ObtenerlistadoVentas([DataSourceRequest]DataSourceRequest request, int IdTipo, string Busqueda)
        {
            resultado = LlenarListado(IdTipo,Busqueda);
            if (resultado == null)
            {
                resultado = new ListadoVentasDetalle();
            }

            return Json(resultado.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult MostrarMantenimiento()
        {

            return View("RegistroVentas");

        }

        private ListadoVentasDetalle LlenarListado(int Tipo, string Busqueda)
        {
           
            StringBuilder consulta = new StringBuilder();
            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;
            ListadoVentasDetalle listavents = new ListadoVentasDetalle();

            try
            {
                conexion = Obtenerconexion(true);

                consulta.AppendLine("SELECT A.FOLIOVTA,A.IDCLIENTE,B.NOMBRE+' '+ ");
                consulta.AppendLine("B.APELLIDOPATERNO+' '+B.APELLIDOMATERNO NOMBRECOMPLETO, ");
                consulta.AppendLine("A.IDARTICULO,A.FECHA,A.ACTIVO,A.TOTAL ");
                consulta.AppendLine(" FROM VTA_VENTADETALLE A  ");
                consulta.AppendLine(" LEFT JOIN VTA_CLIENTES B ON  ");
                consulta.AppendLine(" B.ID = A.IDCLIENTE ");
                consulta.AppendLine(" LEFT JOIN VTA_ARTICULOS C ON  ");
                consulta.AppendLine(" C.ID = A.IDARTICULO ");
                consulta.AppendLine(" WHERE A.ACTIVO='Si' ");

                if (Tipo == 2)
                {
                    consulta.AppendLine(" AND A.FOLIOVTA = @BUSQUEDA ");
                }

                if (Tipo == 1)
                {
                    if (!string.IsNullOrEmpty(Busqueda))
                    {
                        int idcliente = 0;
                        string busqueda = string.Empty;

                        if (!int.TryParse(Busqueda, out idcliente))
                        {
                            consulta.AppendLine(" AND (B.NOMBRE LIKE @BUSQUEDA OR B.APELLIDOMATERNO  ");
                            consulta.AppendLine(" LIKE @BUSQUEDA OR B.APELLIDOPATERNO LIKE @BUSQUEDA) ");
                        }
                        else
                        {
                            consulta.AppendLine(" AND A.IDCLIENTE = @BUSQUEDA  ");
                        }
                    }

                }

                comando = obtenercomando(conexion, consulta);

             
                    if (!string.IsNullOrEmpty(Busqueda))
                    {
                        if (Tipo == 1)
                        {

                            int idcliente = 0;
                            string busqueda = string.Empty;

                            if (!int.TryParse(Busqueda, out idcliente))
                            {
                                comando.Parameters.Add("@BUSQUEDA", SqlDbType.VarChar);
                                comando.Parameters["@BUSQUEDA"].Value = Busqueda;

                            }
                            else
                            {
                                comando.Parameters.Add("@BUSQUEDA", SqlDbType.Int);
                                comando.Parameters["@BUSQUEDA"].Value = Busqueda;
                               
                            }
                        }
                        else if (Tipo==2)
                        {

                            comando.Parameters.Add("@BUSQUEDA", SqlDbType.Int);
                            comando.Parameters["@BUSQUEDA"].Value = Busqueda;
                         

                        }
                    }

                reader = comando.ExecuteReader();

                while (reader.Read())
                {
                    VentasDetalle Entidad = new VentasDetalle();
                    int i = 0;

                    Entidad.FolioVta = (reader[i] != DBNull.Value ? reader.GetInt32(i) : 0); i++;
                    Entidad.IdCliente = (reader[i] != DBNull.Value ? reader.GetInt32(i) : 0); i++;
                    Entidad.IdClienteNombre = (reader[i] != DBNull.Value ? reader.GetString(i) : string.Empty); i++;
                    Entidad.IdArticulo= (reader[i] != DBNull.Value ? reader.GetInt32(i) : 0); i++;
                    Entidad.Fecha = (reader[i] != DBNull.Value ? reader.GetDateTime(i) :DateTime.MinValue); i++;
                    Entidad.Activo = (reader[i] != DBNull.Value ? reader.GetString(i) : string.Empty); i++;
                    Entidad.Importe = (reader[i] != DBNull.Value ? Convert.ToDouble( reader.GetDecimal(i)) : 0); i++;
                    listavents.Add(Entidad);
                }


            }
            catch (Exception ex)
            {
                //ModelState.AddModelError("errors", ex.Message);
            }
            finally
            {

                if (conexion != null)
                {
                    if (conexion.State == ConnectionState.Open)
                    {
                        conexion.Close();
                    }

                    conexion.Dispose();
                    conexion = null;
                }
            }

            return listavents;
        }

        public SqlConnection Obtenerconexion(bool abierta)
        {
            SqlConnection conexion = new SqlConnection();

            conexion.ConnectionString = "workstation id=MUEBLERIABD.mssql.somee.com;packet size=4096;user id=saul9_1293_SQLLogin_1;pwd=sqouuhmhut;data source=MUEBLERIABD.mssql.somee.com;persist security info=False;initial catalog=MUEBLERIABD";

            if (abierta)
            {
                conexion.Open();
            }

            return conexion;
        }

        public void agregarparametro(SqlCommand comando ,string parametro,object valor)
        {
           Type tipodato = valor.GetType();

          
            comando.Parameters.Add(parametro,SqlDbType.Int );
            comando.Parameters["@BUSQUEDA"].Value = 2;


        }

        public SqlCommand obtenercomando(SqlConnection conexion, StringBuilder sentencia)
        {
            SqlCommand comando = new SqlCommand();
            comando.CommandText = sentencia.ToString();
            comando.CommandType = System.Data.CommandType.Text ;

            if (conexion.State != ConnectionState.Open)
            {
                conexion.Open();

            }
            comando.Connection = conexion;

            return comando;
        }

        public ActionResult ObtenerClientes([DataSourceRequest]DataSourceRequest request, string FiltroActual)
        {
            LlenarListaClientes(FiltroActual, request);

            if (listaClientes == null)
            {
                listaClientes = new ListadoClientesVenta();
            }

            return Json(listaClientes.Select(s => new { RFC = s.RFC, Cliente =  string.Concat(s.IdCliente.ToString("00"), " - ", s.NombreCompleto) }), JsonRequestBehavior.AllowGet);
        }

        public void LlenarListaClientes(string FiltroActual, [DataSourceRequest]DataSourceRequest request)
        {
            string textFilter = "";

            StringBuilder consulta = new StringBuilder();
            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;

            try
            {
                conexion = Obtenerconexion(true);

                consulta.AppendLine("SELECT ID,NOMBRE+' '+APELLIDOPATERNO+' '+APELLIDOMATERNO NOMBRECOMPLETO,RFC FROM VTA_CLIENTES   ");
                consulta.AppendLine("WHERE NOMBRE LIKE @FILTRO ");
                consulta.AppendLine(" OR APELLIDOMATERNO LIKE @FILTRO ");
                consulta.AppendLine(" OR APELLIDOPATERNO LIKE  @FILTRO ");

                comando = obtenercomando(conexion, consulta);


              if(FiltroActual!= "")
                {
                    comando.Parameters.Add("@FILTRO", SqlDbType.VarChar);
                    comando.Parameters["@FILTRO"].Value ="%"+ FiltroActual+"%";

                }

                reader = comando.ExecuteReader();
                listaClientes = new ListadoClientesVenta();
                while (reader.Read())
                {
                    ClientesVenta Entidad = new ClientesVenta();
                    int i = 0;

                    Entidad.IdCliente = (reader[i] != DBNull.Value ? reader.GetInt32(i) : 0); i++;
                    Entidad.NombreCompleto = (reader[i] != DBNull.Value ? reader.GetString(i) : string.Empty); i++;
                    Entidad.RFC = (reader[i] != DBNull.Value ? reader.GetString(i) : string.Empty); i++;                   
                    listaClientes.Add(Entidad);
                }

            }
            catch (Exception e)
            {
                
            }
        }

        public ActionResult ObtenerArticulos([DataSourceRequest]DataSourceRequest request, string FiltroActual)
        {
            LlenarListaArticulos(FiltroActual, request);

            if (listadoArticulosVenta == null)
            {
                listadoArticulosVenta = new ListadoArticulosVenta();
            }

            return Json(listadoArticulosVenta.Select(s => new { Existencia = s.Existencia,Precio= s.Precio,Modelo=s.Modelo, IdArticulo=s.IdArticulo, DescripcionArticulo=s.Descripcion, Articulo = string.Concat(s.IdArticulo.ToString("00"), " - ", s.Descripcion) }), JsonRequestBehavior.AllowGet);
        }

        public void LlenarListaArticulos(string FiltroActual, [DataSourceRequest]DataSourceRequest request)
        {

            ObtenerArticulolist(FiltroActual, 0);
   
        }


        public ListadoArticulosVenta ObtenerArticulolist(string FiltroActual, int IdArticulo)
        {
            string textFilter = "";

            StringBuilder consulta = new StringBuilder();
            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;

            try
            {
                conexion = Obtenerconexion(true);

                if (IdArticulo != 0)
                {
                    consulta.AppendLine(" SELECT * FROM VTA_ARTICULOS ");
                    consulta.AppendLine(" WHERE ID = @IDARTICULO ");
                }
                else
                {
                    consulta.AppendLine("SELECT * FROM VTA_ARTICULOS ");
                    consulta.AppendLine("WHERE  ");
                    consulta.AppendLine(" DESCRIPCION LIKE @FILTRO ");
                    consulta.AppendLine("OR MODELO LIKE @FILTRO ");
                }

                comando = obtenercomando(conexion, consulta);


                if (FiltroActual != "")
                {
                    comando.Parameters.Add("@FILTRO", SqlDbType.VarChar);
                    comando.Parameters["@FILTRO"].Value = "%" + FiltroActual + "%";

                }

                if(IdArticulo!=0)
                {

                    comando.Parameters.Add("@IDARTICULO", SqlDbType.Int);
                    comando.Parameters["@IDARTICULO"].Value = IdArticulo;
                }
              

                reader = comando.ExecuteReader();
                listadoArticulosVenta = new ListadoArticulosVenta();

                while (reader.Read())
                {
                    ArticulosVenta Entidad = new ArticulosVenta();
                    int i = 0;

                    Entidad.IdArticulo = (reader[i] != DBNull.Value ? reader.GetInt32(i) : 0); i++;
                    Entidad.Descripcion = (reader[i] != DBNull.Value ? reader.GetString(i) : string.Empty); i++;
                    Entidad.Modelo = (reader[i] != DBNull.Value ? reader.GetString(i) : string.Empty); i++;
                    Entidad.Precio = (reader[i] != DBNull.Value ? Convert.ToDouble(reader.GetDecimal(i)) : 0); i++;
                    Entidad.Existencia = (reader[i] != DBNull.Value ? reader.GetInt32(i) : 0); i++;

                    listadoArticulosVenta.Add(Entidad);
                }

            }
            catch (Exception e)
            {

            }

            return listadoArticulosVenta;
        }

        public ActionResult InsertarVenta (string Entidadetalles,int Existencia)
        {
            VentasDetalle entidaAUX = null;
            JObject resultado = null;
            try
            {
                if (Entidadetalles != string.Empty)
                {
                    entidaAUX = new JavaScriptSerializer().Deserialize<VentasDetalle>(Entidadetalles);

                    if (entidaAUX != null)
                    {
                        if (InsertarEntidad(entidaAUX))
                        {
                            ActualizarExistencia(entidaAUX.IdArticulo,Existencia);

                            resultado = new JObject(new JProperty("Estatus", "Correcto"));
                            return Json(resultado.ToString(), JsonRequestBehavior.AllowGet);
                        }
                    }

                }
            }
            catch(Exception e)
            {
                resultado = new JObject(new JProperty("Estatus", "error"));
                return Json(resultado.ToString(), JsonRequestBehavior.AllowGet);

            }
            resultado = new JObject(new JProperty("Estatus", "error"));
            return Json(resultado.ToString(), JsonRequestBehavior.AllowGet);
        }

        public bool InsertarEntidad(VentasDetalle entidadInsert)
        {
           

            StringBuilder sentencia = new StringBuilder();
            bool result = false;

            try
            {
                SqlConnection conexion = Obtenerconexion(true);
                sentencia.AppendLine(" INSERT INTO VTA_VENTADETALLE(IDARTICULO, IDCLIENTE,");
                sentencia.AppendLine(" CANTIDAD, TIPOVENTA, FECHA, TOTAL, ACTIVO) ");
                sentencia.AppendLine(" VALUES(@IDARTICULO,@IDCLIENTE,@CANTIDAD,@TIPOVENTA,@FECHA,@TOTAL,@ACTIVO )");
                SqlCommand comando = obtenercomando(conexion, sentencia);


                comando.Parameters.Add("@IDARTICULO", SqlDbType.Int);
                comando.Parameters["@IDARTICULO"].Value = entidadInsert.IdArticulo;

                comando.Parameters.Add("@IDCLIENTE", SqlDbType.Int);
                comando.Parameters["@IDCLIENTE"].Value = entidadInsert.IdCliente;

                comando.Parameters.Add("@CANTIDAD", SqlDbType.Int);
                comando.Parameters["@CANTIDAD"].Value = entidadInsert.Cantidad;

                comando.Parameters.Add("@TIPOVENTA", SqlDbType.VarChar);
                comando.Parameters["@TIPOVENTA"].Value = entidadInsert.TipoVenta;

                comando.Parameters.Add("@FECHA", SqlDbType.Date);
                comando.Parameters["@FECHA"].Value = DateTime.Now.Date;

                comando.Parameters.Add("@TOTAL", SqlDbType.Money);
                comando.Parameters["@TOTAL"].Value = entidadInsert.Importe;

                comando.Parameters.Add("@ACTIVO", SqlDbType.VarChar);
                comando.Parameters["@ACTIVO"].Value = entidadInsert.Activo;

              result =  comando.ExecuteNonQuery() > 0 ?true:false ;

            }
            catch
            {

            }
            return result;
        }

        public ActionResult ObtenerConsecutivo()
        {

            StringBuilder consult = new StringBuilder();
            SqlConnection conexion = null;
            SqlCommand comando = null;
            JObject result = null;
            int consecutivo = 0;

            consult.AppendLine("SELECT MAX(FOLIOVTA)+1 FROM VTA_VENTADETALLE");

            try
            {
                conexion = Obtenerconexion(true);
                comando = obtenercomando(conexion, consult);
                consecutivo = (int)comando.ExecuteScalar();

                string conse = consecutivo.ToString("0000");
                result = new JObject(new JProperty("Estatus", "Correcto"),
              new JProperty("Consecutivo", conse.ToString()));


                return Json(result.ToString(),JsonRequestBehavior.AllowGet);
            }
            catch
            {

            }
            result = new JObject( new JProperty("Estatus","error"),
                new JProperty("Consecutivo", consecutivo));

            return Json(result.ToString(), JsonRequestBehavior.AllowGet);

        }

        public ActionResult ObtenerPrecioArticulo(int CantidadIngresada,string Cantidad,int IdArticulo)
       {
            JObject resultado = null;
            double PrecioGenerado = 0;
            double ImporteGenerado = 0;
            double EngancheGenerado = 0;
            double BonificacionEnganche = 0;
            double Total =0;
            int ImportePrueb=0; 
            double PrecioDeContado = 0;
            double Totalapagar3meses = 0;
            double Totalapagar6meses = 0;
            double Totalapagar9meses = 0;
            double Totalapagar12meses = 0;
            double ImporteAbono3Meses = 0;
            double ImporteAbono6Meses = 0;
            double ImporteAbono9Meses = 0;
            double ImporteAbono12Meses = 0;
            double ImporteAhorro3 = 0;
            double ImporteAhorro6 = 0;
            double ImporteAhorro9 = 0;
            double ImporteAhorro12 = 0;


            try
            {
                int Cantidadaux = 0;
                int.TryParse(Cantidad, out Cantidadaux);

                ListadoArticulosVenta listAux = ObtenerArticulolist("", IdArticulo);

                if (listAux != null && listAux.Count==1)
                {
                    ArticulosVenta entidad = new ArticulosVenta();
                    entidad = listAux.ElementAt(0);

                   
                        ConfiguracionVenta EntidadConfiguracion = ObtenerConfiguraciones();

                    //generacion de precio venta eh importe 
                        PrecioGenerado = entidad.Precio*(1+(EntidadConfiguracion.TasaFinanciamiento*EntidadConfiguracion.PlazoMaximo)/100);
                       
                        ImporteGenerado = PrecioGenerado * CantidadIngresada;
                     
                        EngancheGenerado = (EntidadConfiguracion.PorcentajeEnganche / 100.00) * ImporteGenerado;

                    BonificacionEnganche = EngancheGenerado*((EntidadConfiguracion.TasaFinanciamiento*EntidadConfiguracion.PlazoMaximo) / 100);

                        Total = ImporteGenerado - EngancheGenerado - BonificacionEnganche;

                        PrecioDeContado = Total / (1+(EntidadConfiguracion.TasaFinanciamiento*EntidadConfiguracion.PlazoMaximo)/100);

                        Totalapagar3meses = PrecioDeContado*(1+(EntidadConfiguracion.TasaFinanciamiento*3)/100);
                        Totalapagar6meses = PrecioDeContado*(1+(EntidadConfiguracion.TasaFinanciamiento*6)/100);
                        Totalapagar9meses = PrecioDeContado*(1+(EntidadConfiguracion.TasaFinanciamiento*9)/100);
                        Totalapagar12meses = PrecioDeContado*(1+(EntidadConfiguracion.TasaFinanciamiento*12)/100);

                    ImporteAbono3Meses = Totalapagar3meses / 3;
                    ImporteAbono6Meses = Totalapagar6meses / 6;
                    ImporteAbono9Meses = Totalapagar9meses / 9;
                    ImporteAbono12Meses = Totalapagar12meses / 12;

                    ImporteAhorro3 = Total - Totalapagar3meses;
                    ImporteAhorro6 = Total - Totalapagar6meses;
                    ImporteAhorro9 = Total - Totalapagar9meses;
                    ImporteAhorro12 = Total - Totalapagar12meses;

                    int ExistenciaActualizar = Cantidadaux;

                      //  ActualizarExistencia(entidad.IdArticulo,ExistenciaActualizar);

                    resultado = new JObject(new JProperty("Estatus", "Correcto")
                        , new JProperty("PrecioGenerado", PrecioGenerado.ToString())
                        , new JProperty("ImporteGenerado", ImporteGenerado.ToString())
                        , new JProperty("EngancheGenerado", EngancheGenerado.ToString())
                        , new JProperty("BonificacionEnganche", BonificacionEnganche.ToString())
                        , new JProperty("TotalAdeudo", Total.ToString())
                        , new JProperty("PrecioContado", PrecioDeContado.ToString())
                        , new JProperty("Total3Meses", Totalapagar3meses.ToString())
                        , new JProperty("Total6Meses", Totalapagar6meses.ToString())
                        , new JProperty("Total9Meses", Totalapagar9meses.ToString())
                        , new JProperty("Total12Meses", Totalapagar12meses.ToString())
                        , new JProperty("ImporteAbono3Meses", ImporteAbono3Meses.ToString())
                        , new JProperty("ImporteAbono6Meses", ImporteAbono6Meses.ToString())
                        , new JProperty("ImporteAbono9Meses", ImporteAbono9Meses.ToString())
                        , new JProperty("ImporteAbono12Meses", ImporteAbono12Meses.ToString())
                        , new JProperty("ImporteAhorro3", ImporteAhorro3.ToString())
                        , new JProperty("ImporteAhorro6", ImporteAhorro6.ToString())
                        , new JProperty("ImporteAhorro9", ImporteAhorro9.ToString())
                        , new JProperty("ImporteAhorro12", ImporteAhorro12.ToString())
                        );
                            


                        return Json(resultado.ToString(),JsonRequestBehavior.AllowGet);
                    }

            }
            catch(Exception e)
            {
                throw;
            }

            resultado = new JObject(new JProperty("Estatus", "Error"));
            return Json(resultado.ToString(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult ActulizarExistenciaArticulo(int IdArticulo, int Existencia)
        {
            JObject resultado = null;
            try
            {
                ActualizarExistencia(IdArticulo, Existencia);
                resultado = new JObject(new JProperty("Estatus", "Correcto"));
                return Json(resultado.ToString(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                resultado = new JObject(new JProperty("Estatus", "Error"));
                return Json(resultado.ToString(), JsonRequestBehavior.AllowGet);
            }
            resultado = new JObject(new JProperty("Estatus", "Error"));
            return Json(resultado.ToString(), JsonRequestBehavior.AllowGet);
        }

        public void ActualizarExistencia(int IdArticulo,int Existencia)
        {

            StringBuilder Sentencia = new StringBuilder();
            SqlConnection conexion = null;
            SqlCommand comando = null;
            

            try
            {
                conexion = Obtenerconexion(true);
                if (IdArticulo != 0)
                {
                    Sentencia.AppendLine("  UPDATE VTA_ARTICULOS SET EXISTENCIA = @EXISTENCIA ");
                Sentencia.AppendLine(" WHERE ID = @IDARTICULO ");

                comando = obtenercomando(conexion, Sentencia);
                       
                comando.Parameters.Add("@EXISTENCIA", SqlDbType.Int);
                comando.Parameters["@EXISTENCIA"].Value = Existencia;

                
                 comando.Parameters.Add("@IDARTICULO", SqlDbType.Int);
                 comando.Parameters["@IDARTICULO"].Value = IdArticulo;
                }

                comando.ExecuteNonQuery();               
            }
            catch (Exception e)
            {

            }
        }
        public ConfiguracionVenta ObtenerConfiguraciones()
        {
            StringBuilder consulta = new StringBuilder();
            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;
            ConfiguracionVenta Entidad = null;

            try
            {
                conexion = Obtenerconexion(true);

                consulta.AppendLine(" SELECT * FROM VTA_CONFIGURACIONES ");

                comando = obtenercomando(conexion, consulta);

                reader = comando.ExecuteReader();
                
                while (reader.Read())
                {
                     Entidad = new ConfiguracionVenta();
                    int i = 0;

                    Entidad.TasaFinanciamiento = (reader[i] != DBNull.Value ? reader.GetDouble(i) : 0); i++;
                    Entidad.PorcentajeEnganche = (reader[i] != DBNull.Value ? reader.GetInt32(i) : 0); i++;
                    Entidad.PlazoMaximo = (reader[i] != DBNull.Value ? reader.GetInt32(i) : 0); i++;

                }

            }
            catch (Exception e)
            {

            }
            return Entidad;
        }
    }
}