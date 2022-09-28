using CRUDfacturacion.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDfacturacion.Datos.Interfaz
{
    interface iDaoFactura
    {
        int ObtenerProximo();
        List<FormaPago> ObtenerFormas();
        List<Articulo> ObtenerArticulos();
    }
}
