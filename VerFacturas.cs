using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectiFinal_CxC
{
    public partial class VerFacturas : Form
    {
        string dbUser, dbPassword, conStr;
        public VerFacturas()
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

        private void MostrarDatos()
        {
            using(SqlConnection con = new SqlConnection(conStr))
            {
                string query = "SELECT * FROM factura";
                SqlCommand cmd = new SqlCommand(query, con);

                try
                {
                    con.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dgvRegistrosFacturas.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }
        /* using (SqlConnection con = new SqlConnection(conStr))
        {
            string query = "SELECT * FROM products";
            SqlCommand command = new SqlCommand(query, con);

            try
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dgvData.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }*/
        private void StringConnector()
        {
            GetCredentialsDB();
            string connectionString = $"Data Source=LAPTOP-68IMB34E;Initial Catalog=ProyectoFinal;User ID={dbUser};Password={dbPassword};Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            conStr = connectionString;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void VerFacturas_Load(object sender, EventArgs e)
        {
            MostrarDatos();
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
