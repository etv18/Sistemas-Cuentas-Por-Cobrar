using Microsoft.VisualBasic.ApplicationServices;
using System.Data.SqlClient;

namespace ProyectiFinal_CxC
{
    public partial class fmLogin : Form
    {
        string conStr, dbUser, dbPassword;
        public bool UserAuthenticated { get; private set; }
        public bool WantsToRegister { get; private set; }
        public fmLogin()
        {
            InitializeComponent();
            StringConnector();
        }

        private void fmLogin_Load(object sender, EventArgs e)
        {

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


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnHide_Click(object sender, EventArgs e)
        {
            this.WindowState= FormWindowState.Minimized;
        }

        private void ValidUser()
        {
            string user = txtUser.Text;
            string password = txtpassword.Text;
            string query = "SELECT COUNT(*) FROM usuarios WHERE user_name = @user AND password = @password";

            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@user", user);
                    cmd.Parameters.AddWithValue("@password", password);

                    try
                    {
                        con.Open();

                        int results = (int)cmd.ExecuteScalar();

                        if (results > 0)
                        {
                            UserAuthenticated = true;
                            this.Close();
                        }
                        else
                        {
                           MessageBox.Show("Credenciales Invalidas.", "ERROR");
                        }
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("Error trying to connect to the data base: " + ex.Message);
                        UserAuthenticated = false;
                    }
                }
            }
        }


        private void btnLogin_Click(object sender, EventArgs e)
        {
            ValidUser();
            
        }
        string USER, PASSWORD;
        private bool Validacion()
        {
            bool valido = false;
            bool validoU = false;
            bool validoP = false;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                string user = txtUser.Text;
                List<string> users = new List<string>();
                List<string> passwords = new List<string>();
                string query = "SELECT user_name, password FROM usuarios";
                try
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    con.Open();

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        int userIndex = reader.GetOrdinal("user_name");
                        int passIndex = reader.GetOrdinal("password");

                        while (reader.Read())
                        {
                            string datoUser_name = (string)reader.GetString(userIndex);
                            string datoPassword = (string)reader[passIndex];
                            users.Add(datoUser_name);
                            passwords.Add(datoPassword);
                        }
                        foreach (string elem in users)
                        {
                            if (elem.Equals(user))
                            {
                                validoU = true;
                                USER = elem;
                                break;
                            }
                        }
                        foreach(string elem in passwords)
                        {
                            if (elem.Equals(passwords))
                            {
                                validoP = true;
                                PASSWORD = elem;
                                break;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("No se encontro ningun dato");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            if (validoU == true && validoP == true)
            {
                valido = true;
            }
            return valido;
        }

        private void btnSee_Click(object sender, EventArgs e)
        {
            if (txtpassword.PasswordChar == '*')
            {
                btnNotSee.BringToFront();
                txtpassword.PasswordChar = '\0';
            }
        }

        private void btnNotSee_Click(object sender, EventArgs e)
        {
            if (txtpassword.PasswordChar == '\0')
            {
                btnSee.BringToFront();
                txtpassword.PasswordChar = '*';
            }
        }
    }
}