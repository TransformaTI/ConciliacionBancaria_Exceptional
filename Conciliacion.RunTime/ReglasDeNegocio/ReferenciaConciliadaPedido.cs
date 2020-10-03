using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conciliacion.RunTime.DatosSQL;

namespace Conciliacion.RunTime.ReglasDeNegocio
{
    public abstract class ReferenciaConciliadaPedido :  cReferencia
    {

        int añoconciliacion;
        short mesconciliacion;
        int folioconciliacion;
        
        int sucursalpedido;
        string sucursalpedidodes;
        int añopedido;
        int pedido;
        int remisionpedido;
        string seriepedido;
        int foliosat;
        string seriesat;
        string conceptopedido;
        decimal total;
        decimal totalsaldo;
        string statusmovimiento;

        Boolean selecciona;

        int cliente;
        string nombre;
        string pedidoreferencia;
        byte tipoproducto;
        decimal saldo;
        int clientePago;

        bool portatil;

        private int tipoCobro;
        private int tipoCobroAnterior;
        int clientePadre;
        String _iDPedidoCRM;

        private int folioext;
        private int secuenciaext;
        private string conceptoext;
        private decimal montoconciliado;

        private string referenciaexterno;
        private string descripcionexterno;

        private int añoexterno;
        private int numcontrato;
        private int idcontrato;
        private string serie;
        private int folio;
        private DateTime ffactura;
        private int añocargo;
        private int idcargo;
        private int foliofactura;
        private string seriefactura;
        private string tipocargo;

        #region Constructores

        public ReferenciaConciliadaPedido(int corporativo, int añoconciliacion, short mesconciliacion, int folioconciliacion, 
                                    int sucursalext, string sucursalextdes, int folioext, int secuenciaext, string conceptoext, decimal montoconciliado, decimal diferencia, short formaconciliacion, short statusconcepto, string statusconciliacion,DateTime foperacionext, DateTime fmovimientoext,
                                    string chequeexterno, string referenciaexterno, string descripcionexterno, string nombreterceroexterno, string rfcterceroexterno, decimal depositoexterno, decimal retiroexterno,
                                    int sucursalpedido,string sucursalpedidodes,int celulapedido,int añopedido, int pedido,int remisionpedido, string seriepedido,int foliosat,string seriesat, string conceptopedido, decimal total, string statusmovimiento,
                                    int cliente, string nombre,int añoexterno, MensajesImplementacion implementadorMensajes)
            : base(corporativo, sucursalext, sucursalextdes,añoexterno,folioext,secuenciaext,conceptoext,montoconciliado,diferencia,formaconciliacion,statusconcepto,statusconciliacion,foperacionext,fmovimientoext,
            chequeexterno,referenciaexterno,descripcionexterno,nombreterceroexterno,rfcterceroexterno,depositoexterno,retiroexterno,
            implementadorMensajes)
        {
            this.añoconciliacion = añoconciliacion;
            this.mesconciliacion=mesconciliacion;
            this.folioconciliacion=folioconciliacion;

            this.sucursalpedido = sucursalpedido;
            this.sucursalpedidodes=sucursalpedidodes;
            //this.celulapedido=celulapedido;
            this.AñoCargo=añopedido;
            this.IdCargo=pedido;
            this.remisionpedido=remisionpedido;
            this.seriepedido= seriepedido;
            this.foliosat=foliosat;
            this.seriesat=seriesat;
            this.total=total;
            this.conceptopedido = conceptopedido;
            this.statusmovimiento= statusmovimiento;
            this.selecciona = true;

            this.cliente = cliente;
            this.nombre = nombre;

        }

