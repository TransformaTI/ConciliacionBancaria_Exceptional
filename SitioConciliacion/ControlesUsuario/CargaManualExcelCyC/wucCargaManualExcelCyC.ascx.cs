﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.OleDb;
using System.IO;
using System.Data;
using System.Text;
using AjaxControlToolkit;
using Conciliacion.RunTime;
using Conciliacion.RunTime.ReglasDeNegocio;
using Conciliacion.RunTime.DatosSQL;
using System.ComponentModel;
using System.Configuration;

public partial class wucCargaManualExcelCyC : System.Web.UI.UserControl
{
    Conciliacion.RunTime.App objApp = new Conciliacion.RunTime.App();
    #region Propiedades
    private int totalRegistrosCargados;
    public int TotalRegistrosCargados
    {
        get { return totalRegistrosCargados; }
    }

    public int RegistrosCargados { get; set; }

    public List<ValidacionArchivosConciliacion.DetalleValidacion> DetalleProcesoDeCarga { get; set; }
    
    public int Anio
    {
        get
        {
            if (ViewState["anio"] == null)
                return 0;
            else
                return (int)ViewState["anio"];
        }
        set { ViewState["anio"] = value; }
    }

    public bool CargarAgregados
    {
        get
        {
            if (ViewState["CargarAgregados"] == null)
                return false;
            else
                return (bool)ViewState["CargarAgregados"];
        }
        set { ViewState["CargarAgregados"] = value; }
    }

    public Int64 ClienteReferencia
    {
        get
        {
            if (ViewState["clienteReferencia"] == null)
                return 0;
            else
                return (Int64)ViewState["clienteReferencia"];
        }
        set
        {
            ViewState["clienteReferencia"] = value;
            if (value == -1)
            {
                lblReferencia.Text = "El pago no tiene referencia";
                lblReferencia.CssClass = "etiqueta fg-color-naranja";
            }
            else
            {
                lblReferencia.Text = REFERENCIA + value.ToString();
            }
        }
    }
    
    public bool ConsultaPedido
    {
        get
        {
            if (ViewState["consultaPedido"] == null)
                return false;
            else
                return (bool)ViewState["consultaPedido"];
        }
        set { ViewState["consultaPedido"] = value; }
    }

    public int Corporativo
    {
        get
        {
            if (ViewState["corporativo"] == null)
                return int.MinValue;
            else
                return (int)ViewState["corporativo"];
        }
        set { ViewState["corporativo"] = value; }
    }
    
    public Int64 CuentaBancaria
    {
        get
        {
            if (ViewState["cuentaBancaria"] == null)
                return 0;
            else
                return (Int64)ViewState["cuentaBancaria"];
        }
        set { ViewState["cuentaBancaria"] = value; }
    }

    private DataTable DatosAConciliar
    {
        get
        {
            if (ViewState["datosAConciliar"] == null)
                return new DataTable();
            else
                return (DataTable)ViewState["datosAConciliar"];
        }
        set { ViewState["datosAConciliar"] = value; }
    }

    public bool DispersionAutomatica
    {
        get
        {
            if (ViewState["dispersionAutomatica"] == null)
                return false;
            else
                return (bool)ViewState["dispersionAutomatica"];
        }
        set { ViewState["dispersionAutomatica"] = value; }
    }

    public short FormaConciliacion
    {
        get
        {
            if (ViewState["FormaConciliacion"] == null)
                return 0;
            else
                return (short)ViewState["FormaConciliacion"];
        }
        set { ViewState["FormaConciliacion"] = value; }
    }
    
    public int Folio
    {
        get
        {
            if (ViewState["folio"] == null)
                return 0;
            else
                return (int)ViewState["folio"];
        }
        set { ViewState["folio"] = value; }
    }
    
    public sbyte Mes
    {
        get
        {
            if (ViewState["mes"] == null)
                return 0;
            else
                return (sbyte)ViewState["mes"];
        }
        set { ViewState["mes"] = value; }
    }
    
    public decimal MontoPago
    {
        get
        {
            if (ViewState["montoPago"] == null)
                return 0m;
            else
                return (decimal)ViewState["montoPago"];
        }
        set
        {
            ViewState["montoPago"] = value;
            lblMontoPago.Text = MONTO + String.Format("{0:C}", value);
        }
    }

    public bool MostrarBotonCancelar
    {
        get
        {
            if (ViewState["mostrarBotonCancelar"] == null)
                return false;
            else
                return (bool)ViewState["mostrarBotonCancelar"];
        }
        set { ViewState["mostrarBotonCancelar"] = value; }
    }

