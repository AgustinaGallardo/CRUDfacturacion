using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using CRUDfacturacion.Dominio;

namespace CRUDfacturacion.Datos
{
    internal class Helper
    {
        private static Helper instancia;
        SqlConnection cnn = new SqlConnection(Properties.Resources.cnnCRUDfactura);


        public static Helper ObtenerInstancia()
        {
            if(instancia == null)
            {
                instancia = new Helper();
            }
            return instancia;
        }

        public int ObtenerProximo(string sp_nombre,string OutPut)
        {
            cnn.Open();
            SqlCommand cmdProx = new SqlCommand();
            cmdProx.CommandText = sp_nombre;
            cmdProx.CommandType = CommandType.StoredProcedure;
            cmdProx.Connection = cnn;

            SqlParameter pOutPut = new SqlParameter();
            pOutPut.ParameterName = OutPut;
            pOutPut.Direction = ParameterDirection.Output;
            pOutPut.DbType = DbType.Int32;

            cmdProx.Parameters.Add(pOutPut);
            cmdProx.ExecuteNonQuery();
            cnn.Close();
            return (int)pOutPut.Value;
        }

        public DataTable CargarCombo(string sp_nombre)
        {
            DataTable tabla = new DataTable();
            cnn.Open();
            SqlCommand cmdCombo = new SqlCommand();
            cmdCombo.CommandText = sp_nombre;
            cmdCombo.CommandType = CommandType.StoredProcedure;
            cmdCombo.Connection = cnn;
            tabla.Load(cmdCombo.ExecuteReader());
            cnn.Close();
            return tabla;
        }

        public bool ConfirmarFactura(Factura oFactura)
        {
            bool ok = true;
            SqlTransaction t = null;

            try
            {
                t = cnn.BeginTransaction();
                SqlCommand cmdMaestro = new SqlCommand();
                cnn.Open();
            
                
                cmdMaestro.Connection = cnn;
                cmdMaestro.Transaction = t;
                cmdMaestro.CommandType = CommandType.StoredProcedure;
                cmdMaestro.CommandText= "SP_INSERT_MAESTRO";
                

                cmdMaestro.Parameters.AddWithValue("@fecha",oFactura.Fecha);
                cmdMaestro.Parameters.AddWithValue("@id_formapago", oFactura.FormaPago.IdFormaPago);
                cmdMaestro.Parameters.AddWithValue("@cliente",oFactura.Cliente);

                SqlParameter OutPut = new SqlParameter();
                OutPut.ParameterName = "@nroFactura";
                OutPut.DbType = DbType.Int32;
                OutPut.Direction = ParameterDirection.Output;                

                cmdMaestro.Parameters.Add(OutPut);

                cmdMaestro.ExecuteNonQuery();

                int nroFactura = (int)OutPut.Value;

                foreach (DetalleFactura df in oFactura.ListDetalles)
                {
                    SqlCommand cmdDetalle = new SqlCommand();

                    cmdDetalle.Connection = cnn;
                    cmdDetalle.Transaction=t;
                    cmdDetalle.CommandType=CommandType.StoredProcedure;
                    cmdDetalle.CommandText="sp_INSERTAR_DETALLE";

                    cmdDetalle.Parameters.AddWithValue("@nroFactura",nroFactura);
                    cmdDetalle.Parameters.AddWithValue("@id_articulo", df.Articulo.IdArticulo);
                    cmdDetalle.Parameters.AddWithValue("@cantidad", df.Cantidad);

                    cmdDetalle.ExecuteNonQuery();
                }

                t.Commit();
            }
           catch (Exception )
            {
                if(t != null)
                {
                    t.Rollback();
                    ok=false;
                }
            }
            finally
            {
                if (cnn !=null && cnn.State == ConnectionState.Open)
                    cnn.Close();
            }
            return ok;
        }
    }
}
