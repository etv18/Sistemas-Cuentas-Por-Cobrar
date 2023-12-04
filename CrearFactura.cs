using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectiFinal_CxC
{
    public partial class CrearFactura : Form
    {
        public bool BackOrClose { get; private set; }
        private double costo;
        private int noFactura;
        private string estado, descripcion, conStr;
        public CrearFactura()
        {
            InitializeComponent();
        }

        public CrearFactura(string conStr)
        {
            this.conStr = conStr;
        }

        private void btnHide_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            BackOrClose= true;
            this.Close();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            BackOrClose= true;
            this.Close();
        }

        private void CrearFactura_Load(object sender, EventArgs e)
        {

        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {
            if(rbtnContado.Checked)
            {
               rbtnContado_CheckedChanged(sender, e);
            }
            else
            {
                rbtnCredito_CheckedChanged(sender, e);
            }
        }

        private void rbtnCredito_CheckedChanged(object sender, EventArgs e)
        {
            lblEstadoFactura.Text = "Pendiente";
        }

        private void rbtnContado_CheckedChanged(object sender, EventArgs e)
        {
            lblEstadoFactura.Text = "Salda";
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
         
        }

        CultureInfo culture = new CultureInfo("en-US");
        public void CifraFormateada(object sender, CultureInfo culture)
        {
            if (sender is TextBox textBox)
            {
                //Eliminar los caracteres no numericos

                string txtLimpio = string.Join("", textBox.Text.Where(char.IsDigit));

                //Formatea el numero con comas
                if (long.TryParse(txtLimpio, out long salarioNumerico))
                {
                    textBox.Text = salarioNumerico.ToString("N0", culture); //Este N0 es para agg las comas a los miles
                    textBox.SelectionStart = textBox.Text.Length; //Esto agrega el cursor al final de txt
                }
            }
        }

        private void GuardarFactura()
        {
            using(SqlConnection con = new SqlConnection(conStr))
            {
                try
                {
                    con.Open();
                    noFactura = int.Parse(txtNoFactura.Text);
                    ObtenerCosto(); //aqui la variable costo tomara el valor de la caja de texto costo.
                    DateTime dt = DateTime.Now;
                    descripcion = txtDescripcion.Text;

                    
                }
            }
        }
        /*
                 private void SaveUser()
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();
                string name = txtName.Text;
                string user_name = txtuser_name.Text;
                string password = txtpassword.Text;

                string query = "INSERT INTO usuarios (name, user_name, password) VALUES (@name, @user_name, @password)";

                SqlCommand cmd = new SqlCommand(query, con);
            
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@user_name", user_name);
                cmd.Parameters.AddWithValue("@password", password);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Usuario correctamente guardado");
            }

            userSaved = true;
        }
         
         */
        private void txtNoFactura_TextChanged(object sender, EventArgs e)
        {
           if (!System.Text.RegularExpressions.Regex.IsMatch(txtNoFactura.Text, "^[0-9]*$"))
           {
                MessageBox.Show("Por favor, ingrese solo números.", "Aviso");
                txtNoFactura.Text = string.Empty; // Limpia el contenido si no es un número
           }
        }

        private void txtCosto_TextChanged(object sender, EventArgs e)
        {
            CifraFormateada(sender, culture);
        }

        private void ObtenerCosto()
        {
            string montoStr = txtCosto.Text;
            string[] montos = montoStr.Split(',');

            montoStr = "";

            for(int i = 0; i < montos.Length; i++)
            {
                montoStr+= montos[i].Trim();
            }

            costo = double.Parse(montoStr);
        }
    }
}
