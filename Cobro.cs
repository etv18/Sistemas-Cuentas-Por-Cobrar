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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectiFinal_CxC
{
    public partial class Cobro : Form
    {
        string dbUser, dbPassword, conStr;
        private double pago;
        public Cobro()
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

        private void Cobro_Load(object sender, EventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            MostrarDatos();
        }

        private void MostrarDatos()
        {
            int noFactura = int.Parse(txtNoFactura.Text);

            using (SqlConnection con = new SqlConnection(conStr))
            {
                string query = "SELECT * FROM factura WHERE noFactura = @noFactura";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@noFactura", noFactura);

                try
                {
                    con.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dgvFactura.DataSource = dt;

                    dgvFactura.AutoGenerateColumns = false; //This only make visible the columns you set up in the data grid view.
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");

                }
            }

            using (SqlConnection con = new SqlConnection(conStr))
            {
                string query = "SELECT monto, fecha FROM pagos WHERE noFactura = @noFactura";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@noFactura", noFactura);

                try
                {
                    con.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dgvPagos.DataSource = dt;

                    dgvPagos.AutoGenerateColumns = false; //This only make visible the columns you set up in the data grid view.
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");

                }
            }
        }

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

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            bool cobrar = false;
            using (SqlConnection con = new SqlConnection(conStr))
            {

                 try
                 {
                     con.Open();
                     ObtenerPago();

                    if (pago > 0)
                    {
                        if (VerificarPagoMenorABalance())
                        {
                            DateTime dateTime = DateTime.Now;
                            int noFactura = int.Parse(txtNoFactura.Text);

                            string query = "INSERT INTO pagos (noFactura, monto, fecha) VALUES (@noFactura, @pago, @dateTime)";

                            SqlCommand cmd = new SqlCommand(query, con);

                            cmd.Parameters.AddWithValue("@noFactura", noFactura);
                            cmd.Parameters.AddWithValue("@pago", pago);
                            cmd.Parameters.AddWithValue("@dateTime", dateTime);

                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Cobro guardado exitosamente !");
                            cobrar = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Introduzca una cifra mayor a 0.", "Aviso");

                    }

                }
                 catch (Exception ex)
                 {
                     MessageBox.Show("Error: " + ex.Message);
                 }
             }
            if(cobrar)
            {
                RealizarCobro();
                MostrarDatos();
            }

        }
        private bool VerificarPagoMenorABalance()
        {
            bool cobrar = false;
            using (SqlConnection con = new SqlConnection(conStr))
            {
                try
                {
                    int noFactura = int.Parse(txtNoFactura.Text);

                    string query = "SELECT balance FROM factura WHERE noFactura = @noFactura";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@noFactura", noFactura);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    decimal montoBalance;
                   
                    if (reader.Read())
                    {
                        montoBalance = (decimal)reader["balance"];

                        decimal dpago = (decimal)pago;
                        if (dpago <= montoBalance)
                        {
                            montoBalance -= dpago;
                            cobrar = true;
                        }
                        else
                        {
                            MessageBox.Show($"Introduzca una cifra menor o igual a {montoBalance}");

                        }

                    }
                    else
                    {
                        MessageBox.Show("Valor no encontrado");
                        montoBalance = 0;
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            return cobrar;
        }

        private void RealizarCobro()
        {
            using(SqlConnection con = new SqlConnection(conStr))
            {
                try
                {
                        int noFactura = int.Parse(txtNoFactura.Text);

                        string query = "SELECT balance FROM factura WHERE noFactura = @noFactura";

                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@noFactura", noFactura);

                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        decimal montoBalance;
                        bool cobrar = false;
                        if (reader.Read())
                        {
                            montoBalance = (decimal)reader["balance"];

                            decimal dpago = (decimal)pago;
                            if (dpago <= montoBalance)
                            {
                                montoBalance -= dpago;
                                cobrar= true;
                            }
                            else
                            {
                                MessageBox.Show($"Introduzca una cifra menor o igual a {montoBalance}");
                            }
                            
                        }
                        else
                        {
                            MessageBox.Show("Valor no encontrado");
                            montoBalance = 0;
                        }
                        reader.Close();
                    if (cobrar)
                    {
                        query = "UPDATE factura SET balance = @montoBalance WHERE noFactura = @noFactura";


                        SqlCommand cmd2 = new SqlCommand(query, con);

                        cmd2.Parameters.AddWithValue("@noFactura", noFactura);
                        cmd2.Parameters.AddWithValue("@montoBalance", montoBalance);
                        cmd2.ExecuteNonQuery();

                        if(montoBalance==0)
                        {
                            string estado = "Salda";
                            query = "UPDATE factura SET estado = @estado WHERE noFactura = @noFactura";
                            
                            SqlCommand cmd3 = new SqlCommand(query, con);
                            cmd3.Parameters.AddWithValue("@estado", estado);
                            cmd3.Parameters.AddWithValue("@noFactura", noFactura);
                            cmd3.ExecuteNonQuery();
                        }
                    }

                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error: "+ex.Message);
                }
            }
        }

        /*using(SqlConnection con = new SqlConnection(conStr))
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

                    string query = "INSERT INTO factura (noFactura, cliente, costo, fecha, descripcion, estado) VALUES (@noFactura, @cliente, @costo, @dt, @descripcion, @estado)";

                    SqlCommand cmd = new SqlCommand(query, con);

                    cmd.Parameters.AddWithValue("@noFactura",noFactura);
                    cmd.Parameters.AddWithValue("@cliente",cliente);
                    cmd.Parameters.AddWithValue("@costo",costo);
                    cmd.Parameters.AddWithValue("@descripcion",descripcion);
                    cmd.Parameters.AddWithValue("@dt",dt);
                    cmd.Parameters.AddWithValue("@estado",estado);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Factura correctamente guardada!");
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Ha ocurrido un error: "+ex.Message);
                }
            }*/

        private void ObtenerPago()
        {
            if (txtPago.Text.Length > 0)
            {
                string montoStr = txtPago.Text;

                string[] montos = montoStr.Split(',');

                montoStr = "";

                for (int i = 0; i < montos.Length; i++)
                {
                    montoStr += montos[i].Trim();
                }

                pago = double.Parse(montoStr);
            }
            else MessageBox.Show("Debe llenar el campo de pago.", "Aviso");
        }
        private void GuardarCobro()
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        CultureInfo culture = new CultureInfo("en-US");
        private void txtPago_TextChanged(object sender, EventArgs e)
        {
            CifraFormateada(sender, culture);
        }

        private void dgvFactura_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvFactura_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            this.dgvPagos.Rows[e.RowIndex].Cells["numPago"].Value = (e.RowIndex + 1).ToString();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnHide_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
