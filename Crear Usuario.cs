using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ProyectiFinal_CxC
{
    public partial class Crear_Usuario : Form
    {
        private string dbUser, dbPassword, conStr, user, password, rpassword, nombre;
        private bool usuarioR, passwordCoinciden;
        public Crear_Usuario()
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

        private void txtUser_TextChanged(object sender, EventArgs e)
        {
            if (UsuarioRepetido())
            {
                lblRepetido.Text = "Usuario Registrado.";
                usuarioR = true;
            }
            else
            {
                lblRepetido.Text = "";
                usuarioR = false;
            }
        }

        private void StringConnector()
        {
            GetCredentialsDB();
            string connectionString = $"Data Source=LAPTOP-68IMB34E;Initial Catalog=ProyectoFinal;User ID={dbUser};Password={dbPassword};Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            conStr = connectionString;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (CheckBoxes())
            {
                if (usuarioR == false && passwordCoinciden == true)
                {
                    using (SqlConnection con = new SqlConnection(conStr))
                    {
                        try
                        {
                            con.Open();
                            nombre = txtNombre.Text;
                            user = txtUser.Text;
                            password = txtPassword.Text;
                            string query = "INSERT INTO usuarios (name, user_name, password) VALUES (@nombre, @user, @password)";
                            SqlCommand cmd = new SqlCommand(query, con);

                            cmd.Parameters.AddWithValue("@nombre", nombre);
                            cmd.Parameters.AddWithValue("@user", user);
                            cmd.Parameters.AddWithValue("@password", password);

                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Usuario correctamente guardado!");
                            Limpiar();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.Message);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Llene los campos de Manera Correcta.", "Aviso");
                }
            }
        }

        private bool CheckBoxes()
        {
            if (!string.IsNullOrWhiteSpace(txtNombre.Text)
                && !string.IsNullOrWhiteSpace(txtPassword.Text)
                && !string.IsNullOrWhiteSpace(txtUser.Text)
                && !string.IsNullOrWhiteSpace(txtRepeatedPassword.Text))
            {
                return true;
            }
            else
            {
                MessageBox.Show("Todos los campos deben ser llenados.");
                return false;
            }
        }

        private void Limpiar()
        {
            txtNombre.Text = "";
            txtPassword.Text = "";
            txtUser.Text = "";
            txtRepeatedPassword.Text = "";
        }

        private void txtRepeatedPassword_TextChanged(object sender, EventArgs e)
        {
            if (!CheckPassword())
            {
                lblNoCoinciden.Text = "Las contraseñas no coinciden.";
                passwordCoinciden = false;
            }
            else
            {
                lblNoCoinciden.Text = "";
                passwordCoinciden = true;
            }
        }

        private bool CheckPassword()
        {
            bool repetida = false;

            password= txtPassword.Text;
            rpassword = txtRepeatedPassword.Text;

            if(rpassword.Equals(password))
            {
                repetida = true;
            }

            return repetida;
        }

        private bool UsuarioRepetido()
        {
            bool repetido = false;

            using(SqlConnection con = new SqlConnection(conStr))
            {
                string user = txtUser.Text;
                List<string> users = new List<string>();
                string query = "SELECT user_name FROM usuarios";
                try
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    con.Open();

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        int columnIndex = reader.GetOrdinal("user_name");

                        while(reader.Read())
                        {
                            string datoUser_name = (string)reader.GetString(columnIndex);
                            users.Add(datoUser_name);
                        }
                        foreach (string elem in users)
                        {
                            if (elem.Equals(user))
                            {
                                repetido= true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("No se encontro ningun dato");
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error: "+ex.Message);
                }
            }

            return repetido;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnHide_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void Crear_Usuario_Load(object sender, EventArgs e)
        {

        }
    }
}
