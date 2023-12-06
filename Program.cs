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
          
            fmLogin login = new fmLogin();
            Cobro cobro = new Cobro();
            Menu menu = new Menu();
            CrearFactura factura= new CrearFactura();
            Crear_Usuario cuser = new Crear_Usuario();
            //CrearFactura factura = new CrearFactura();
            Application.Run(login);


            /*REMEMBER: TO CREATE ALL THE OBJECTS HERE AND THEN OPEN AND CLOSE THEM USING CONDITIONALS FROM HERE.*/
            if (login.UserAuthenticated)
            {
                Application.Run(menu);
            }
        }
    }
}