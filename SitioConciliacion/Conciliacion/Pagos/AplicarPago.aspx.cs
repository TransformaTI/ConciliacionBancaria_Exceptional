﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Conciliacion.RunTime.ReglasDeNegocio;
using Conciliacion.RunTime;
using System.Configuration;
using SeguridadCB.Public;
using Conciliacion.RunTime.DatosSQL;
//using RTGMGateway;
using System.Threading.Tasks;
using System.Diagnostics;

public partial class Conciliacion_Pagos_AplicarPago : System.Web.UI.Page
{
    Conciliacion.RunTime.App objApp = new Conciliacion.RunTime.App();
    #region "Propiedades Globales"
    private SeguridadCB.Public.Operaciones operaciones;
    private SeguridadCB.Public.Usuario usuario;
    private SeguridadCB.Public.Parametros parametros;
    public List<ReferenciaConciliadaPedido> listaReferenciaConciliadaPagos = new List<ReferenciaConciliadaPedido>();
    private List<ListaCombo> listFormasConciliacion = new List<ListaCombo>();
    private DataTable tblReferenciasAPagar;
    public List<ListaCombo> listCamposDestino = new List<ListaCombo>();
    public int corporativoConciliacion, añoConciliacion, folioConciliacion, folioExterno, sucursalConciliacion;
    public short mesConciliacion, tipoConciliacion;
    public short esEdificios;
    public MovimientoCajaDatos movimientoCajaAlta = null;
    //private RTGMGateway.SolicitudActualizarPedido solicitudActualizaPedido = new RTGMGateway.SolicitudActualizarPedido();
    //private RTGMGateway.RTGMActualizarPedido ActualizaPedido = new RTGMGateway.RTGMActualizarPedido();
    private string _URLGateway;
    //private List<RTGMCore.DireccionEntrega> listaDireccinEntrega = new List<RTGMCore.DireccionEntrega>();
    private bool validarPeticion = false;
    private List<int> listaClientesEnviados;
    private List<int> listaClientes = new List<int>();

    private int PagosSeleccionados
    {
        get
        {
            return Convert.ToInt32(Session["PagosSeleccionados"]);
        }

        set
        {
            Session["PagosSeleccionados"] = value;
        }
    }

    private int ClientePadre
    {
        get
        {
            return Convert.ToInt32(Session["ClientePadre"]);
        }

        set
        {
            Session["ClientePadre"] = value;
        }
    }


    private decimal MontoSeleccionado
    {
        get
        {
            return Convert.ToDecimal(Session["MontoSeleccionado"]);
        }

        set
        {
            Session["MontoSeleccionado"] = value;
        }
    }
    #endregion

    #region Eventos de la Forma



    protected override void OnPreInit(EventArgs e)
    {
        if (HttpContext.Current.Session["Operaciones"] == null)
            Response.Redirect("~/Acceso/Acceso.aspx", true);
        else
            operaciones = (SeguridadCB.Public.Operaciones)HttpContext.Current.Session["Operaciones"];
    }
    protected void Page_Load(object sender, EventArgs e)
    {

        if (IsPostBack)
        {
            if (Page.Request.Params["__EVENTTARGET"] == "miPostBack")
            {
                validarPeticion = false;
                //listaDireccinEntrega = ViewState["LISTAENTREGA"] as List<RTGMCore.DireccionEntrega>;
                listaClientes = ViewState["LISTACLIENTES"] as List<int>;
                //ViewState["TBL_REFCON_CANTREF"] = tblReferenciasAConciliar;
                string dat = Page.Request.Params["__EVENTARGUMENT"].ToString();
                if (dat == "1")
                {
                    //ObtieneNombreCliente(listaClientes);
                }
                else if (dat == "2")
                {
                    llenarListaEntrega();
                }
            }
        }
        objApp.ImplementadorMensajes.ContenedorActual = this;
        try
        {
            
            if (HttpContext.Current.Request.UrlReferrer != null)
            {
                if ((!HttpContext.Current.Request.UrlReferrer.AbsoluteUri.Contains("SitioConciliacion")) || (HttpContext.Current.Request.UrlReferrer.AbsoluteUri.Contains("Acceso.aspx")))
                {
                    HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
                    HttpContext.Current.Response.Cache.SetAllowResponseInBrowserHistory(false);
                }
            }
            if (!Page.IsPostBack)
            {
                usuario = (SeguridadCB.Public.Usuario)HttpContext.Current.Session["Usuario"];
                parametros = (SeguridadCB.Public.Parametros)HttpContext.Current.Session["Parametros"];

                string statusInicial;

                int visibleAreasComunes;
                StatusMovimientoCaja status;

                ClientePadre = -1;
                PagosSeleccionados = 0;
                MontoSeleccionado = 0;


                statusInicial = parametros.ValorParametro(30, "StatusInicialMovCaja");


                try
                {
                    visibleAreasComunes = int.Parse(parametros.ValorParametro(30, "PAGOAREASCOMUNES"));
                }
                catch 
                {
                    visibleAreasComunes = 0;
                }

                td1.Visible= visibleAreasComunes == 1;

                esEdificios = Convert.ToSByte(Request.QueryString["EsEdificios"]);

                if (statusInicial == "EMITIDO")
                {
                    status = StatusMovimientoCaja.Emitido;
                }
                else
                {
                    status = StatusMovimientoCaja.Validado;
                }
                
                if (esEdificios == 1) 
                {
                    status = StatusMovimientoCaja.Validado;
                }

                
                  
                
                corporativoConciliacion = Convert.ToInt32(Request.QueryString["Corporativo"]);
                sucursalConciliacion = Convert.ToInt16(Request.QueryString["Sucursal"]);
                añoConciliacion = Convert.ToInt32(Request.QueryString["Año"]);
                folioConciliacion = Convert.ToInt32(Request.QueryString["Folio"]);
                mesConciliacion = Convert.ToSByte(Request.QueryString["Mes"]);
                tipoConciliacion = Convert.ToSByte(Request.QueryString["TipoConciliacion"]);

                HttpContext.Current.Session["Pago_corporativoConciliacion"] = Convert.ToInt32(Request.QueryString["Corporativo"]);
                HttpContext.Current.Session["Pago_sucursalConciliacion"] = Convert.ToInt16(Request.QueryString["Sucursal"]);
                HttpContext.Current.Session["Pago_añoConciliacion"] = Convert.ToInt32(Request.QueryString["Año"]);
                HttpContext.Current.Session["Pago_folioConciliacion"] = Convert.ToInt32(Request.QueryString["Folio"]);
                HttpContext.Current.Session["Pago_mesConciliacion"] = Convert.ToSByte(Request.QueryString["Mes"]);
                HttpContext.Current.Session["Pago_tipoConciliacion"] = Convert.ToSByte(Request.QueryString["TipoConciliacion"]);





                LlenarBarraEstado();
                Carga_FormasConciliacion(tipoConciliacion);
                cargar_ComboCampoFiltroDestino(tipoConciliacion, rdbFiltrarEn.SelectedItem.Value);
                enlazarCampoFiltrar();
                activarControles(tipoCampoSeleccionado());
                //CARGAR LAS TRANSACCIONES CONCILIADAS POR EL CRITERIO DE AUTOCONCILIACIÓN
                Consulta_MovimientoCaja(corporativoConciliacion, sucursalConciliacion, añoConciliacion, mesConciliacion, folioConciliacion);
                Consulta_TransaccionesAPagar(corporativoConciliacion, sucursalConciliacion, añoConciliacion, mesConciliacion, folioConciliacion);
                GenerarTablaReferenciasAPagarPedidos();
                //LlenaGridViewReferenciasPagos();

                decimal totalTemp = 0;
                //foreach (ReferenciaConciliadaPedido objReferencia in listaReferenciaConciliadaPagos)
                //{
                //    totalTemp = totalTemp + objReferencia.MontoConciliado;
                //}
                HttpContext.Current.Session["TipoMovimientoCaja"]= Convert.ToInt32(Request.QueryString["tipoMovimientoCaja"]);

                movimientoCajaAlta = HttpContext.Current.Session["MovimientoCaja"] as MovimientoCajaDatos;
                foreach (Cobro objmcCobros in movimientoCajaAlta.ListaCobros)
                {
                    totalTemp = totalTemp + objmcCobros.Total;
                }
                movimientoCajaAlta.StatusAltaMC = status;
                movimientoCajaAlta.Total = totalTemp;
                movimientoCajaAlta.TipoMovimientoCaja = 3;
                //movimientoCajaAlta.SaldoAFavor = listaReferenciaConciliadaPagos.Sum(x => x.Diferencia);
                HttpContext.Current.Session["MovimientoCaja"] = movimientoCajaAlta;
                
                tdExportar.Attributes.Add("class", "iconoOpcion bg-color-grisClaro02");
                imgExportar.Enabled = false;
            }
        }
        catch (Exception ex) { objApp.ImplementadorMensajes.MostrarMensaje(ex.Message); }
    }