        public ReferenciaConciliadaPedido(int corporativo, int añoconciliacion, short mesconciliacion, int folioconciliacion,
                                    int sucursalext, string sucursalextdes, int folioext, int secuenciaext, string conceptoext, decimal montoconciliado, decimal diferencia, short formaconciliacion, short statusconcepto, string statusconciliacion,
                                    DateTime foperacionext, DateTime fmovimientoext,
                                    string chequeexterno, string referenciaexterno, string descripcionexterno, string nombreterceroexterno, string rfcterceroexterno, decimal depositoexterno, decimal retiroexterno,
                                    int sucursalpedido, string sucursalpedidodes, int celulapedido, int añocargo, int idcargo, int remisionpedido, string seriepedido, int foliosat, string seriesat, string conceptopedido, decimal total, string statusmovimiento,
                                    int cliente, string nombre, string pedidoreferencia, int añoexterno, 
                                    MensajesImplementacion implementadorMensajes)
            : base(corporativo, sucursalext, sucursalextdes, añoexterno, folioext, secuenciaext, conceptoext, montoconciliado, diferencia, formaconciliacion, statusconcepto, statusconciliacion, foperacionext, fmovimientoext,
            chequeexterno, referenciaexterno, descripcionexterno, nombreterceroexterno, rfcterceroexterno, depositoexterno, retiroexterno,
            implementadorMensajes)
        {
            this.añoconciliacion = añoconciliacion;
            this.mesconciliacion = mesconciliacion;
            this.folioconciliacion = folioconciliacion;

            this.sucursalpedido = sucursalpedido;
            this.sucursalpedidodes = sucursalpedidodes;
            //this.celulapedido = celulapedido;
            this.añocargo = añopedido;
            this.idcargo = idcargo;
            this.remisionpedido = remisionpedido;
            this.seriepedido = seriepedido;
            this.foliosat = foliosat;
            this.seriesat = seriesat;
            this.total = total;
            this.conceptopedido = conceptopedido;
            this.statusmovimiento = statusmovimiento;
            this.selecciona = true;

            this.cliente = cliente;
            this.nombre = nombre;
            this.pedidoreferencia = pedidoreferencia;
            
        }

        public ReferenciaConciliadaPedido(int corporativo, int añoconciliacion, short mesconciliacion, int folioconciliacion,
                                    int sucursalext, string sucursalextdes, int folioext, int secuenciaext, string conceptoext, decimal montoconciliado, decimal diferencia, short formaconciliacion, short statusconcepto, string statusconciliacion,
                                    DateTime foperacionext, DateTime fmovimientoext,
                                    string chequeexterno, string referenciaexterno, string descripcionexterno, string nombreterceroexterno, string rfcterceroexterno, decimal depositoexterno, decimal retiroexterno,
                                    int sucursalpedido, string sucursalpedidodes, int celulapedido, int añopedido, int pedido, int remisionpedido, string seriepedido, int foliosat, string seriesat, string conceptopedido, decimal total, string statusmovimiento,
                                    int cliente, string nombre, string pedidoreferencia, decimal saldo, int añoexterno, 
                                    MensajesImplementacion implementadorMensajes)
            : base(corporativo, sucursalext, sucursalextdes, añoexterno, folioext, secuenciaext, conceptoext, montoconciliado, diferencia, formaconciliacion, statusconcepto, statusconciliacion, foperacionext, fmovimientoext,
            chequeexterno, referenciaexterno, descripcionexterno, nombreterceroexterno, rfcterceroexterno, depositoexterno, retiroexterno,
            implementadorMensajes)
        {
            this.añoconciliacion = añoconciliacion;
            this.mesconciliacion = mesconciliacion;
            this.folioconciliacion = folioconciliacion;

            this.sucursalpedido = sucursalpedido;
            this.sucursalpedidodes = sucursalpedidodes;
            //this.celulapedido = celulapedido;
            this.AñoCargo = añopedido;
            this.IdCargo = pedido;
            this.remisionpedido = remisionpedido;
            this.seriepedido = seriepedido;
            this.foliosat = foliosat;
            this.seriesat = seriesat;
            this.total = total;
            this.conceptopedido = conceptopedido;
            this.statusmovimiento = statusmovimiento;
            this.selecciona = true;

            this.cliente = cliente;
            this.nombre = nombre;
            this.pedidoreferencia = pedidoreferencia;
            this.saldo = saldo;

            this.foliofactura = foliosat;
            this.seriefactura = seriesat;

        }

