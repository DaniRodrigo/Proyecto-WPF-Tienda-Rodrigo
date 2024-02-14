using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tienda_Rodrigo.Views
{
    /// <summary>
    /// Lógica de interacción para Productos.xaml
    /// </summary>
    public partial class Productos : UserControl
    {
        private readonly string connectionString;
        public Productos()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["conexionDB"].ConnectionString;
            CargarDatos();
        }

        private void CargarDatos()
        {
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT IdArticulos, Nombre, Grupo, Codigo, Precio, UnidadMedida, Descripcion " +
                                   "FROM articulos " +
                                   "ORDER BY IdArticulos ASC";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
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


        #region Create
        private void AgregarProducto(object sender, RoutedEventArgs e)
        {
            CRUDProductos ventana = new CRUDProductos();
            FrameProductos.Content = ventana;
            Contenido.Visibility = Visibility.Hidden;
            ventana.BtnCrear.Visibility = Visibility.Visible;
        }

        #endregion

        #region Buscando
        private void Buscando(object sender, TextChangedEventArgs e)
        {
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                
                try
                {
                    con.Open();
                    string nombreProducto = ((TextBox)sender).Text;
                    string query = "SELECT IdArticulos, Nombre, Grupo, Codigo, Precio, UnidadMedida, Descripcion " +
                                   "FROM articulos " +
                                   "WHERE Nombre LIKE @Nombre OR Codigo LIKE @Nombre " +
                                   "ORDER BY IdArticulos ASC";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Nombre", "%" + nombreProducto + "%");
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


        #endregion

        #region Consult
        private void Consultar(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            CRUDProductos ventana = new CRUDProductos();
            ventana.IdArticulos = id;
            ventana.Consultar();
            FrameProductos.Content = ventana;
            Contenido.Visibility = Visibility.Hidden;
            ventana.Titulo.Text = "Consulta de Producto";
            ventana.tbNombre.IsEnabled = false;
            ventana.tbCodigo.IsEnabled = false;
            ventana.tbPrecio.IsEnabled = false;
            ventana.tbUnidadMedida.IsEnabled = false;
            ventana.tbDescripcion.IsEnabled = false;
            ventana.cbGrupo.IsEnabled = false;
            ventana.BtnSubir.IsEnabled = false;
        }


        #endregion

        #region Update
        private void Actualizar(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            CRUDProductos ventana = new CRUDProductos();
            ventana.IdArticulos = id;
            ventana.Consultar();
            FrameProductos.Content = ventana;
            Contenido.Visibility = Visibility.Hidden;
            ventana.Titulo.Text = "Actualización de Producto";
            ventana.tbNombre.IsEnabled = true;
            ventana.tbCodigo.IsEnabled = true;
            ventana.tbPrecio.IsEnabled = true;
            ventana.tbUnidadMedida.IsEnabled = true;
            ventana.tbDescripcion.IsEnabled = true;
            ventana.cbGrupo.IsEnabled = true;
            ventana.BtnSubir.IsEnabled = true;

            ventana.BtnModificar.Visibility = Visibility.Visible;
        }


        #endregion

        #region Delete
        private void Eliminar(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            CRUDProductos ventana = new CRUDProductos();
            ventana.IdArticulos = id;
            ventana.Consultar();
            FrameProductos.Content = ventana;
            Contenido.Visibility = Visibility.Hidden;
            ventana.Titulo.Text = "Eliminar Producto";
            ventana.tbNombre.IsEnabled = false;
            ventana.tbCodigo.IsEnabled = false;
            ventana.tbPrecio.IsEnabled = false;
            ventana.tbUnidadMedida.IsEnabled = false;
            ventana.tbDescripcion.IsEnabled = false;
            ventana.cbGrupo.IsEnabled = false;
            ventana.BtnSubir.IsEnabled = false;

            ventana.BtnEliminar.Visibility = Visibility.Visible;
        }
        #endregion
    }
}
