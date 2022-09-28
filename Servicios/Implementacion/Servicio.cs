using CRUDfacturacion.Datos.Implementacion;
using CRUDfacturacion.Datos.Interfaz;
using CRUDfacturacion.Dominio;
using CRUDfacturacion.Servicios.Interfaz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDfacturacion.Servicios.Implementacion
{
    class Servicio : iServicio
    {
        private iDaoFactura dao;

        public Servicio()
        {
            dao = new DaoFactura();
        }

        public  List<Articulo> ObtenerArticulos()
        {
           return dao.ObtenerArticulos();
        }

        public List<FormaPago> ObtenerFormas()
        {
            return dao.ObtenerFormas();
        }

        public int ObtenerProximo()
        {
            return dao.ObtenerProximo();
        }
    }
}