    public decimal SaldoAFavor
    {
        get
        {
            if (ViewState["saldoAFavor"] == null)
                return 0m;
            else
                return (decimal)ViewState["saldoAFavor"];
        }
        set
        {
            ViewState["saldoAFavor"] = value;
            lblSaldo.Visible = true;
            lblSaldo.Text = SALDO + String.Format("{0:C}", value);
        }
    }

    public Int16 Sucursal
    {
        get
        {
            if (ViewState["sucursal"] == null)
                return Int16.MinValue;
            else
                return (Int16)ViewState["sucursal"];
        }
        set { ViewState["sucursal"] = value; }
    }

    private List<ReferenciaNoConciliada> _referenciasPorConciliarExcel;
    public List<ReferenciaNoConciliada> ReferenciasPorConciliarExcel
    {
        get
        {
            RecuperaReferenciasNoConciliadas();
            return _referenciasPorConciliarExcel;
        }
    }

    private List<ReferenciaNoConciliadaPedido> _referenciasPorConciliarPedidoExcel;
    public List<ReferenciaNoConciliadaPedido> ReferenciasPorConciliarPedidoExcel
    {
        get
        {
            RecuperaReferenciasNoConciliadas();
            return _referenciasPorConciliarPedidoExcel;
        }
    }
    
    //private bool recuperoNoConciliados;
    public bool RecuperoNoConciliados
    {
    
        get
        {
            bool ValorRetorno = false;
            RecuperaReferenciasNoConciliadas();
            if((_referenciasPorConciliarExcel == null || _referenciasPorConciliarExcel.Count == 0) && (_referenciasPorConciliarPedidoExcel == null || _referenciasPorConciliarPedidoExcel.Count == 0))
            {
                ValorRetorno = false;
            }
            else
            {
                ValorRetorno = true;
            }
                return ValorRetorno;
        }
    }

    public short TipoConciliacion { get; set; }

    public ModalPopupExtender PopupContenedor { get; set; }

    public string URLGateway
    {
        get
        {
            if (ViewState["urlgateway"] == null)
                return "";
            else
                return (string)ViewState["urlgateway"];
        }
        set { ViewState["urlgateway"] = value; }
    }

    public byte Modulo
    {
        get
        {
            if (ViewState["modulo"] == null)
                return 0;
            else
                return (byte)ViewState["modulo"];
        }
        set { ViewState["modulo"] = value; }
    }

    public string CadenaConexion
    {
        get
        {
            if (ViewState["cadenaconexion"] == null)
                return "";
            else
                return (string)ViewState["cadenaconexion"];
        }
        set { ViewState["cadenaconexion"] = value; }
    }

    #endregion

    private const string ARCHIVO = "Archivo: ";
    private const string REGISTROS = "Total de registros a cargar: ";
    private const string MONTO = "Monto pago: ";
    private const string REFERENCIA = "Cliente: ";
    private const string SALDO = "Saldo a favor: ";

