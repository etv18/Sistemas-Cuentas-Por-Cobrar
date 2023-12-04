namespace ProyectiFinal_CxC
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
          
            // fmLogin login = new fmLogin();
            Menu menu = new Menu();
            Application.Run(menu);

            //if (login.UserAuthenticated)
            //{
            //    Menu menu = new Menu();
            //    Application.Run(menu);
            //}
        }
    }
}