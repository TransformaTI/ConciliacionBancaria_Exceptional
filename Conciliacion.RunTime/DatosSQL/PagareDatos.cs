///////////////////////////////////////////////////////////
//  PagareDatos.cs
//  Implementation of the Class PagareDatos
//  Generated by Enterprise Architect
//  Created on:      27-nov-2017 03:23:16 p.m.
//  Original author: Desarollo_Transforma
///////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Data;
using Conciliacion.RunTime.ReglasDeNegocio;
using System.Data.SqlClient;

namespace Conciliacion.RunTime.DatosSQL
{
    public class PagareDatos : Pagare
    {
        /// <summary>
        /// Constructor sobrecargado de la clase
        /// </summary>
        /// <param name="implementadorMensajes"></param>
        public PagareDatos(MensajesImplementacion implementadorMensajes)
            : base(implementadorMensajes)
        {
        }

        /// <summary>
        /// Constructor sobrecargado de la clase
        /// </summary>
        /// <param name="foliocorte"></param>
        /// <param name="foperacion"></param>
        /// <param name="caja"></param>
        /// <param name="consecutivo"></param>
        /// <param name="descripcion"></param>
        /// <param name="total"></param>
        /// <param name="observaciones"></param>
        /// <param name="anioPed"></param>
        /// <param name="celula"></param>
        /// <param name="pedido"></param>
        /// <param name="cobranza"></param>
        /// <param name="saldo"></param>
        /// <param name="gestioninicial"></param>
        /// <param name="implementadorMensajes"></param>
        public PagareDatos(
            int foliocorte,
            DateTime foperacion,
            int caja,
            int consecutivo,
            string descripcion,
            decimal total,
            string observaciones,
            short anioPed,
            short celula,
            int pedido,
            int cobranza,
            decimal saldo,
            short gestioninicial,
            MensajesImplementacion implementadorMensajes
            )
            : base(
                foliocorte,
                foperacion,
                caja,
                consecutivo,
                descripcion,
                total,
                observaciones,
                anioPed,
                celula,
                pedido,
                cobranza,
                saldo,
                gestioninicial,
                implementadorMensajes
            )
        {
        }

        public override Pagare CrearObjeto()
        {
            return new PagareDatos(this.implementadorMensajes);
        }

        public virtual void Dispose()
        {

		}
        #region comentarios metodo
        /// <summary>
        /// Invocar al procedimiento almacenado spCBConsultaPagare y regresar una lista de
        /// instancias de la clase Pagare
        /// </summary>
        /// <param name="FechaFin"></param>
        /// <param name="FechaIni"></param>
        #endregion
        public override List<Pagare> ConsultaPagares(Conexion _conexion, DateTime FechaFin, DateTime FechaIni)
        {
            SqlDataReader drConsulta = null;
            try
            {
                _conexion.Comando.CommandType = CommandType.StoredProcedure;
                _conexion.Comando.CommandText = "spCBConsultaPagare";
                _conexion.Comando.Parameters.Clear();
                _conexion.Comando.Parameters.Add(new SqlParameter("@FechaIni", System.Data.SqlDbType.Date)).Value = FechaIni;
                _conexion.Comando.Parameters.Add(new SqlParameter("@FechaFin", System.Data.SqlDbType.Date)).Value = FechaFin;
                _conexion.Comando.Parameters.Add(new SqlParameter("@Todos", System.Data.SqlDbType.Bit)).Value = 1;
                _conexion.Comando.ExecuteNonQuery();

                drConsulta = _conexion.Comando.ExecuteReader();
                if (drConsulta.HasRows)
                {
                    while (drConsulta.Read())
                    {
                        Pagare dato = new PagareDatos(Convert.ToInt16(drConsulta["FolioCorte"]),
                                       Convert.ToDateTime(drConsulta["FOperacion"]),
                                       Convert.ToInt16(drConsulta["Caja"]),
                                       Convert.ToInt16(drConsulta["Consecutivo"]),
                                       Convert.ToString(drConsulta["Descripcion"]),
                                       Convert.ToDecimal(drConsulta["Total"]),
                                       Convert.ToString(drConsulta["Observaciones"]),
                                       0, // AnioPed
                                       0, //Celula 
                                       0, //Pedido 
                                       0, //Cobranza 
                                       0, //Saldo 
                                       0, //GestionInicial 
                                       this.implementadorMensajes
                            );
                    }
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        #region comentarios metodo
        /// <summary>
        /// Invoca al procedimiento almacenado spCBExtractorRobo
        /// </summary>
        /// <param name="Corporativo"></param>
        /// <param name="Sucursal"></param>
        /// <param name="FechaIni"></param>
        /// <param name="FechaFin"></param>
        /// <param name="CuentaBanco"></param>
        #endregion
        public override List<Pagare> CargaPagare(Conexion _conexion, sbyte Corporativo, sbyte Sucursal, DateTime FechaIni, DateTime FechaFin, string CuentaBanco)
        {
            SqlDataReader drConsulta = null;
            try
            {
                _conexion.Comando.CommandType = CommandType.StoredProcedure;
                _conexion.Comando.CommandText = "spCBExtractorRobo";
                _conexion.Comando.Parameters.Clear();
                _conexion.Comando.Parameters.Add(new SqlParameter("@Corporativo", System.Data.SqlDbType.SmallInt)).Value = Corporativo;
                _conexion.Comando.Parameters.Add(new SqlParameter("@Sucursal", System.Data.SqlDbType.SmallInt)).Value = Sucursal;
                _conexion.Comando.Parameters.Add(new SqlParameter("@FechaIni", System.Data.SqlDbType.Date)).Value = FechaIni;
                _conexion.Comando.Parameters.Add(new SqlParameter("@FechaFin", System.Data.SqlDbType.Date)).Value = FechaFin;
                _conexion.Comando.Parameters.Add(new SqlParameter("@CuentaBanco", System.Data.SqlDbType.SmallInt)).Value = CuentaBanco;
                _conexion.Comando.ExecuteNonQuery();

                drConsulta = _conexion.Comando.ExecuteReader();
                if (drConsulta.HasRows)
                {
                    while (drConsulta.Read())
                    {

                    }
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

    }//end PagareDatos

}//end namespace Pagares