        public ReferenciaConciliadaPedido(int corporativo, int añoconciliacion, short mesconciliacion, int folioconciliacion,
                             int sucursalext, string sucursalextdes, int folioext, int secuenciaext, string conceptoext, decimal montoconciliado, decimal diferencia, short formaconciliacion, short statusconcepto, string statusconciliacion,
                             DateTime foperacionext, DateTime fmovimientoext,
                             string chequeexterno, string referenciaexterno, string descripcionexterno, string nombreterceroexterno, string rfcterceroexterno, decimal depositoexterno, decimal retiroexterno,
                             int sucursalpedido, string sucursalpedidodes, int celulapedido, int añopedido, int pedido, int remisionpedido, string seriepedido, int foliosat, string seriesat, string conceptopedido, decimal total, string statusmovimiento,
                             int cliente, string nombre, string pedidoreferencia, int añoexterno,
                             int idcontrato, string serie, int folio, DateTime ffactura, int anocargo, int idcargo,
                             MensajesImplementacion implementadorMensajes)
     : base(corporativo, sucursalext, sucursalextdes, añoexterno, folioext, secuenciaext, conceptoext, montoconciliado, diferencia, formaconciliacion, statusconcepto, statusconciliacion, foperacionext, fmovimientoext,
     chequeexterno, referenciaexterno, descripcionexterno, nombreterceroexterno, rfcterceroexterno, depositoexterno, retiroexterno,
     idcontrato, serie, folio, ffactura, anocargo, idcargo,
     implementadorMensajes)
        {
            this.añoexterno = añoexterno;
            this.folioext = folioext;
            this.secuenciaext = secuenciaext;
            this.conceptoext = conceptoext;
            this.referenciaexterno = referenciaexterno;
            this.descripcionexterno = descripcionexterno;

            this.añoconciliacion = añoconciliacion;
            this.mesconciliacion = mesconciliacion;
            this.folioconciliacion = folioconciliacion;

            this.sucursalpedido = sucursalpedido;
            this.sucursalpedidodes = sucursalpedidodes;
            //this.celulapedido = celulapedido;
            this.añopedido = añopedido;
            this.pedido = pedido;
            this.remisionpedido = remisionpedido;
            this.seriepedido = seriepedido;
            this.foliosat = foliosat;
            this.seriesat = seriesat;
            this.total = total;
            this.conceptopedido = conceptopedido;
            this.statusmovimiento = statusmovimiento;
            this.selecciona = true;

            this.cliente = cliente;
            this.nombre = nombre;
            this.pedidoreferencia = pedidoreferencia;
            this.idcontrato = idcontrato;
            this.serie = serie;
            this.folio = folio;
            this.ffactura = ffactura;
            this.añocargo = anocargo;
            this.idcargo = idcargo;

        }



        public ReferenciaConciliadaPedido(MensajesImplementacion implementadorMensajes)
            : base(implementadorMensajes)
        {
            this.añoconciliacion = 0;
            this.mesconciliacion = 0;
            this.folioconciliacion = 0;

            this.sucursalpedido = 0;
            this.sucursalpedidodes = "";
            //this.celulapedido = 0;
            this.añopedido = 0;
            this.pedido = 0;
            this.remisionpedido = 0;
            this.seriepedido = "";
            this.foliosat = 0;
            this.seriesat = "";
            this.conceptopedido = "";
            this.total = 0;
            this.statusmovimiento = "PENDIENTE";
            this.cliente = 0;
            this.nombre = "";
            this.pedidoreferencia = "";

            this.tipoproducto = 0;
            this.portatil = false;
        }
        
        #endregion

        #region Propiedades

        public string PedidoReferencia 
        {
            get { return pedidoreferencia; }
            set { pedidoreferencia = value; }
        }
        
        public string StatusMovimiento
        {
            get { return statusmovimiento; }
            set { statusmovimiento = value; }
        }

        public int AñoConciliacion
        {
            get { return añoconciliacion; }
            set { añoconciliacion = value; }
        }

        public short MesConciliacion
        {
            get { return mesconciliacion; }
            set { mesconciliacion = value; }
        }