    #endregion

    #region Funciones Privadas
    //Cargar InfoConciliacion Actual
    public void cargarInfoConciliacionActual()
    {
        corporativoConciliacion = Convert.ToInt32(Request.QueryString["Corporativo"]);
        sucursalConciliacion = Convert.ToInt16(Request.QueryString["Sucursal"]);
        añoConciliacion = Convert.ToInt32(Request.QueryString["Año"]);
        folioConciliacion = Convert.ToInt32(Request.QueryString["Folio"]);
        mesConciliacion = Convert.ToSByte(Request.QueryString["Mes"]);
        tipoConciliacion = Convert.ToSByte(Request.QueryString["TipoConciliacion"]);
    }

    /// <summary>
    /// Leer Movimiento Caja Alta
    /// </summary>
    public void Consulta_MovimientoCaja(int corporativoConciliacion, int sucursalConciliacion, int añoConciliacion, short mesConciliacion, int folioConciliacion)
    {
        try
        {

            usuario = (SeguridadCB.Public.Usuario)HttpContext.Current.Session["Usuario"];
            string strUsuario = usuario.IdUsuario.Trim();
            //movimientoCajaAlta = HttpContext.Current.Session["MovimientoCaja"] as MovimientoCaja;

            parametros = (SeguridadCB.Public.Parametros)HttpContext.Current.Session["Parametros"];

            int transbanUsuario;

            try
            {
                transbanUsuario = int.Parse(parametros.ValorParametro(30, "TransBanPorUsuario"));
            }
            catch
            {
                transbanUsuario = 0;
            }

            if (transbanUsuario == 0)
            {
                strUsuario = "";
            }

HttpContext.Current.Session["UsuarioTransBan"] = strUsuario;

movimientoCajaAlta = objApp.Consultas.ConsultaMovimientoCajaAlta(
                corporativoConciliacion, 
                sucursalConciliacion, 
                añoConciliacion, 
                mesConciliacion, 
                folioConciliacion, 
                strUsuario);

            //short consecutivo = movimientoCajaAlta.Consecutivo;
            //int folio = movimientoCajaAlta.Folio;
            //short caja = movimientoCajaAlta.Caja;
            //string FOperacion = Convert.ToString(movimientoCajaAlta.FOperacion);
            HttpContext.Current.Session["MovimientoCaja"] = movimientoCajaAlta;
            HttpContext.Current.Session["LIST_REF_PAGAR"] = movimientoCajaAlta.ListaPedidos;
        }
        catch (Exception ex) { objApp.ImplementadorMensajes.MostrarMensaje("Error:\n" + ex.StackTrace + "\nMensaje: " + ex.Message); }

    }
    /// <summary>
    /// Carga las Formas de Conciliación
    /// </summary>
    public void Carga_FormasConciliacion(short tipoConciliacion)
    {
        try
        {
            listFormasConciliacion = objApp.Consultas.ConsultaFormaConciliacion(tipoConciliacion);
            this.ddlCriteriosConciliacion.DataSource = listFormasConciliacion;
            this.ddlCriteriosConciliacion.DataValueField = "Identificador";
            this.ddlCriteriosConciliacion.DataTextField = "Descripcion";
            this.ddlCriteriosConciliacion.DataBind();
            //this.ddlCriteriosConciliacion.Dispose();
        }
        catch (Exception ex)
        {
            objApp.ImplementadorMensajes.MostrarMensaje(ex.Message);
        }
    }
    /// <summary>
    /// Leer Pedidos --> Movimiento Caja Alta
    /// </summary>
    public void Consulta_TransaccionesAPagar(int corporativoC, int sucursalC, int añoC, short mesC, int folioC)
    {
        try
        {
            usuario = (SeguridadCB.Public.Usuario)HttpContext.Current.Session["Usuario"];
            string strUsuario = usuario.IdUsuario.Trim();
            //movimientoCajaAlta = HttpContext.Current.Session["MovimientoCaja"] as MovimientoCaja;

            string usuarioBusqueda = "";

            parametros = (SeguridadCB.Public.Parametros)HttpContext.Current.Session["Parametros"];

            int transbanUsuario;

            try
            {
                transbanUsuario = int.Parse(parametros.ValorParametro(30, "TransBanPorUsuario"));
            }
            catch
            {
                transbanUsuario = 0;
            }

            if (transbanUsuario == 1)
            {
                usuarioBusqueda = strUsuario;
            }

            listaReferenciaConciliadaPagos = objApp.Consultas.ConsultaPagosPorAplicar(corporativoC, sucursalC, añoC, mesC, folioC, usuarioBusqueda);
            listaReferenciaConciliadaPagos.ForEach(pedido => { pedido.Usuario = strUsuario; pedido.Portatil = false; });

            HttpContext.Current.Session["LIST_REF_PAGAR"] = listaReferenciaConciliadaPagos;
        }
        catch (Exception ex)
        {
            objApp.ImplementadorMensajes.MostrarMensaje("Error:\n" + ex.Message);
        }
    }

    protected void chkSeleccionado_CheckedChanged(object sender, System.EventArgs e)
    {
        decimal diferenciaDecimal;
        string diferencia;
        int clientePadreTemp;



        CheckBox rb = (CheckBox)sender;
        GridViewRow row = (GridViewRow)rb.NamingContainer;



        List<GridViewRow> filasCheck =new List<GridViewRow>
                (from GridViewRow r in grvPagos.Rows
                 where ((CheckBox)r.FindControl("chkSeleccionado")).Checked == true
                 select r);


        
        if (filasCheck.Count>1)
        {
            rb.Checked = false;
            ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "alert('Solo puede seleccionar un registro');", true);
            return;
        }

        diferencia = ((Label)row.FindControl("lblDiferencia")).Text;
        clientePadreTemp = int.Parse(((Label)row.FindControl("lblClientePadre")).Text);

        decimal.TryParse(diferencia, System.Globalization.NumberStyles.Currency, null, out diferenciaDecimal);


        if (diferenciaDecimal <= 0)
        {
            rb.Checked = false;
            ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "alert('No se pueden seleccionar registros sin saldo a favor');", true);
            return;
        }

        if (rb.Checked)
        {






            if (PagosSeleccionados==0)
            {
                PagosSeleccionados = PagosSeleccionados + 1;
                ClientePadre = clientePadreTemp;
                MontoSeleccionado = MontoSeleccionado + diferenciaDecimal;
            }
            else
            {
                if (clientePadreTemp==ClientePadre)
                {
                    PagosSeleccionados = PagosSeleccionados + 1;
                    MontoSeleccionado = MontoSeleccionado + diferenciaDecimal;
                }
                else
                {
                    rb.Checked = false;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "alert('No se pueden seleccionar registros sin saldo a favor o con Cliente Padre diferentes');", true);
                    return;
                }
            }            
        }
        else
        {
            PagosSeleccionados = PagosSeleccionados - 1;
            if (PagosSeleccionados ==0)
            {
                ClientePadre = -1;
                MontoSeleccionado = MontoSeleccionado - diferenciaDecimal;
            }
        }
    }

