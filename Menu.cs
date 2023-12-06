using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectiFinal_CxC
{
    public partial class Menu : Form
    {
        public bool CrearFactura { get;private set; }
        public bool  Cobrar { get; private set; }
        public bool VerFacturas { get; private set; }
        public bool CrearUsuario { get; private set; }
        private string conStr;
        public bool Close { get; private set; }
        public Menu()
        {
            InitializeComponent();
        }

        public Menu(string conStr)
        {
            this.conStr = conStr;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Desea Cerrar el programa?", "Aviso", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Close = true;
                Close();
            }

            //Close();

        }

        private void btnHide_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnCrearFactura_Click(object sender, EventArgs e)
        {
            CrearFactura= true;

            CrearFactura factura= new CrearFactura();
            factura.Show();
        }

        private void btnCobrar_Click(object sender, EventArgs e)
        {
            Cobro cobro = new Cobro();
            cobro.Show();
        }

        private void btnVerFacturas_Click(object sender, EventArgs e)
        {
            VerFacturas verFacturas= new VerFacturas();
            verFacturas.Show();
        }

        private void Menu_Load(object sender, EventArgs e)
        {

        }

        private void btnCrearUser_Click(object sender, EventArgs e)
        {
            Crear_Usuario usuario = new Crear_Usuario();
            usuario.Show();
        }
    }
}