        public int FolioConciliacion
        {
            get { return folioconciliacion; }
            set { folioconciliacion = value; }
        }

        public int SucursalPedido
        {
            get { return sucursalpedido; }
            set { sucursalpedido = value; }
        }

        public abstract void Guardar(Conexion conexion);

        public string SucursalPedidoDes
        {
            get { return sucursalpedidodes; }
            set { sucursalpedidodes = value; }
        }
        
        public int AñoCargo
        {
            get { return añocargo; }
            set { añocargo = value; }
        }

        public int IdCargo
        {
            get { return idcargo; }
            set { idcargo = value; }
        }
        public int IdContrato
        {
            get { return idcontrato; }
            set { idcontrato = value; }
        }
        public int NumContrato
        {
            get { return numcontrato; }
            set { numcontrato = value; }
        }
        public int FolioFactura
        {
            get { return foliofactura; }
            set { foliofactura = value; }
        }
        public string SerieFactura
        {
            get { return seriefactura; }
            set { seriefactura = value; }
        }
        public DateTime Ffactura
        {
            get { return ffactura; }
            set { ffactura = value; }
        }
        public string TipoCargo
        {
            get { return tipocargo; }
            set { tipocargo = value; }
        }

        public int RemisionPedido
        {
            get { return remisionpedido; }
            set { remisionpedido = value; }
        }

        public string SeriePedido
        {
            get { return seriepedido; }
            set { seriepedido = value; }
        }

        public int  FolioSat
        {
            get { return foliosat; }
            set { foliosat = value; }
        }

        public string SerieSat
        {
            get { return seriesat; }
            set { seriesat = value; }
        }

        public string ConceptoPedido
        {
            get { return conceptopedido; }
            set { conceptopedido = value; }
        }

        public decimal Total
        {
            get { return total; }
            set { total = value; }
        }

        public decimal TotalSaldo
        {
            get { return totalsaldo; }
            set { totalsaldo = value; }
        }

        public Boolean Selecciona
        {
            get { return selecciona; }
            set { selecciona = value; }
        }

        public int Cliente
        {
            get { return cliente; }
            set { cliente = value; }
        }

        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }

        public decimal Saldo
        {
            get { return saldo; }
            set { saldo = value; }
        }

        public int ClientePago
        {
            get { return clientePago; }
            set { clientePago = value; }
        }

        public byte TipoProducto
        {
            get { return tipoproducto; }
            set { tipoproducto = value; }
        }

        public bool Portatil
        {
            get { return portatil; }
            set { portatil = value; }
        }

        public int TipoCobro
        {
            get { return tipoCobro; }
            set { tipoCobro = value; }
        }

        public int TipoCobroAnterior
        {
            get { return tipoCobroAnterior; }
            set { tipoCobroAnterior = value; }
        }

        //public decimal SaldoDeposito
        //{
        //    get { return saldodeposito; }
        //    set { saldodeposito = value; }
        //}

        #endregion


        public abstract bool CobroPedidoAlta(short añocobro, int cobro, Conexion _conexion);
        public abstract bool CobroPedidoAlta(short añocobro, int cobro, decimal total, Conexion _conexion);

        public abstract bool PedidoActualizaSaldo(Conexion _conexion);
        public abstract bool PedidoActualizaSaldo(Conexion _conexion, decimal MontoConciliado);

        public abstract bool ActualizaPagosPorAplicar(Conexion _conexion);

        public abstract ReferenciaConciliadaPedido CrearObjeto();
        public abstract bool Guardar2(Conexion _conexion);

        public virtual string CadenaConexion
        {
            get
            {
                Conciliacion.RunTime.App objApp = new Conciliacion.RunTime.App();
                return objApp.CadenaConexion;
            }
        }

        public int ClientePadre
        {
            get
            {
                return clientePadre;
            }

            set
            {
                clientePadre = value;
            }
        }

        public String IDPedidoCRM
        {
            get
            {
                return _iDPedidoCRM;
            }

            set
            {
                _iDPedidoCRM = value;
            }
        }
    }
}