private string TipoCobroDescripcion(int tipoCobro)
    {
        if (tipoCobro == 10) //Transferencia
            return "Transferencia";
        else
        if (tipoCobro == 5) //Efectivo, ID: 
            return "Efectivo";
        else
        if (tipoCobro == 3) //c) Cheques, ID: 3
            return "Cheques";
        else
        if (tipoCobro == 6) //d) Tarjeta de crédito, ID: 
            return "Tarjeta de crédito";
        else
        if (tipoCobro == 19) //e) Tarjeta de débito, ID: 
            return "Tarjeta de débito";
        else
            return "";
    }

    /// <summary>
    /// Genera la tabla Referencias a Pagar Pedidos
    /// </summary>
    public void GenerarTablaReferenciasAPagarPedidos()
    {
        string _URLGateway = "";
        SeguridadCB.Public.Parametros parametros;
        parametros = (SeguridadCB.Public.Parametros)HttpContext.Current.Session["Parametros"];
        AppSettingsReader settings = new AppSettingsReader();
        _URLGateway = parametros.ValorParametro(Convert.ToSByte(settings.GetValue("Modulo", typeof(sbyte))), "URLGateway");
        try
        {
            //tblReferenciasAPagar = new DataTable("ReferenciasAConciliarPedidos");
            ////Campos Externos
            //tblReferenciasAPagar.Columns.Add("Secuencia", typeof(int));
            //tblReferenciasAPagar.Columns.Add("FolioExt", typeof(int));
            //tblReferenciasAPagar.Columns.Add("RFCTercero", typeof(string));
            //tblReferenciasAPagar.Columns.Add("Retiro", typeof(decimal));
            //tblReferenciasAPagar.Columns.Add("Referencia", typeof(string));
            //tblReferenciasAPagar.Columns.Add("NombreTercero", typeof(string));
            //tblReferenciasAPagar.Columns.Add("Deposito", typeof(decimal));
            //tblReferenciasAPagar.Columns.Add("Cheque", typeof(string));
            //tblReferenciasAPagar.Columns.Add("FMovimiento", typeof(DateTime));
            //tblReferenciasAPagar.Columns.Add("FOperacion", typeof(DateTime));
            //tblReferenciasAPagar.Columns.Add("MontoConciliado", typeof(decimal));
            //tblReferenciasAPagar.Columns.Add("Concepto", typeof(string));
            //tblReferenciasAPagar.Columns.Add("Descripcion", typeof(string));
            //tblReferenciasAPagar.Columns.Add("ClientePadre", typeof(int));
            //tblReferenciasAPagar.Columns.Add("Diferencia", typeof(decimal));
            //tblReferenciasAPagar.Columns.Add("StatusMovimiento", typeof(string));
            ////Campos Pedidos
            //tblReferenciasAPagar.Columns.Add("Pedido", typeof(int));
            //tblReferenciasAPagar.Columns.Add("PedidoReferencia", typeof(string));
            //tblReferenciasAPagar.Columns.Add("SeriePedido", typeof(string));
            //tblReferenciasAPagar.Columns.Add("RemisionPedido", typeof(string));
            //tblReferenciasAPagar.Columns.Add("FolioSat", typeof(string));
            //tblReferenciasAPagar.Columns.Add("SerieSat", typeof(string));
            //tblReferenciasAPagar.Columns.Add("AñoPed", typeof(int));
            //tblReferenciasAPagar.Columns.Add("Celula", typeof(int));
            //tblReferenciasAPagar.Columns.Add("Cliente", typeof(string));
            //tblReferenciasAPagar.Columns.Add("Nombre", typeof(string));
            //tblReferenciasAPagar.Columns.Add("Total", typeof(decimal));
            //tblReferenciasAPagar.Columns.Add("ConceptoPedido", typeof(string));
            //tblReferenciasAPagar.Columns.Add("TipoCobro", typeof(string));
            //tblReferenciasAPagar.Columns.Add("IdTipoCobro", typeof(int));
            //tblReferenciasAPagar.Columns.Add("FormaConciliacion", typeof(int));
            //tblReferenciasAPagar.Columns.Add("SucursalPedido", typeof(int));

            //List<int> listadistintos = new List<int>();//listaReferenciaConciliadaPedido.GroupBy(item => item.Cliente).Select(x=> x.f).ToList();//.GroupBy(x => x.Cliente).Select(c => c.First()).ToList();
            //listaClientesEnviados = new List<int>();
            //try
            //{
            //    listaDireccinEntrega = ViewState["LISTAENTREGA"] as List<RTGMCore.DireccionEntrega>;
            //    if (listaDireccinEntrega == null)
            //    {
            //        listaDireccinEntrega = new List<RTGMCore.DireccionEntrega>();
            //    }
            //}
            //catch (Exception)
            //{

            //}
            //foreach (var item in listaReferenciaConciliadaPagos)
            //{
            //    if (!listaDireccinEntrega.Exists(x => x.IDDireccionEntrega == item.Cliente))
            //    {
            //        if (!listadistintos.Exists(x => x == item.Cliente))
            //        {
            //            listadistintos.Add(item.Cliente);
            //        }
            //    }
            //}
            try
            {
                //if (listadistintos.Count > 0)
                //{
                //    HttpContext.Current.Session["CONCILIAR_PAGOS"] = listaReferenciaConciliadaPagos;
                //    validarPeticion = true;
                //    ObtieneNombreCliente(listadistintos);
                //    llenarListaEntrega();
                //}
                //else
                //{
                    llenarListaEntrega();
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //List<Cliente> lstClientes = new List<Cliente>();
            //if (_URLGateway != string.Empty)
            //    lstClientes = ConsultaCLienteCRMdt(listaReferenciaConciliadaPagos, _URLGateway);

            //foreach (ReferenciaConciliadaPedido rc in listaReferenciaConciliadaPagos)
            //{
            //    if (_URLGateway != string.Empty)
            //    {
            //        Cliente cliente;
            //        cliente = lstClientes.Find(x => x.NumCliente == rc.Cliente);
            //        if (cliente != null)
            //        {
            //            rc.Nombre = cliente.Nombre;
            //        }
            //    }


            //    tblReferenciasAPagar.Rows.Add(
            //           rc.Secuencia,
            //            rc.Folio,
            //            rc.RFCTercero,
            //            rc.Retiro,
            //            rc.Referencia,
            //            rc.NombreTercero,
            //            rc.Deposito,
            //            rc.Cheque,
            //            rc.FMovimiento,
            //            rc.FOperacion,
            //            rc.MontoConciliado,
            //            rc.Concepto,
            //            rc.Descripcion,
            //            rc.ClientePadre,
            //            rc.Diferencia,
            //            rc.StatusMovimiento,
            //            rc.Pedido,
            //            rc.PedidoReferencia,
            //            rc.SeriePedido,
            //            rc.RemisionPedido,
            //            rc.FolioSat,
            //            rc.SerieSat,
            //            rc.AñoPedido,
            //            rc.CelulaPedido,
            //            rc.Cliente,
            //            rc.Nombre,
            //            rc.Total,
            //            rc.ConceptoPedido,
            //            TipoCobroDescripcion(rc.TipoCobro),
            //            rc.TipoCobro,
            //            rc.FormaConciliacion,
            //            rc.SucursalPedido
            //            );
            //}
            //HttpContext.Current.Session["TAB_REF_PAGAR"] = tblReferenciasAPagar;
            //ViewState["TAB_REF_PAGAR"] = tblReferenciasAPagar;
        }
        catch (Exception e)
        {
            throw e;
        }
    }
    //private List<Cliente> ConsultaCLienteCRMdt(List<ReferenciaConciliadaPedido> listReferenciaPedidos, string URLGateway)
    //{
    //    List<Cliente> lstClientes = new List<Cliente>();
    //    List<int> listadistintos = new List<int>();
    //    try
    //    {
    //        foreach (var item in listReferenciaPedidos)
    //        {
    //            if (!listadistintos.Exists(x => x == item.Cliente))
    //            {
    //                listadistintos.Add(item.Cliente);
    //            }
    //        }
    //        AppSettingsReader settings = new AppSettingsReader();
    //        SeguridadCB.Public.Usuario usuario = (SeguridadCB.Public.Usuario)HttpContext.Current.Session["Usuario"];
    //        byte modulo = byte.Parse(settings.GetValue("Modulo", typeof(string)).ToString());
    //        string cadenaconexion = objApp.CadenaConexion;
    //        ParallelOptions options = new ParallelOptions();
    //        options.MaxDegreeOfParallelism = 3;
    //        Parallel.ForEach(listadistintos, options, (client) => {
    //            Cliente cliente;
    //            cliente = objApp.Cliente.CrearObjeto();
    //            cliente.NumCliente = client;
    //            cliente.Nombre = consultaClienteCRM(client, usuario, modulo, cadenaconexion, URLGateway);
    //            lstClientes.Add(cliente);
    //        });

    //        while (lstClientes.Count < listadistintos.Count)
    //        {

    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        throw;
    //    }
    //    return lstClientes;
    //}

    //private void ObtieneNombreCliente(List<int> listadistintos)
    //{
    //    RTGMGateway.RTGMGateway oGateway;
    //    RTGMGateway.SolicitudGateway oSolicitud;
    //    AppSettingsReader settings = new AppSettingsReader();
    //    string cadena = objApp.CadenaConexion;
    //    try
    //    {
    //        SeguridadCB.Public.Parametros parametros;
    //        parametros = (SeguridadCB.Public.Parametros)HttpContext.Current.Session["Parametros"];
    //        _URLGateway = parametros.ValorParametro(Convert.ToSByte(settings.GetValue("Modulo", typeof(sbyte))), "URLGateway").Trim();
    //        if (_URLGateway != string.Empty)
    //        { 
    //            SeguridadCB.Public.Usuario user = (SeguridadCB.Public.Usuario)Session["Usuario"];
    //            oGateway = new RTGMGateway.RTGMGateway(byte.Parse(settings.GetValue("Modulo", typeof(string)).ToString()), objApp.CadenaConexion);//,_URLGateway);
    //            oGateway.ListaCliente = listadistintos;
    //            oGateway.URLServicio = _URLGateway;//"http://192.168.1.21:88/GasMetropolitanoRuntimeService.svc";//URLGateway;
    //            oGateway.eListaEntregas += completarListaEntregas;
    //            oSolicitud = new RTGMGateway.SolicitudGateway();
    //            listaClientesEnviados = listadistintos;
    //            foreach (var item in listadistintos)
    //            {
    //                oSolicitud.IDCliente = item;
    //                oGateway.busquedaDireccionEntregaAsync(oSolicitud);
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        throw;
    //    }
    //}

    //public void completarListaEntregas(List<RTGMCore.DireccionEntrega> direccionEntregas)
    //{
    //    RTGMCore.DireccionEntrega direccionEntrega;
    //    RTGMCore.DireccionEntrega direccionEntregaTemp;
    //    bool errorConsulta = false;
    //    try
    //    {
    //        foreach (var item in direccionEntregas)
    //        {
    //            try
    //            {
    //                if (item != null)
    //                {
    //                    if (item.Message != null)
    //                    {
    //                        direccionEntrega = new RTGMCore.DireccionEntrega();
    //                        direccionEntrega.IDDireccionEntrega = item.IDDireccionEntrega;
    //                        direccionEntrega.Nombre = item.Message;
    //                        listaDireccinEntrega.Add(direccionEntrega);
    //                    }
    //                    else if (item.IDDireccionEntrega == -1)
    //                    {
    //                        errorConsulta = true;
    //                    }
    //                    else if (item.IDDireccionEntrega >= 0)
    //                    {
    //                        listaDireccinEntrega.Add(item);
    //                    }
    //                }
    //                else
    //                {
    //                    direccionEntrega = new RTGMCore.DireccionEntrega();
    //                    direccionEntrega.IDDireccionEntrega = item.IDDireccionEntrega;
    //                    direccionEntrega.Nombre = "No se encontró cliente";
    //                    listaDireccinEntrega.Add(direccionEntrega);
    //                }
    //            }
    //            catch (Exception ex)
    //            {
    //                direccionEntrega = new RTGMCore.DireccionEntrega();
    //                direccionEntrega.IDDireccionEntrega = item.IDDireccionEntrega;
    //                direccionEntrega.Nombre = ex.Message;
    //                listaDireccinEntrega.Add(direccionEntrega);
    //            }
    //        }
    //        if (validarPeticion && errorConsulta)
    //        {
    //            validarPeticion = false;
    //            listaClientes = new List<int>();
    //            foreach (var item in listaClientesEnviados)
    //            {
    //                direccionEntregaTemp = listaDireccinEntrega.FirstOrDefault(x => x.IDDireccionEntrega == item);
    //                if (direccionEntregaTemp == null)
    //                {
    //                    listaClientes.Add(item);
    //                }
    //            }
    //            ViewState["LISTAENTREGA"] = listaDireccinEntrega;
    //            ViewState["LISTACLIENTES"] = listaClientes;
    //            HttpContext.Current.Session["CONCILIAR_PAGOS"] = listaReferenciaConciliadaPagos;
    //            ScriptManager.RegisterStartupScript(this, typeof(Page), "Mensaje", "mensajeAsincrono(" + listaClientes.Count + ");", true);
    //        }
    //        else
    //        {
    //            llenarListaEntrega();
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        objApp.ImplementadorMensajes.MostrarMensaje("Error:\n" + ex.Message);
    //    }
    //}

    private void llenarListaEntrega()
    {
        try
        {
            SeguridadCB.Public.Parametros parametros;
            parametros = (SeguridadCB.Public.Parametros)HttpContext.Current.Session["Parametros"];
            AppSettingsReader settings = new AppSettingsReader();
            _URLGateway = parametros.ValorParametro(Convert.ToSByte(settings.GetValue("Modulo", typeof(sbyte))), "URLGateway");
            tblReferenciasAPagar = new DataTable("ReferenciasAConciliarPedidos");
            //Campos Externos
            tblReferenciasAPagar.Columns.Add("Secuencia", typeof(int));
            tblReferenciasAPagar.Columns.Add("FolioExt", typeof(int));
            tblReferenciasAPagar.Columns.Add("RFCTercero", typeof(string));
            tblReferenciasAPagar.Columns.Add("Retiro", typeof(decimal));
            tblReferenciasAPagar.Columns.Add("Referencia", typeof(string));
            tblReferenciasAPagar.Columns.Add("NombreTercero", typeof(string));
            tblReferenciasAPagar.Columns.Add("Deposito", typeof(decimal));
            tblReferenciasAPagar.Columns.Add("Cheque", typeof(string));
            tblReferenciasAPagar.Columns.Add("FMovimiento", typeof(DateTime));
            tblReferenciasAPagar.Columns.Add("FOperacion", typeof(DateTime));
            tblReferenciasAPagar.Columns.Add("MontoConciliado", typeof(decimal));
            tblReferenciasAPagar.Columns.Add("Concepto", typeof(string));
            tblReferenciasAPagar.Columns.Add("Descripcion", typeof(string));
            tblReferenciasAPagar.Columns.Add("ClientePadre", typeof(int));
            tblReferenciasAPagar.Columns.Add("Diferencia", typeof(decimal));
            tblReferenciasAPagar.Columns.Add("StatusMovimiento", typeof(string));
            //Campos Pedidos
            tblReferenciasAPagar.Columns.Add("Pedido", typeof(int));
            tblReferenciasAPagar.Columns.Add("PedidoReferencia", typeof(string));
            tblReferenciasAPagar.Columns.Add("SeriePedido", typeof(string));
            tblReferenciasAPagar.Columns.Add("RemisionPedido", typeof(string));
            tblReferenciasAPagar.Columns.Add("FolioSat", typeof(string));
            tblReferenciasAPagar.Columns.Add("SerieSat", typeof(string));
            tblReferenciasAPagar.Columns.Add("AñoPed", typeof(int));
            tblReferenciasAPagar.Columns.Add("Celula", typeof(int));
            tblReferenciasAPagar.Columns.Add("Cliente", typeof(string));
            tblReferenciasAPagar.Columns.Add("Nombre", typeof(string));
            tblReferenciasAPagar.Columns.Add("Total", typeof(decimal));
            tblReferenciasAPagar.Columns.Add("ConceptoPedido", typeof(string));
            tblReferenciasAPagar.Columns.Add("TipoCobro", typeof(string));
            tblReferenciasAPagar.Columns.Add("IdTipoCobro", typeof(int));
            tblReferenciasAPagar.Columns.Add("FormaConciliacion", typeof(int));
            tblReferenciasAPagar.Columns.Add("SucursalPedido", typeof(int));
            tblReferenciasAPagar.Columns.Add("AñoExterno", typeof(int));
            listaReferenciaConciliadaPagos = HttpContext.Current.Session["CONCILIAR_PAGOS"] as List<ReferenciaConciliadaPedido>;
            if (listaReferenciaConciliadaPagos == null)
            {
                listaReferenciaConciliadaPagos = new List<ReferenciaConciliadaPedido>();
            }
            //if (_URLGateway != string.Empty)
            //{
            //    foreach (var rc in listaReferenciaConciliadaPagos)
            //    {
            //        try
            //        {
            //            RTGMCore.DireccionEntrega cliente;
            //            cliente = listaDireccinEntrega.Find(x => x.IDDireccionEntrega == rc.Cliente);
            //            if (cliente != null)
            //            {
            //                rc.Nombre = cliente.Nombre;
            //            }
            //            else
            //            {
            //                rc.Nombre = "No encontrado";
            //            }
            //        }
            //        catch(Exception Ex)
            //        {
            //            rc.Nombre = Ex.Message;
            //        }
            //    }
            //}
            foreach (ReferenciaConciliadaPedido rc in listaReferenciaConciliadaPagos)
            {
                tblReferenciasAPagar.Rows.Add(
                       rc.Secuencia,
                        rc.Folio,
                        rc.RFCTercero,
                        rc.Retiro,
                        rc.Referencia,
                        rc.NombreTercero,
                        rc.Deposito,
                        rc.Cheque,
                        rc.FMovimiento,
                        rc.FOperacion,
                        rc.MontoConciliado,
                        rc.Concepto,
                        rc.Descripcion,
                        rc.ClientePadre,
                        rc.Diferencia,
                        rc.StatusMovimiento,
                        rc.Pedido,
                        rc.PedidoReferencia,
                        rc.SeriePedido,
                        rc.RemisionPedido,
                        rc.FolioSat,
                        rc.SerieSat,
                        rc.AñoPedido,
                        rc.CelulaPedido,
                        rc.Cliente,
                        rc.Nombre,
                        rc.Total,
                        rc.ConceptoPedido,
                        TipoCobroDescripcion(rc.TipoCobro),
                        rc.TipoCobro,
                        rc.FormaConciliacion,
                        rc.SucursalPedido,
                        rc.Año);
            }
            HttpContext.Current.Session["TAB_REF_PAGAR"] = tblReferenciasAPagar;
            ViewState["TAB_REF_PAGAR"] = tblReferenciasAPagar;
            //ViewState["LISTAENTREGA"] = listaDireccinEntrega;
            ViewState["LISTACLIENTES"] = listaClientes;
            LlenaGridViewReferenciasPagos();
        }
        catch (Exception ex)
        {
            objApp.ImplementadorMensajes.MostrarMensaje("Error:\n" + ex.Message);
        }
    }

    //public string consultaClienteCRM(int cliente, SeguridadCB.Public.Usuario user, byte modulo, string cadenaconexion, string URLGateway)
    //{
    //    RTGMGateway.RTGMGateway Gateway;
    //    RTGMGateway.SolicitudGateway Solicitud;
    //    RTGMCore.DireccionEntrega DireccionEntrega = new RTGMCore.DireccionEntrega();
    //    try
    //    {
    //        if (URLGateway != string.Empty)
    //        {
    //            //AppSettingsReader settings = new AppSettingsReader();
    //            //SeguridadCB.Public.Usuario usuario = (SeguridadCB.Public.Usuario)HttpContext.Current.Session["Usuario"];
    //            //byte modulo = byte.Parse(settings.GetValue("Modulo", typeof(string)).ToString());
    //            Gateway = new RTGMGateway.RTGMGateway(modulo, cadenaconexion);// objApp.CadenaConexion);
    //            Gateway.URLServicio = URLGateway;
    //            Solicitud = new RTGMGateway.SolicitudGateway();
    //            Solicitud.IDCliente = cliente;
    //            DireccionEntrega = Gateway.buscarDireccionEntrega(Solicitud);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //    if (DireccionEntrega == null ||
    //        DireccionEntrega.Nombre == null)
    //    {
    //        //DireccionEntrega.Message == null ||
    //        if (DireccionEntrega.Message.Contains("La consulta no produjo resultados con los parametros indicados."))
    //        {
    //            return "No encontrado";
    //        }
    //        else
    //        {
    //            return "";
    //        }
    //    }
    //    else
    //        return DireccionEntrega.Nombre.Trim();
    //}
    /// <summary>
    /// Llena el gridview con las conciliaciones antes leídas
    /// </summary>
    private void LlenaGridViewReferenciasPagos()//
    {
        try
        {
            DataTable tablaReferenacias = (DataTable)HttpContext.Current.Session["TAB_REF_PAGAR"];
            grvPagos.DataSource = tablaReferenacias;
            grvPagos.DataBind();
        }
        catch (Exception e)
        {
            objApp.ImplementadorMensajes.MostrarMensaje(e.Message);
        }

    }
    //Llenar Barra de Estado
    public void LlenarBarraEstado()
    {
        try
        {
            cConciliacion c = objApp.Consultas.ConsultaConciliacionDetalle(corporativoConciliacion, sucursalConciliacion, añoConciliacion, mesConciliacion, folioConciliacion);
            lblFolio.Text = c.Folio.ToString();
            lblBanco.Text = c.BancoStr;
            lblCuenta.Text = c.CuentaBancaria;
            lblGrupoCon.Text = c.GrupoConciliacionStr;
            lblSucursal.Text = c.SucursalDes;
            lblTipoCon.Text = c.TipoConciliacionStr;
            lblMesAño.Text = c.Mes + "/" + c.Año;
            lblConciliadasExt.Text = c.ConciliadasExternas.ToString();
            lblConciliadasInt.Text = c.ConciliadasInternas.ToString();
            lblStatusConciliacion.Text = c.StatusConciliacion;
            imgStatusConciliacion.ImageUrl = c.UbicacionIcono;
        }
        catch (Exception)
        {

            throw;
        }

    }
    /// <summary>
    /// Cargar campos de Filtro y Busqueda externo
    /// </summary>
    public void cargar_ComboCampoFiltroDestino(int tConciliacion, string filtrarEn)
    {
        try
        {
            listCamposDestino = filtrarEn.Equals("Externos")
                                            ? objApp.Consultas.ConsultaDestino()
                                            : objApp.Consultas.ConsultaDestinoPedido();


        }
        catch (Exception ex)
        {
            objApp.ImplementadorMensajes.MostrarMensaje(ex.Message);
        }

    }

    //Enlazar Campo a Filtrar/Busqueda
    public void enlazarCampoFiltrar()
    {
        try
        {
            this.ddlCampoFiltrar.DataSource = listCamposDestino;
            this.ddlCampoFiltrar.DataValueField = "Identificador";
            this.ddlCampoFiltrar.DataTextField = "Descripcion";
            this.ddlCampoFiltrar.DataBind();
            //this.ddlCampoFiltrar.Dispose();
        }
        catch (Exception ex)
        {
            objApp.ImplementadorMensajes.MostrarMensaje("Error:\n" + ex.Message);
        }
    }

    /// <summary>
    /// Activar los textbox segun sea el tipo de campa seleccionado en el filtro
    /// </summary>
    public void activarControles(string tipoCampo)
    {
        switch (tipoCampo)
        {
            case "Numero":
                // this.rfvValorExterno.ControlToValidate = "txtValorNumericoExterno";
                this.txtValorNumericoFiltro.Visible = true;
                this.txtValorCadenaFiltro.Visible = false;
                this.txtValorFechaFiltro.Visible = false;
                this.txtValorNumericoFiltro.Text = "0";
                this.txtValorCadenaFiltro.Text = String.Empty;
                this.txtValorFechaFiltro.Text = String.Empty;
                break;
            case "Fecha":
                //this.rfvValorExterno.ControlToValidate = "txtValorFechaExterno";
                this.txtValorFechaFiltro.Visible = true;
                this.txtValorNumericoFiltro.Visible = false;
                this.txtValorCadenaFiltro.Visible = false;
                this.txtValorFechaFiltro.Text = String.Empty;
                this.txtValorNumericoFiltro.Text = String.Empty;
                this.txtValorCadenaFiltro.Text = String.Empty;
                break;
            case "Cadena":
                //  this.rfvValorExterno.ControlToValidate = "txtValorCadenaExterno";
                this.txtValorCadenaFiltro.Visible = true;
                this.txtValorNumericoFiltro.Visible = false;
                this.txtValorFechaFiltro.Visible = false;
                this.txtValorCadenaFiltro.Text = String.Empty;
                this.txtValorNumericoFiltro.Text = String.Empty;
                this.txtValorFechaFiltro.Text = String.Empty;
                break;
        }

    }
    /// <summary>
    /// Lee el tipo de campo Seleccionado
    /// </summary>
    public string tipoCampoSeleccionado()
    {
        return listCamposDestino[ddlCampoFiltrar.SelectedIndex].Campo1;
    }
    /// <summary>
    /// Lee el valor del TextBox por tipo de Campo Seleccionado
    /// </summary>
    public string valorFiltro(string tipoCampo)
    {
        return tipoCampo.Equals("Cadena")
                   ? txtValorCadenaFiltro.Text
                   : (tipoCampo.Equals("Fecha") ? txtValorFechaFiltro.Text : txtValorNumericoFiltro.Text);
    }
    /// <summary>
    /// Ejecutar el Filtro por Valor y Campo
    /// </summary>
    private void FiltrarCampo(string valorFiltro, string filtroEn)
    {
        try
        {
            DataTable dt = (DataTable)HttpContext.Current.Session["TAB_REF_PAGAR"];

            DataView dv = new DataView(dt);
            string SearchExpression = String.Empty;
            if (!String.IsNullOrEmpty(valorFiltro))
            {
                SearchExpression = string.Format(
                    ddlOperacion.SelectedItem.Value == "LIKE"
                        ? "{0} {1} '%{2}%'"
                        : "{0} {1} '{2}'", ddlCampoFiltrar.SelectedItem.Text,
                    ddlOperacion.SelectedItem.Value, valorFiltro);
            }
            if (dv.Count <= 0) return;
            dv.RowFilter = SearchExpression;
            ViewState["TAB_REF_PAGAR"] = dv.ToTable();
            grvPagos.DataSource = ViewState["TAB_REF_PAGAR"] as DataTable;
            grvPagos.DataBind();
        }
        catch (Exception ex)
        {
            objApp.ImplementadorMensajes.MostrarMensaje(ex.Message);
        }
    }
    /// <summary>
    /// Obtiene la direccion de ordebamiento ASC o DESC
    /// </summary>
    private string direccionOrdenarCadena(string columna)
    {
        string direccionOrden = "ASC";
        string expresionOrden = ViewState["ExpresionOrden"] as string;
        if (expresionOrden != null)
            if (expresionOrden == columna)
            {
                string direccionAnterior = ViewState["DireccionOrden"] as string;
                if ((direccionAnterior != null) && (direccionAnterior == "ASC"))
                    direccionOrden = "DESC";
            }

        ViewState["DireccionOrden"] = direccionOrden;
        ViewState["ExpresionOrden"] = columna;

        return direccionOrden;
    }
    /// <summary>
    /// Metodos Busqueda
    ///</summary>
    public string resaltarBusqueda(string entradaTexto)
    {
        if (!txtBuscar.Text.Equals(""))
        {
            string strBuscar = txtBuscar.Text;
            Regex RegExp = new Regex(strBuscar.Replace(" ", "|").Trim(),
                           RegexOptions.IgnoreCase);
            return RegExp.Replace(entradaTexto, pintarBusqueda);
        }
        return entradaTexto;
    }
    /// <summary>
    /// Metodo Pintar
    ///</summary>
    public string pintarBusqueda(Match m)
    {
        return "<span class=marcarBusqueda>" + m.Value + "</span>";
    }
    #endregion

    #region Funciones de las Formas
    protected void btnFiltrar_Click(object sender, ImageClickEventArgs e)
    {
        //Leer el tipoConciliacion URL
        tipoConciliacion = Convert.ToSByte(Request.QueryString["TipoConciliacion"]);
        cargar_ComboCampoFiltroDestino(tipoConciliacion, rdbFiltrarEn.SelectedItem.Value);

        FiltrarCampo(valorFiltro(tipoCampoSeleccionado()), rdbFiltrarEn.SelectedItem.Value);
    }
    protected void ddlCampoFiltrar_SelectedIndexChanged(object sender, EventArgs e)
    {
        //Leer el tipoConciliacion URL
        tipoConciliacion = Convert.ToSByte(Request.QueryString["TipoConciliacion"]);
        cargar_ComboCampoFiltroDestino(tipoConciliacion, rdbFiltrarEn.SelectedItem.Value);

        activarControles(tipoCampoSeleccionado());
    }
    protected void imgBuscar_Click(object sender, ImageClickEventArgs e)
    {
        txtBuscar.Text = String.Empty;
        mpeBuscar.Show();
    }
    protected void btnIrBuscar_Click(object sender, EventArgs e)
    {
        grvPagos.DataSource = ViewState["TAB_REF_PAGAR"] as DataTable;
        grvPagos.DataBind();
        mpeBuscar.Hide();
    }

    protected void Nueva_Ventana(string Pagina, string Titulo, int Ancho, int Alto, int X, int Y)
    {

        ScriptManager.RegisterClientScriptBlock(this.upAplicarPagos,
                                         upAplicarPagos.GetType(),
                                            "ventana",
                                            "ShowWindow('" + Pagina + "','" + Titulo + "'," + Ancho + "," + Alto + "," + X + "," + Y + ")",
                                            true);

    }

    public void lanzarReporteComprobanteDeCaja(MovimientoCaja mc)
    {
        AppSettingsReader settings = new AppSettingsReader();

        string strReporte = Server.MapPath("~/") + settings.GetValue("RutaComprobanteDeCaja", typeof(string));
        if (!File.Exists(strReporte)) return;
        try
        {
            string strServer = settings.GetValue("Servidor", typeof(string)).ToString();
            string strDatabase = settings.GetValue("Base", typeof(string)).ToString();
            usuario = (SeguridadCB.Public.Usuario)HttpContext.Current.Session["Usuario"];

            string strUsuario = usuario.IdUsuario.Trim();
            string strPW = usuario.ClaveDesencriptada;
            ArrayList Par = new ArrayList();

            Par.Add("@Consecutivo=" + mc.Consecutivo);
            Par.Add("@Folio=" + mc.Folio);
            Par.Add("@Caja=" + mc.Caja);
            Par.Add("@FOperacion=" + mc.FOperacion);

            ClaseReporte Reporte = new ClaseReporte(strReporte, Par, strServer, strDatabase, strUsuario, strPW);
            HttpContext.Current.Session["RepDoc"] = Reporte.RepDoc;
            HttpContext.Current.Session["ParametrosReporte"] = Par;
            Nueva_Ventana("../../Reporte/Reporte.aspx", "Carta", 0, 0, 0, 0);
            Reporte = null;
        }
        catch (Exception ex)
        {
            objApp.ImplementadorMensajes.MostrarMensaje("Error: " + ex.Message);
        }
    }

    public void lanzarReporteCobranza(int Cobranza)
    {
        AppSettingsReader settings = new AppSettingsReader();

        string strReporte = Server.MapPath("~/") + settings.GetValue("RutaCobranza", typeof(string));
        if (!File.Exists(strReporte)) return;
        try
        {
            string strServer = settings.GetValue("Servidor", typeof(string)).ToString();
            string strDatabase = settings.GetValue("Base", typeof(string)).ToString();
            usuario = (SeguridadCB.Public.Usuario)HttpContext.Current.Session["Usuario"];

            string strUsuario = usuario.IdUsuario.Trim();
            string strPW = usuario.ClaveDesencriptada;
            ArrayList Par = new ArrayList();

            Par.Add("@Cobranza=" + Cobranza);

            ClaseReporte Reporte = new ClaseReporte(strReporte, Par, strServer, strDatabase, strUsuario, strPW);
            HttpContext.Current.Session["RepDoc"] = Reporte.RepDoc;
            HttpContext.Current.Session["ParametrosReporte"] = Par;
            Nueva_Ventana("../../Reporte/ReporteAlternativo.aspx", "Carta", 0, 0, 0, 0);
            Reporte = null;
        }
        catch (Exception ex)
        {
            objApp.ImplementadorMensajes.MostrarMensaje("Error: " + ex.Message);
        }
    }



    protected void imgExportar_Click(object sender, ImageClickEventArgs e)
    {
        try
        {
            //lanzarReporteComprobanteDeCaja();
        }
        catch (Exception ex)
        {
            objApp.ImplementadorMensajes.MostrarMensaje(ex.StackTrace);
        }
    }
    protected void btnActualizar_Click(object sender, ImageClickEventArgs e)
    {
        try
        {
            //Leer la InfoActual Conciliacion
            cargarInfoConciliacionActual();
            Consulta_TransaccionesAPagar(corporativoConciliacion, sucursalConciliacion, añoConciliacion, mesConciliacion, folioConciliacion);
            GenerarTablaReferenciasAPagarPedidos();
            //LlenaGridViewReferenciasPagos();
        }
        catch (Exception ex)
        {
            objApp.ImplementadorMensajes.MostrarMensaje(ex.Message);
        }
    }

    private bool HaySaldoAFavor(List<Cobro> ListaCobros)
    {
        bool resultado = false;
        foreach(Cobro cobro in ListaCobros)
        {
            resultado = cobro.SaldoAFavor;
            if (cobro.SaldoAFavor == true)
                break;
        }
        return resultado;
    }

    //private void PreparaPedidoRTGM(ReferenciaConciliadaPedido objPedido)
    //{
    //    solicitudActualizaPedido.TipoActualizacion = RTGMCore.TipoActualizacion.Liquidacion;
    //    solicitudActualizaPedido.Fuente = RTGMCore.Fuente.Sigamet;
    //    solicitudActualizaPedido.IDEmpresa = 1;
    //    //solicitudActualizaPedido.Portatil = ? booleano;
    //    //solicitudActualizaPedido.Usuario = ? "ROPIMA";

    //    RTGMCore.Pedido pedido = new RTGMCore.Pedido();
    //    pedido.IDPedido = objPedido.Pedido;
    //    pedido.IDZona = objPedido.CelulaPedido;
    //    pedido.AnioPed = objPedido.AñoPedido;

    //    solicitudActualizaPedido.Pedidos.Add(pedido);
    //}

    protected void btnAreasComunes_Click(object sender, ImageClickEventArgs e)
    {
        CheckBox sel;
        string referencia;
        string pedidoReferencia;

        DataTable tablaReferencias = (DataTable)HttpContext.Current.Session["TAB_REF_PAGAR"];
        DataTable tablaReferenciasSeleccionadas = tablaReferencias.Clone();
        DataRow[] filas;






        //if (PagosSeleccionados == 0)
        //{
        //    ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "alert('Es necesario que seleccione un registro');", true);
        //    return;
        //}



        List<GridViewRow> filasCheck = new List<GridViewRow>
                (from GridViewRow r in grvPagos.Rows
                 where ((CheckBox)r.FindControl("chkSeleccionado")).Checked == true
                 select r);

        if (filasCheck.Count==0)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "alert('Es necesario que seleccione un registro');", true);
            return;
        }


        foreach (GridViewRow fila in filasCheck)
        {                    
            referencia= ((Label)fila.FindControl("lblReferencia")).Text;
            pedidoReferencia = ((Label)fila.FindControl("lblPedidoReferencia")).Text;

            filas = tablaReferencias.Select("referencia = '"+referencia+"' and pedidoReferencia = '"+pedidoReferencia+"'");

            foreach (DataRow filaTabla in filas)
            {
                tablaReferenciasSeleccionadas.ImportRow(filaTabla);
            }                     
        }
    

        Conexion conexion = new Conexion();
        wuAreascomunes.inicializa(ClientePadre, MontoSeleccionado);
      //  wuAreascomunes.inicializa(8885, 1000);
        wuAreascomunes.TablaPagos = tablaReferenciasSeleccionadas;


        wuAreascomunes.CorporativoConciliacion = Convert.ToInt32(HttpContext.Current.Session["Pago_corporativoConciliacion"]); 
        wuAreascomunes.SucursalConciliacion = Convert.ToInt16(HttpContext.Current.Session["Pago_sucursalConciliacion"]); 
        wuAreascomunes.AnioConciliacion = Convert.ToInt32(HttpContext.Current.Session["Pago_añoConciliacion"]);
        wuAreascomunes.MesConciliacion = Convert.ToSByte(HttpContext.Current.Session["Pago_mesConciliacion"]);
        wuAreascomunes.FolioConciliacion = Convert.ToInt32(HttpContext.Current.Session["Pago_folioConciliacion"]);
        wuAreascomunes.cargaDatos();
        mpeAreasComunes.Show();
    }

    protected void btnAplicarPagos_Click(object sender, ImageClickEventArgs e)
    {
        btnAplicarPagos.Enabled = false;
        Conexion conexion  = new Conexion();
        bool urlValida = false;
        bool guardoMovimientoCaja = false;
        string fuente="";
        int corporativoConciliacion = 0;
        Int16 sucursalConciliacion = 0;
        int añoConciliacion = 0;
        int folioConciliacion = 0;
        short mesConciliacion = 0;
        short tipoConciliacion = 0;

        try
        {
            Parametros p = Session["Parametros"] as Parametros;
            AppSettingsReader settings = new AppSettingsReader();
            byte modulo = Convert.ToByte(settings.GetValue("Modulo", typeof(string)));
            string valor = p.ValorParametro(modulo, "NumeroDocumentosTRANSBAN");

            movimientoCajaAlta = HttpContext.Current.Session["MovimientoCaja"] as MovimientoCajaDatos;

            corporativoConciliacion = Convert.ToInt32(Request.QueryString["Corporativo"]);
            sucursalConciliacion = Convert.ToInt16(Request.QueryString["Sucursal"]);
            añoConciliacion = Convert.ToInt32(Request.QueryString["Año"]);
            folioConciliacion = Convert.ToInt32(Request.QueryString["Folio"]);
            mesConciliacion = Convert.ToSByte(Request.QueryString["Mes"]);
            string usuariotemp = Convert.ToString(HttpContext.Current.Session["UsuarioTransBan"]);

            

            if (valor.Equals(""))
            {
                throw new Exception("Parámetro NumeroDocumentosTRANSBAN no dado de alta");
            }

            int MaxDocumentos = Convert.ToInt16(valor);
            TransBan objTransBan = new TransBan();
            cargarInfoConciliacionActual();


            if (movimientoCajaAlta.StatusAltaMC == StatusMovimientoCaja.Emitido)
            {
                movimientoCajaAlta.Status = "EMITIDO";
            }
            else
            {
                movimientoCajaAlta.Status = "VALIDADO";
            }

            List<MovimientoCaja> lstMovimientoCaja = objTransBan.ReorganizaTransban(movimientoCajaAlta, MaxDocumentos);

            _URLGateway = p.ValorParametro(modulo, "URLGateway");
            urlValida = ValidarURL(_URLGateway);

            try
            {
                fuente = p.ValorParametro(modulo, "FuenteCRM").Trim();
            }
            catch
            {

            }


            tipoConciliacion = Convert.ToSByte(Request.QueryString["TipoConciliacion"]);

            bool hayPagos = objApp.Consultas.ExistenPagosPorAplicar(corporativoConciliacion,
                sucursalConciliacion, añoConciliacion, mesConciliacion, folioConciliacion, usuariotemp);

            if (!hayPagos)
            {
                objApp.ImplementadorMensajes.MostrarMensaje("Pagos ya aplicados");
                return;
            }
            decimal totalTemp = 0;
            decimal safTemp = 0;
            movimientoCajaAlta = HttpContext.Current.Session["MovimientoCaja"] as MovimientoCajaDatos;
            foreach (Cobro objmcCobros in movimientoCajaAlta.ListaCobros)
            {
                totalTemp = totalTemp + objmcCobros.Total;
                safTemp = safTemp + objmcCobros.Saldo;
            }
            movimientoCajaAlta.SaldoAFavor = safTemp;
            movimientoCajaAlta.Total = totalTemp;

            conexion.AbrirConexion(true,true);

            Session["CONCILIAR_PAGOS"] = null;
            foreach (MovimientoCaja objMovimientoCaja in lstMovimientoCaja)
            {             
                
                guardoMovimientoCaja = objMovimientoCaja.Guardar(conexion, tipoConciliacion);

                //if (objMovimientoCaja.Guardar(conexion))
                //{
                if (guardoMovimientoCaja)
                {
                    corporativoConciliacion = Convert.ToInt32(Request.QueryString["Corporativo"]);
                    sucursalConciliacion = Convert.ToInt16(Request.QueryString["Sucursal"]);
                    añoConciliacion = Convert.ToInt32(Request.QueryString["Año"]);
                    folioConciliacion = Convert.ToInt32(Request.QueryString["Folio"]);
                    mesConciliacion = Convert.ToSByte(Request.QueryString["Mes"]);
                    tipoConciliacion = Convert.ToSByte(Request.QueryString["TipoConciliacion"]);

                    GenerarTablaReferenciasAPagarPedidos();
                    //LlenaGridViewReferenciasPagos();

                    int idCobranza = 0;

                    parametros = (SeguridadCB.Public.Parametros)HttpContext.Current.Session["Parametros"];
                    string aplicacobranza = parametros.ValorParametro(30, "AplicaCobranza");

                    List<ReferenciaConciliadaPedido> _listaReferenciaConciliadaPagos = objMovimientoCaja.ListaPedidos;

                    foreach (ReferenciaConciliadaPedido objPedido in _listaReferenciaConciliadaPagos)
                    {
                        objApp.Consultas.ActualizaStatusConciliacionPedido(
                                                    corporativoConciliacion,
                                                    sucursalConciliacion,
                                                    añoConciliacion,
                                                    folioConciliacion,
                                                    mesConciliacion,
                                                    objPedido.Pedido,
                                                    objPedido.CelulaPedido,
                                                    objPedido.AñoPedido,
                                                    conexion);
                    }

                    if (aplicacobranza == "1")
                    {
                        usuario = (SeguridadCB.Public.Usuario)HttpContext.Current.Session["Usuario"];
                        string strUsuario = usuario.IdUsuario.Trim();
                        Cobranza cobranza = objApp.Cobranza.CrearObjeto();
                        cobranza.FCobranza = DateTime.Now;
                        cobranza.UsuarioCaptura = strUsuario;
                        cobranza.ListaReferenciaConciliadaPedido = _listaReferenciaConciliadaPagos;
                        idCobranza = cobranza.GuardarProcesoCobranza(conexion);

                        Boolean HasBoveda = p.ValorParametro(modulo, "BovedaExiste").Equals("1");

                        RelacionCobranzaException rCobranzaE = null;
                        RelacionCobranza rCobranza;
                        try
                        {
                            rCobranza = objApp.RelCobranza.CrearObjeto(objMovimientoCaja, HasBoveda);
                            rCobranza.CadenaConexion = objApp.CadenaConexion;
                            rCobranzaE = rCobranza.CreaRelacionCobranza(conexion);
                            if (!rCobranzaE.DetalleExcepcion.VerificacionValida)
                            {
                                objApp.ImplementadorMensajes.MostrarMensaje("Error: " + rCobranzaE.DetalleExcepcion.Mensaje + ", Código: " + rCobranzaE.DetalleExcepcion.CodigoError);
                            }
                        }
                        catch (Exception ex)
                        {
                            rCobranzaE.DetalleExcepcion.CodigoError = 201;
                            rCobranzaE.DetalleExcepcion.Mensaje = rCobranzaE.DetalleExcepcion.Mensaje + " " + ex.Message;
                            rCobranzaE.DetalleExcepcion.VerificacionValida = false;
                            throw new Exception("Error: " + rCobranzaE.DetalleExcepcion.Mensaje + ", Código: " + rCobranzaE.DetalleExcepcion.CodigoError);
                        }
                        // lanzarReporteCobranza(idCobranza); quitar, ya no debe mostrarse por múltiple trasban
                    }

                    MovimientoCajaConciliacion objMCC = new MovimientoCajaConciliacionDatos(objMovimientoCaja.Caja, objMovimientoCaja.FOperacion, objMovimientoCaja.Consecutivo, objMovimientoCaja.Folio,
                         corporativoConciliacion, sucursalConciliacion, añoConciliacion, mesConciliacion, folioConciliacion, "ABIERTO", idCobranza,
                         new MensajesImplementacion());
                    objMCC.Guardar(conexion);

                    List<ReferenciaConciliadaPedido> ListaConciliados = (List<ReferenciaConciliadaPedido>)HttpContext.Current.Session["LIST_REF_PAGAR"];

                    if (HaySaldoAFavor(objMovimientoCaja.ListaCobros))
                    {
                        SaldoAFavor objSaldoAFavor = objApp.SaldoAFavor.CrearObjeto();

                        foreach (ReferenciaConciliadaPedido referencia in ListaConciliados)
                        {
                            // FK TablaDestinoDetalle
                            objSaldoAFavor.CorporativoExterno = referencia.Corporativo;
                            objSaldoAFavor.SucursalExterno = referencia.Sucursal;
                            objSaldoAFavor.AñoExterno = referencia.Año;
                            objSaldoAFavor.FolioExterno = referencia.Folio;
                            objSaldoAFavor.SecuenciaExterno = referencia.Secuencia;

                            objSaldoAFavor.AñoCobro = 0;
                            objSaldoAFavor.Cobro = 0;
                            foreach (Cobro icobro in objMovimientoCaja.ListaCobros)
                            {
                                foreach (ReferenciaConciliadaPedido iRefConPed in icobro.ListaPedidos)
                                {
                                    if (iRefConPed.Corporativo == referencia.Corporativo &&
                                       iRefConPed.Sucursal == referencia.Sucursal &&
                                       iRefConPed.Año == referencia.Año &&
                                       iRefConPed.Folio == referencia.Folio &&
                                       iRefConPed.Secuencia == referencia.Secuencia)
                                    {
                                        objSaldoAFavor.AñoCobro = icobro.AñoCobro;
                                        objSaldoAFavor.Cobro = icobro.NumCobro;
                                        break;
                                    }
                                }
                            }

                            if (objSaldoAFavor.ExisteExterno(conexion))
                            {
                                objSaldoAFavor.RegistrarCobro(conexion);
                            }
                        }
                    }
                }
                else
                    throw new Exception("Error al aplicar el pago de los pedidos, por favor verifique.");
            }


            if (movimientoCajaAlta.StatusAltaMC == StatusMovimientoCaja.Validado)
            {
                FacturasComplemento objFacturasComplemento = objApp.FacturasComplemento;
                objFacturasComplemento.CorporativoConciliacion = corporativoConciliacion;
                objFacturasComplemento.SucursalConciliacion = sucursalConciliacion;
                objFacturasComplemento.AnioConciliacion = añoConciliacion;
                objFacturasComplemento.MesConciliacion = mesConciliacion;
                objFacturasComplemento.FolioConciliacion = folioConciliacion;
                objFacturasComplemento.Guardar(conexion);
            }


            if (conexion.Comando.Transaction != null)
            {
                conexion.Comando.Transaction.Commit();
            }
            objApp.ImplementadorMensajes.MostrarMensaje("El registro se guardó con éxito.");
        }
        catch (Exception ex)
        {
            objApp.ImplementadorMensajes.MostrarMensaje("Perdida de Conexion con el servidor, favor de intentar nuevamente saliendose y volviendo a entrar."+
                                                    "Detalles: \n\rClase: " +
                                                      this.GetType().Name + "\n\r Metodo :" +
                                                      ex.StackTrace + "\n\r Error :" +
                                                      ex.Message
                                                      );
            
            try
            {
                conexion.Comando.Transaction.Rollback();
            }
            catch
            {

            }
            
        }
    }


    //private bool EjecutaActualizaPedidoRTGM()
    //{
    //    bool exito = true; 
    //    //SeguridadCB.Public.Parametros parametros;
    //    //parametros = (SeguridadCB.Public.Parametros)HttpContext.Current.Session["Parametros"];
    //    AppSettingsReader settings = new AppSettingsReader();
    //    string _URLGateway = parametros.ValorParametro(Convert.ToSByte(settings.GetValue("Modulo", typeof(sbyte))), "URLGateway");
    //    if (_URLGateway != string.Empty)
    //    {
    //        List<RTGMCore.Pedido> lstPedidosRepuesta = new List<RTGMCore.Pedido>();
    //        ActualizaPedido.URLServicio = _URLGateway;
    //        lstPedidosRepuesta = ActualizaPedido.ActualizarPedido(solicitudActualizaPedido);
    //        if (lstPedidosRepuesta.Count > 0)
    //            if (lstPedidosRepuesta[0].Message != "NO HAY ERROR")
    //                exito = false;
    //    }
    //    return exito;
    //}

    protected void grvPagos_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes.Add("onmouseover", "this.className='bg-color-grisClaro02'");
            e.Row.Attributes.Add("onmouseout", "this.className='bg-color-blanco'");
        }
    }
    protected void grvPagos_Sorting(object sender, GridViewSortEventArgs e)
    {
        DataTable dtSortTable = ViewState["TAB_REF_PAGAR"] as DataTable;
        if (dtSortTable == null) return;
        string order = direccionOrdenarCadena(e.SortExpression);
        dtSortTable.DefaultView.Sort = e.SortExpression + " " + order;
        grvPagos.DataSource = dtSortTable;
        grvPagos.DataBind();
    }
    protected void rdbFiltrarEn_SelectedIndexChanged(object sender, EventArgs e)
    {
        //Leer el tipoConciliacion URL
        tipoConciliacion = Convert.ToSByte(Request.QueryString["TipoConciliacion"]);
        cargar_ComboCampoFiltroDestino(tipoConciliacion, rdbFiltrarEn.SelectedItem.Value);
        enlazarCampoFiltrar();
        activarControles(tipoCampoSeleccionado());
    }
    #endregion
    protected void grvPagos_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Image imgSM = e.Row.FindControl("imgStatusMovimiento") as Image;
            if (imgSM.AlternateText.Equals("PENDIENTE"))
                imgSM.CssClass = "icono bg-color-grisClaro02";
            else
            {
                imgSM.CssClass = "icono bg-color-verdeClaro";
                btnAplicarPagos.Enabled = false;
                tdAplicarPagos.Attributes.Add("class", "iconoOpcion bg-color-grisClaro02");
            }
        }
    }

    /// <summary>
    /// Verifíca que la URL es correcta
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    private bool ValidarURL(string url)
    {
        Uri uriValidada = null;
        return Uri.TryCreate(url, UriKind.Absolute, out uriValidada);
    }




    protected void grvPagos_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
}