    private string _URLGateway;
//    private List<RTGMCore.DireccionEntrega> listaDireccinEntrega = new List<RTGMCore.DireccionEntrega>();
    private bool validarPeticion = false;
    private List<int> listaClientesEnviados;
    private List<int> listaClientes = new List<int>();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack)
        {
            string entrar = ViewState["Entrar"] as string;
            if (entrar == "Ok")
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
        }
        OcultarMensajes();

        if (MostrarBotonCancelar)
        {
            this.btnCancelar.Visible = true;
        }
    }

    protected void btnCancelar_Click(object sender, EventArgs e)
    {
        CargarAgregados = true;
        LimpiarGrid();
        if (PopupContenedor != null)
        {
            PopupContenedor.Hide();
        }
    }

    public void Cancelar(object sender, EventArgs e)
    {
        btnCancelar_Click(sender, e);
    }

    //public string consultaClienteCRM(int cliente)
    //{
    //    RTGMGateway.RTGMGateway Gateway;
    //    RTGMGateway.SolicitudGateway Solicitud;
    //    RTGMCore.DireccionEntrega DireccionEntrega = new RTGMCore.DireccionEntrega();
    //    try
    //    {
    //        if (_URLGateway != string.Empty)
    //        {
    //            AppSettingsReader settings = new AppSettingsReader();
    //            SeguridadCB.Public.Usuario usuario = (SeguridadCB.Public.Usuario)HttpContext.Current.Session["Usuario"];
    //            byte modulo = byte.Parse( settings.GetValue("Modulo", typeof(string)).ToString() );
    //            Gateway = new RTGMGateway.RTGMGateway(modulo, objApp.CadenaConexion);
    //            Gateway.URLServicio = _URLGateway;
    //            Solicitud = new RTGMGateway.SolicitudGateway();
    //            Solicitud.IDCliente = cliente;
    //            DireccionEntrega = Gateway.buscarDireccionEntrega(Solicitud);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //    if (DireccionEntrega != null && DireccionEntrega.Nombre != null)
    //        return DireccionEntrega.Nombre.Trim();
    //    else
    //        return "No encontrado";
    //}

    protected void btnSubirArchivo_Click(object sender, EventArgs e)
    {
        DataTable dtTabla = new DataTable();
        ValidacionArchivosConciliacion.ValidadorCyC Validador = new ValidacionArchivosConciliacion.ValidadorCyC();
        ValidacionArchivosConciliacion.IValidadorExcel iValidador = Validador;
        string sArchivo;
        string sRutaArchivo;
        string sExt;
        StringBuilder sbMensaje;
        string[] extensiones = { ".xlsx" };
        string[] MIME = {"application/vnd.ms-excel" ,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" };

        LimpiarGrid();
        sExt = Path.GetExtension(fupSeleccionar.FileName).ToLower();
        try
        {
            if (fupSeleccionar.HasFile)
            {
                if (extensiones.Contains(sExt))
                {
                    /*      Subir archivo       */
                    sRutaArchivo = System.IO.Path.GetFullPath(Server.MapPath("~/ControlesUsuario/CargaManualExcelCyC/Excel/"));
                    sArchivo = System.IO.Path.GetFullPath(Server.MapPath("~/ControlesUsuario/CargaManualExcelCyC/Excel/"))
                        + fupSeleccionar.FileName;
                    fupSeleccionar.SaveAs(sArchivo);

                    if (File.Exists(sArchivo))
                    {
                        iValidador.CuentaBancaria = this.CuentaBancaria;
                        //iValidador.DocumentoReferencia = 2;
                        iValidador.RutaArchivo = sRutaArchivo;
                        iValidador.NombreArchivo = Path.GetFileName(sArchivo);
                        iValidador.TipoMIME = (sExt == ".xls" ? MIME[0] : MIME[1]);
                        iValidador.URLGateway = this.URLGateway;
                        iValidador.Modulo = this.Modulo;
                        iValidador.CadenaConexion = this.CadenaConexion;
                        iValidador.Corporativo = this.Corporativo;
                        iValidador.Sucursal = this.Sucursal;

                        //if (iValidador.ArchivoValido(sRutaArchivo, Path.GetFileName(sArchivo)))
                        //{
                        dtTabla = iValidador.CargaArchivo(sArchivo);
                        DatosAConciliar = dtTabla;
                        DetalleProcesoDeCarga = iValidador.ValidacionCompleta();

                        if (DetalleProcesoDeCarga.Where(x => x.CodigoError != 0).Count() == 0)
                        {
                            //_URLGateway = "http://192.168.1.30:88/GasMetropolitanoRuntimeService.svc";
                            //if (_URLGateway != string.Empty)
                            //{
                            //    foreach (DataRow fila in dtTabla.Rows)
                            //    {
                            //        fila["Nombre"] = consultaClienteCRM(int.Parse(fila["cliente"].ToString()));
                            //    }
                            //}
                            grvDetalleConciliacionManual.Visible = true;
                            grvDetalleConciliacionManual.DataSource = dtTabla.DefaultView;
                            grvDetalleConciliacionManual.DataBind();
                            totalRegistrosCargados = grvDetalleConciliacionManual.Rows.Count;
                            lblArchivo.Text = ARCHIVO + sArchivo;
                            lblRegistros.Text = REGISTROS + totalRegistrosCargados.ToString();
                        }

                        sbMensaje = new StringBuilder();
                        foreach (ValidacionArchivosConciliacion.DetalleValidacion detalle in DetalleProcesoDeCarga)
                        {
                            if (!detalle.VerificacionValida)
                            {
                                if (detalle.CodigoError > 0)
                                {
                                    sbMensaje.Append(detalle.Mensaje + "<br />");
                                }
                                else
                                {
                                    sbMensaje.Append("El código de error y la validación no concuerdan: " + detalle.Mensaje + "\n");
                                }
                            }
                        }
                        if (sbMensaje.Length > 0)
                        {
                            DatosAConciliar = null;
                            lblMensajeError.Text = sbMensaje.ToString();
                            dvAlertaError.Visible = true;
                        }
                        else
                        {
                            dvMensajeExito.Visible = true;
                            RecuperaReferenciasNoConciliadas();

                            if (CargarAgregados == true)
                            {
                                CargarAgregados = false;
                            }

                            if (DispersionAutomatica)
                            {
                                DispersarPagos(dtTabla);
                            }
                        }
                        //}
                    }
                    else
                    {
                        objApp.ImplementadorMensajes.MostrarMensaje("El archivo no existe en el servidor.");
                    }
                }
                else
                {
                    objApp.ImplementadorMensajes.MostrarMensaje("El archivo a cargar debe ser de formato Excel, con extensión de archivo XLSX");
                }
            }// fupSeleccionar.HasFile
            else
            {
                objApp.ImplementadorMensajes.MostrarMensaje("El componente de carga no reconoce el archivo.");
            }
        }
        catch (Exception ex)
        {
            objApp.ImplementadorMensajes.MostrarMensaje(ex.ToString());
        }
    }

    private void llenarListaEntrega()
    {
        DataTable dtTablaPropuestos = ViewState["POR_CONCILIAR"] as DataTable;
        try
        {
            foreach (DataRow item in dtTablaPropuestos.Rows)
            {
                try
                {
                    //RTGMCore.DireccionEntrega cliente = listaDireccinEntrega.FirstOrDefault(x => x.IDDireccionEntrega == int.Parse(item["Cliente"].ToString()));
                    //if (cliente != null)
                    //{
                    //    item["Nombre"] = cliente.Nombre;
                    //}
                    //else
                    //{
                    //    item["Nombre"] = "No encontrado";
                    //}
                }
                catch (Exception Ex)
                {
                    item["Nombre"] = Ex.Message;
                }
            }


            grvDetalleConciliacionManual.Visible = false;
            grvPagosPropuestos.DataSource = dtTablaPropuestos;
            grvPagosPropuestos.DataBind();
            grvPagosPropuestos.Visible = true;
        }
        catch (Exception ex)
        {
            objApp.ImplementadorMensajes.MostrarMensaje("Error:\n" + ex.Message);
        }
        finally
        {
            ViewState["Entrar"] = "No";
        }
    }

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
    //            ScriptManager.RegisterStartupScript(this, typeof(Page), "Mensaje", " mensajeAsincrono(" + listaClientes.Count + ");", true);
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
    //        SeguridadCB.Public.Usuario user = (SeguridadCB.Public.Usuario)Session["Usuario"];
    //        oGateway = new RTGMGateway.RTGMGateway(byte.Parse(settings.GetValue("Modulo", typeof(string)).ToString()), objApp.CadenaConexion);//,_URLGateway);
    //        oGateway.ListaCliente = listadistintos;
    //        oGateway.URLServicio = _URLGateway;//"http://192.168.1.21:88/GasMetropolitanoRuntimeService.svc";//URLGateway;
    //        oGateway.eListaEntregas += completarListaEntregas;
    //        oSolicitud = new RTGMGateway.SolicitudGateway();
    //        listaClientesEnviados = listadistintos;
    //        foreach (var item in listadistintos)
    //        {
    //            oSolicitud.IDCliente = item;
    //            oGateway.busquedaDireccionEntregaAsync(oSolicitud);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        throw;
    //    }
    //}

    private void DispersarPagos(DataTable dtDatos)
    {
        if (dtDatos.Rows.Count > 0)
        {
            try
            {
                string sDocumento;
                decimal dMonto;
                DataTable dtTablaPropuestos;
                ReferenciaNoConciliadaPedido ReferenciaNoConciliada;
                PagoPropuesto pago;
                List<PagoPropuesto> pagosEntrada = new List<PagoPropuesto>();
                List<PagoPropuesto> pagosPropuestos = new List<PagoPropuesto>();
                DispersorPagoDatos dispersor = new DispersorPagoDatos();
                Conexion conexion = new Conexion();
                
                foreach (DataRow row in dtDatos.Rows)
                {
                    sDocumento = row["Documento"].ToString();
                    dMonto = Convert.ToDecimal(row["Monto"].ToString());
                    ReferenciaNoConciliada = objApp.Consultas.ConsultaPedidoReferenciaEspecifico(Corporativo, Sucursal, 1, 1, 1, 1, sDocumento);

                    pago = new PagoPropuesto();
                    pago.MontoPropuesto = dMonto;
                    pago.PedidoReferencia = sDocumento;
                    pago.SaldoPedido = ReferenciaNoConciliada.Total; /*RNCP.total*/
                    pago.ClienteReferencia = ReferenciaNoConciliada.Cliente.ToString(); /*RNCP.cliente*/
                    pagosEntrada.Add(pago);
                }

                try
                {
                    conexion.AbrirConexion(false);
                    dispersor.MontoTotalPago = MontoPago;
                    //dispersor.SaldoAFavor = 10000;      /*      TEMPORAL        */
                    dispersor.PagosPropuestos = pagosEntrada;
                    if (dispersor.ValidaClientes(pagosEntrada, ClienteReferencia.ToString(), conexion))
                    {
                        pagosPropuestos = dispersor.PagosPropuestos;
                        dtTablaPropuestos = ConvertirListaATabla<PagoPropuesto>(pagosPropuestos);
                        SaldoAFavor = dispersor.SaldoAFavor;

                        ViewState["POR_CONCILIAR"] = dtTablaPropuestos;
                        ViewState["Entrar"] = "Ok";
                        List<int> listadistintos = new List<int>();
                        listaClientesEnviados = new List<int>();
                        try
                        {
                            //listaDireccinEntrega = ViewState["LISTAENTREGA"] as List<RTGMCore.DireccionEntrega>;
                            //if (listaDireccinEntrega == null)
                            //{
                            //    listaDireccinEntrega = new List<RTGMCore.DireccionEntrega>();
                            //}
                        }
                        catch (Exception)
                        {

                        }
                        foreach (DataRow item in dtTablaPropuestos.Rows)
                        {
                            //if (!listaDireccinEntrega.Exists(x => x.IDDireccionEntrega == int.Parse(item["Cliente"].ToString())))
                            //{
                            //    if (!listadistintos.Exists(x => x == int.Parse(item["Cliente"].ToString())))
                            //    {
                            //        listadistintos.Add(int.Parse(item["Cliente"].ToString()));
                            //    }
                            //}
                        }
                        try
                        {
                            if (listadistintos.Count > 0)
                            {
                                validarPeticion = true;
                                //ObtieneNombreCliente(listadistintos);
                            }
                            else
                            {
                                llenarListaEntrega();
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        //grvDetalleConciliacionManual.Visible = false;
                        //grvPagosPropuestos.DataSource = dtTablaPropuestos;
                        //grvPagosPropuestos.DataBind();
                        //grvPagosPropuestos.Visible = true;

                        /*          Actualizar etiqueta de registros        */
                        lblRegistros.Text = REGISTROS + dtTablaPropuestos.Rows.Count.ToString();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conexion.CerrarConexion();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    private DataTable ConvertirListaATabla<T>(IList<T> list)
    {
        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
        DataTable table = new DataTable();
        foreach (PropertyDescriptor prop in properties)
            table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

        foreach (T item in list)
        {
            DataRow row = table.NewRow();
            foreach (PropertyDescriptor prop in properties)
                row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
            table.Rows.Add(row);
        }
        return table;
    }

    protected void btnAceptar_Click(object sender, EventArgs e)
    {
        //RecuperaReferenciasNoConciliadas();
        btnCancelar_Click(sender, e);
    }

    private void LimpiarGrid()
    {
        grvDetalleConciliacionManual.DataSource = null;
        grvDetalleConciliacionManual.DataBind();
        grvDetalleConciliacionManual.Visible = false;
        grvPagosPropuestos.DataSource = null;
        grvPagosPropuestos.DataBind();
        grvPagosPropuestos.Visible = false;
        lblArchivo.Text = ARCHIVO;
        lblRegistros.Text = REGISTROS;
    }

    private void OcultarMensajes()
    {
        dvAlertaError.Visible = false;
        dvMensajeExito.Visible = false;
    }

    private bool RecuperaReferenciasNoConciliadas()
    {
        string sDocumento;
        decimal dMonto;
        bool recupero = false;
        ReferenciaNoConciliada RefNoConciliada;
        ReferenciaNoConciliadaPedido ReferenciaNoConciliada;
        _referenciasPorConciliarExcel = new List<ReferenciaNoConciliada>();  /*      Inicializar campo de la propiedad     */
        _referenciasPorConciliarPedidoExcel = new List<ReferenciaNoConciliadaPedido>();

        try
        {
            if (DatosAConciliar.Rows.Count > 0)
            {
                //if (Convert.ToSByte(Request.QueryString["TipoConciliacion"]) == 2 || Convert.ToSByte(Request.QueryString["TipoConciliacion"]) == 6)
                //{
                if (ConsultaPedido)
                {
                    foreach (DataRow row in DatosAConciliar.Rows)
                    {
                        sDocumento = row["Documento"].ToString();
                        dMonto = Convert.ToDecimal(row["Monto"].ToString());

                        ReferenciaNoConciliadaPedido RefNoConciliadaPedido = objApp.ReferenciaNoConciliadaPedido.CrearObjeto();

                        ReferenciaNoConciliada = objApp.Consultas.ConsultaPedidoReferenciaEspecificoCliente(Corporativo, Sucursal, 1, 1, 1, 1, sDocumento);


                        RefNoConciliadaPedido.PedidoReferencia = sDocumento;
                        RefNoConciliadaPedido.Total = dMonto;
                        RefNoConciliadaPedido.AñoPedido = ReferenciaNoConciliada.AñoPedido;//Convert.ToInt32(sDocumento.Substring(0, 4));
                        RefNoConciliadaPedido.CelulaPedido = ReferenciaNoConciliada.CelulaPedido;//Convert.ToInt32(sDocumento.Substring(4, 1));
                        //int.TryParse(sDocumento.Substring(5, sDocumento.Length - 5), out Pedido);
                        RefNoConciliadaPedido.Pedido = ReferenciaNoConciliada.Pedido;

                        if (_referenciasPorConciliarExcel.Count > 0)
                        {
                            RefNoConciliadaPedido.Folio =
                                _referenciasPorConciliarExcel[_referenciasPorConciliarExcel.Count - 1].Folio;
                            RefNoConciliadaPedido.Secuencia =
                                _referenciasPorConciliarExcel[_referenciasPorConciliarExcel.Count - 1].Secuencia + 1;
                        }
                        else
                        {
                            RefNoConciliadaPedido.Folio = 1;
                            RefNoConciliadaPedido.Secuencia = 1;
                        }

                        //RefNoConciliadaPedido.FormaConciliacion = Convert.ToSByte(Request.QueryString["TipoConciliacion"]);
                        RefNoConciliadaPedido.FormaConciliacion = FormaConciliacion;
                        _referenciasPorConciliarPedidoExcel.Add(RefNoConciliadaPedido);

                        recupero = true;
                    }
                }
                else
                {
                    foreach (DataRow row in DatosAConciliar.Rows)
                    {
                        sDocumento = row["Documento"].ToString();
                        dMonto = Convert.ToDecimal(row["Monto"].ToString());

                        RefNoConciliada = objApp.ReferenciaNoConciliada.CrearObjeto();
                        RefNoConciliada.Referencia = sDocumento;
                        RefNoConciliada.Monto = dMonto;

                        if (_referenciasPorConciliarExcel.Count > 0)
                        {
                            RefNoConciliada.Folio =
                                _referenciasPorConciliarExcel[_referenciasPorConciliarExcel.Count - 1].Folio;
                            RefNoConciliada.Secuencia =
                                _referenciasPorConciliarExcel[_referenciasPorConciliarExcel.Count - 1].Secuencia + 1;
                        }
                        else
                        {
                            RefNoConciliada.Folio = 1;
                            RefNoConciliada.Secuencia = 1;
                        }

                        //RefNoConciliada.FormaConciliacion = Convert.ToSByte(Request.QueryString["TipoConciliacion"]);
                        RefNoConciliada.FormaConciliacion = FormaConciliacion;
                        _referenciasPorConciliarExcel.Add(RefNoConciliada);

                        recupero = true;
                    }
                }                
            }
        }
        catch(Exception ex)
        {
            objApp.ImplementadorMensajes.MostrarMensaje(ex.ToString());
        }
        HttpContext.Current.Session["_referenciasPorConciliarExcel"] = _referenciasPorConciliarExcel;
        this._referenciasPorConciliarExcel = _referenciasPorConciliarExcel;
        HttpContext.Current.Session["_referenciasPorConciliarPedidoExcel"] = _referenciasPorConciliarPedidoExcel;
        this._referenciasPorConciliarPedidoExcel = _referenciasPorConciliarPedidoExcel;
        return recupero;
    }// FIN Recupera Referencias No Conciliadas
    
}
