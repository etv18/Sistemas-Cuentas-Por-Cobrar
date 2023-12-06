using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectiFinal_CxC
{
    public partial class CrearFactura : Form
    {
        public bool BackOrClose { get; private set; }
        private bool pendiente;
        private double costo;
        private int noFactura;
        private string estado, descripcion, conStr, cliente, dbUser,dbPassword;
        public CrearFactura()
        {
            InitializeComponent();
            StringConnector();
        }

        private void GetCredentialsDB()
        {
            string env_v = "C_DTLS"; //Name of the enviroment variable.
            try
            {
                string configFilePath = Environment.GetEnvironmentVariable(env_v);

                if (File.Exists(configFilePath))
                {
                    string[] lines = File.ReadAllLines(configFilePath);

                    if (lines.Length > 0)
                    {
                        string[] cdtls = lines[0].Split(',');

                        if (cdtls.Length == 2)
                        {
                            dbUser = cdtls[0].Trim();
                            dbPassword = cdtls[1].Trim();

                        }
                        else MessageBox.Show("The file it doesn't have the expected format.");
                    }
                    else MessageBox.Show("The file is empty");

                }
                else MessageBox.Show($"The enviroment variable {env_v} is not set or the file doesn't exists in the especified path.");

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error trying to read the file: " + ex.Message);
            }
        }

        private void StringConnector()
        {
            GetCredentialsDB();
            string connectionString = $"Data Source=LAPTOP-68IMB34E;Initial Catalog=ProyectoFinal;User ID={dbUser};Password={dbPassword};Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            conStr = connectionString;
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
            pendiente = true;
        }

        private void rbtnContado_CheckedChanged(object sender, EventArgs e)
        {
            lblEstadoFactura.Text = "Salda";
            pendiente= false;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            GuardarFactura();
            //ImprimirFactura();
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
                    cliente = txtCliente.Text;
                    estado = lblEstadoFactura.Text;
                    double balance = 0;
                    if (pendiente) balance = costo;
                    

                    string query = "INSERT INTO factura (noFactura, cliente, costo, fecha, descripcion, estado, balance) VALUES (@noFactura, @cliente, @costo, @dt, @descripcion, @estado, @balance)";

                    SqlCommand cmd = new SqlCommand(query, con);

                    cmd.Parameters.AddWithValue("@noFactura",noFactura);
                    cmd.Parameters.AddWithValue("@cliente",cliente);
                    cmd.Parameters.AddWithValue("@costo",costo);
                    cmd.Parameters.AddWithValue("@descripcion",descripcion);
                    cmd.Parameters.AddWithValue("@dt",dt);
                    cmd.Parameters.AddWithValue("@estado",estado);
                    cmd.Parameters.AddWithValue("@balance",balance);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Factura correctamente guardada!");
                    Limpiar();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Ha ocurrido un error: "+ex.Message);
                }
            }
        }

        private void Limpiar()
        {
            txtNoFactura.Text = "";
            txtDescripcion.Text = "";
            txtCliente.Text = "";
            txtCosto.Text = "";
            rbtnCredito.Checked= true;
        }

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

        private void ImprimirFactura()
        {
            DateTime date= DateTime.Now;
            string nombreArchivo = $"{date}factura.txt";
            int numeroFactura = int.Parse(txtNoFactura.Text);
            double balance = 0;
            if(lblEstadoFactura.Text == "Pendiente")
            {
                balance = costo;
            }
            // Generar los datos de la factura con las variables
          string[] datosFactura = {
                "Factura",
                "-----------------------------------------------------",
                $"Fecha\t\t{date}",
                $"No. {numeroFactura}", // Reemplaza 'valor' con la variable correspondiente
                $"Cliente\t\t{cliente}",
                $"Monto\t\t{costo}",
                $"Descripcion\t{descripcion}",
                $"Estado\t\t{estado}",
                $"Balance\t\t{balance}"
          };

            try
            {
                // Ruta donde se guardará el archivo
                string rutaArchivo = @"C:\Facturas";
          

                // Escribe los datos en el archivo
                File.WriteAllLines(rutaArchivo, datosFactura);

                MessageBox.Show("Archivo de factura creado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear el archivo de factura: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
