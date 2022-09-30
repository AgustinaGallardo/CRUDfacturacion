using CRUDfacturacion.Datos;
using CRUDfacturacion.Dominio;
using CRUDfacturacion.Servicios.Implementacion;
using CRUDfacturacion.Servicios.Interfaz;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRUDfacturacion
{
    public partial class frmAltaFactura : Form
    {
        private iServicio gestor;
        private Factura nueva;
        public frmAltaFactura()
        {
            InitializeComponent();
            gestor = new Servicio();
            nueva = new Factura();
        }

        private void frmAltaFactura_Load(object sender, EventArgs e)
        {
            ObtenerProximo();
            ObtenerArticulos();
            ObtenerFormas();
        }
        private void ObtenerFormas()
        {
            cboFormaPago.DataSource = gestor.ObtenerFormas();
            cboFormaPago.DisplayMember = " IdFormaPago";
            cboFormaPago.ValueMember= "tipoFP";
            cboFormaPago.DropDownStyle=ComboBoxStyle.DropDownList;
        }
        private void ObtenerArticulos()
        {
            cboArticulo.DataSource = gestor.ObtenerArticulos();
            cboArticulo.ValueMember = "idArticulo";
            cboArticulo.DisplayMember = "nombre";
            cboArticulo.DropDownStyle=ComboBoxStyle.DropDownList;
        }
        private void ObtenerProximo()
        {
            int next = gestor.ObtenerProximo();
            if (next > 0)
            {
                lblNext.Text = "Proxima Factura:" + next.ToString();
            }
            else
            {
                MessageBox.Show("No se puede conseguir la proxima factura!!!","error",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if(cboArticulo.SelectedIndex == -1)//si o si coloque un articulo
            {
                MessageBox.Show("Tiene que seleccionar un articulo", "Error", 
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
            }
            if(int.Parse(txtCantidad.Text) < 0 ) //que la cantidad sea mayor a 0
            {
                MessageBox.Show("La cantidad de articulos debe ser mayor a 0","error",
                     MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
            }
            foreach (DataGridViewRow rows in dgvFactura.Rows) //no se puede agregar el mismo articulo a la grilla
            {
                if (rows.Cells["colArticulo"].Value.ToString().Equals(cboArticulo.Text))
                {
                    MessageBox.Show("El articulo " + cboArticulo.Text + " Ya se encuentra en la lista!!! ", "control",
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return;
                }
            }

                Articulo a = (Articulo)cboArticulo.SelectedItem;
               
                int cant = Convert.ToInt32(txtCantidad.Text);

                DetalleFactura df = new DetalleFactura(a,cant);
                nueva.AgregarDetalle(df);

                dgvFactura.Rows.Add(df.Articulo.IdArticulo,
                                    df.Articulo.Nombre, 
                                    df.Cantidad,
                                    df.Articulo.PrecioUnitario);

                txtSubTotal.Text = df.CalcularSubTotal().ToString();
                txtTotal.Text = nueva.CalcularTotal().ToString();
            }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if(txtCliente.Text == "")
            {
                MessageBox.Show("Tiene que agregar un cliente", "error", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop);
            }
            if(cboFormaPago.SelectedIndex == -1)
            {
                MessageBox.Show("Tiene que seleecionar una forma de pago", "error", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop);
            }
            GuardarFactura();
        }

        private void GuardarFactura()
        {
            FormaPago fp = (FormaPago)cboFormaPago.SelectedItem;
            nueva.FormaPago = fp;
            nueva.Cliente = txtCliente.Text;
            nueva.Fecha = dtpFecha.Value;

            if(Helper.ObtenerInstancia().ConfirmarFactura(nueva))
            {
                MessageBox.Show("Se agrego con exito la factura!!!!!!", "Registro", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation);
            }
            else
            {
                MessageBox.Show("NO SE PUEDO REGISTRAR LA FACTURA", "ERROR", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
            }

            LimpiarCampos();
        }

        private void LimpiarCampos()
        {
            txtCantidad.Text="";
            txtCliente.Text="";
            txtSubTotal.Text="";
            txtTotal.Text="";
            cboArticulo.SelectedIndex=-1;
            cboFormaPago.SelectedIndex=-1;
            dgvFactura.Rows.Clear();
        }

        private void label2_Click(object sender, EventArgs e)
        {
                
        }

        private void txtCliente_TextChanged(object sender, EventArgs e)
        {

        }

        private void dgvFactura_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(dgvFactura.CurrentCell.ColumnIndex==4)
            {
                nueva.QuitarDetalle(dgvFactura.CurrentRow.Index);
                dgvFactura.Rows.Remove(dgvFactura.CurrentRow);
                nueva.CalcularTotal();
            }
        }
    }       
}

