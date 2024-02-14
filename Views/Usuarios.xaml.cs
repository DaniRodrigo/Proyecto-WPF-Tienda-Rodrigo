using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Controls;
using System.Windows;
using MySqlConnector;


namespace Tienda_Rodrigo.Views
{
    public partial class Usuarios : UserControl
    {
        private readonly string connectionString;

        public Usuarios()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["conexionDB"].ConnectionString;
            CargarDatos();
        }

        private void CargarDatos()
        {
            using (MySqlConnection con = new MySqlConnection(connectionString)) // Utiliza MySqlConnection
            {
                try
                {
                    con.Open();
                    string query = "SELECT IdUsuario, Nombres, Apellidos, Telefono, Correo, NombrePrivilegio " +
                                   "FROM Usuarios INNER JOIN Privilegios ON Usuarios.Privilegio = Privilegios.IdPrivilegio " +
                                   "ORDER BY IdUsuario ASC";
                    MySqlCommand cmd = new MySqlCommand(query, con); // Utiliza MySqlCommand
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd); // Utiliza MySqlDataAdapter
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    GridDatos.ItemsSource = dt.DefaultView;
                }
                catch (Exception ex)
                {
                    // Manejo de excepciones, por ejemplo, mostrar un mensaje de error
                    Console.WriteLine("Error al cargar datos: " + ex.Message);
                }
            }
        }

        private void Agregar(object sender, RoutedEventArgs e)
        {
            CRUDUsuarios ventana = new CRUDUsuarios();
            FrameUsuarios.Content = ventana;
            Contenido.Visibility = Visibility.Hidden;
            ventana.BtnCrear.Visibility = Visibility.Visible;
        }

        private void Consultar(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            CRUDUsuarios ventana = new CRUDUsuarios();
            ventana.IdUsuario = id;
            ventana.Consultar();
            FrameUsuarios.Content = ventana;
            Contenido.Visibility = Visibility.Hidden;
            ventana.Titulo.Text = "Consulta de Usuario";
            ventana.tbNombres.IsEnabled = false;
            ventana.tbApellidos.IsEnabled = false;
            ventana.tbDUI.IsEnabled = false;
            ventana.tbNIT.IsEnabled = false;
            ventana.tbFecha.IsEnabled = false;
            ventana.tbTelefono.IsEnabled = false;
            ventana.tbCorreo.IsEnabled = false;
            ventana.cbPrivilegio.IsEnabled = false;
            ventana.tbUsuario.IsEnabled = false;
            ventana.tbContrasenia.IsEnabled = false;
            ventana.BtnSubir.IsEnabled = false;
        }

        private void Actualizar(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            CRUDUsuarios ventana = new CRUDUsuarios();
            ventana.IdUsuario = id;
            ventana.Consultar();
            FrameUsuarios.Content = ventana;
            Contenido.Visibility = Visibility.Hidden;
            ventana.Titulo.Text = "Actualización de Usuario";
            ventana.tbNombres.IsEnabled = true;
            ventana.tbApellidos.IsEnabled = true;
            ventana.tbDUI.IsEnabled = true;
            ventana.tbNIT.IsEnabled = true;
            ventana.tbFecha.IsEnabled = true;
            ventana.tbTelefono.IsEnabled = true;
            ventana.tbCorreo.IsEnabled = true;
            ventana.cbPrivilegio.IsEnabled = true;
            ventana.tbUsuario.IsEnabled = true;
            ventana.tbContrasenia.IsEnabled = true; ;
            ventana.BtnSubir.IsEnabled = true;

            ventana.BtnModificar.Visibility = Visibility.Visible;
        }

        private void Eliminar(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            CRUDUsuarios ventana = new CRUDUsuarios();
            ventana.IdUsuario = id;
            ventana.Consultar();
            FrameUsuarios.Content = ventana;
            Contenido.Visibility = Visibility.Hidden;
            ventana.Titulo.Text = "Eliminar Usuario";
            ventana.tbNombres.IsEnabled = false;
            ventana.tbApellidos.IsEnabled = false;
            ventana.tbDUI.IsEnabled = false;
            ventana.tbNIT.IsEnabled = false;
            ventana.tbFecha.IsEnabled = false;
            ventana.tbTelefono.IsEnabled = false;
            ventana.tbCorreo.IsEnabled = false;
            ventana.cbPrivilegio.IsEnabled = false;
            ventana.tbUsuario.IsEnabled = false;
            ventana.tbContrasenia.IsEnabled = false;
            ventana.BtnSubir.IsEnabled = false;

            ventana.BtnEliminar.Visibility = Visibility.Visible;
        }

        private void Buscando(object sender, TextChangedEventArgs e)
        {
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string nombreUsuario = ((TextBox)sender).Text;
                    string query = "SELECT IdUsuario, Nombres, Apellidos, Telefono, Correo, NombrePrivilegio " +
                                   "FROM Usuarios INNER JOIN Privilegios ON Usuarios.Privilegio = Privilegios.IdPrivilegio " +
                                   "WHERE Nombres LIKE @Nombre OR Apellidos LIKE @Nombre " +
                                   "ORDER BY IdUsuario ASC";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Nombre", "%" + nombreUsuario + "%");
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    GridDatos.ItemsSource = dt.DefaultView;
                }
                catch (Exception ex)
                {
                    // Manejo de excepciones, por ejemplo, mostrar un mensaje de error
                    Console.WriteLine("Error al buscar datos: " + ex.Message);
                }
            }
        }

        public bool LogIn(string usuario, string contra)
        {
            bool loggedIn = false;

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT IdUsuario FROM Usuarios WHERE Usuario = @Usuario AND Contrasenia = @Contrasenia";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Usuario", usuario);
                    cmd.Parameters.AddWithValue("@Contrasenia", contra);

                    object result = cmd.ExecuteScalar();

                    if (result != null) // Si se encontró un usuario con las credenciales proporcionadas
                    {
                        loggedIn = true;
                    }
                }
                catch (Exception ex)
                {
                    // Manejo de excepciones, por ejemplo, mostrar un mensaje de error
                    Console.WriteLine("Error al intentar iniciar sesión: " + ex.Message);
                }
            }

            return loggedIn;
        }





    }
}
