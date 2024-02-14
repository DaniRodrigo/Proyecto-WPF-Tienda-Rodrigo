using Microsoft.Win32;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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
    /// Lógica de interacción para CRUDProductos.xaml
    /// </summary>
    public partial class CRUDProductos : Page
    {
        public CRUDProductos()
        {
            InitializeComponent();
            CargarCB();
        }

        #region Consultar

        public int IdArticulos;
        public void Consultar()
        {
            con.Open();
            MySqlCommand com = new MySqlCommand("SELECT articulos.*, grupo.Nombre AS NombreGrupo, articulos.Nombre AS NombreArticulo FROM articulos INNER JOIN grupo ON articulos.Grupo = grupo.idGrupo WHERE idArticulos = @idArticulos", con);
            com.Parameters.AddWithValue("@idArticulos", IdArticulos);

            MySqlDataReader rdr = com.ExecuteReader(System.Data.CommandBehavior.CloseConnection);

            if (rdr.Read())
            {
                this.tbNombre.Text = rdr["Nombre"].ToString();
                this.tbCodigo.Text = rdr["Codigo"].ToString();
                this.tbPrecio.Text = rdr["Precio"].ToString();
                this.tbUnidadMedida.Text = rdr["UnidadMedida"].ToString();
                this.tbDescripcion.Text = rdr["Descripcion"].ToString();
                this.cbGrupo.SelectedItem = rdr["NombreGrupo"].ToString();

                // Leer la imagen como un arreglo de bytes desde la base de datos
                byte[] imgData = rdr["Img"] as byte[];

                if (imgData != null && imgData.Length > 0)
                {
                    // Convertir el arreglo de bytes en una imagen y mostrarla en tu control de imagen
                    using (MemoryStream stream = new MemoryStream(imgData))
                    {
                        BitmapImage imgSource = new BitmapImage();
                        imgSource.BeginInit();
                        imgSource.StreamSource = stream;
                        imgSource.CacheOption = BitmapCacheOption.OnLoad;
                        imgSource.EndInit();
                        // Asigna la imagen al control de imagen (cambia 'imagen' por el nombre de tu control de imagen)
                        imagen.Source = imgSource;
                    }
                }
            }
            else
            {
                // Si no se encontró el artículo, podrías mostrar un mensaje o realizar alguna otra acción
                MessageBox.Show("Artículo no encontrado");
            }

            rdr.Close();
            con.Close();
        }



        #endregion


        #region Regresar
        private void Regresar(object sender, RoutedEventArgs e)
        {
            Content = new Productos();
        }
        #endregion

        readonly MySqlConnector.MySqlConnection con = new MySqlConnector.MySqlConnection(ConfigurationManager.ConnectionStrings["conexionDB"].ConnectionString);
        

        void CargarCB()
        {
            con.Open();
            MySqlCommand cmd = new MySqlCommand("select Nombre from Grupo", con);
            MySqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                cbGrupo.Items.Add(dr["Nombre"].ToString());
            }
            con.Close();
        }

        #region Create
        private void Crear(object sender, RoutedEventArgs e)
        {
            // Validar que los campos no estén vacíos
            if (string.IsNullOrWhiteSpace(tbNombre.Text) || string.IsNullOrWhiteSpace(tbCodigo.Text) ||
                string.IsNullOrWhiteSpace(tbPrecio.Text) || string.IsNullOrWhiteSpace(tbUnidadMedida.Text) ||
                string.IsNullOrWhiteSpace(tbDescripcion.Text))
            {
                MessageBox.Show("Ningún campo debe quedar vacío");
                return;
            }

            // Validar que Precio sea un número válido
            if (!float.TryParse(tbPrecio.Text, out float precio))
            {
                MessageBox.Show("El valor ingresado en el campo Precio no es un número válido.");
                return;
            }


            con.Open();

            // Validar que el Grupo seleccionado sea válido
            MySqlCommand cmd = new MySqlCommand("SELECT IdGrupo FROM grupo WHERE Nombre=@nombreGrupo", con);
            cmd.Parameters.AddWithValue("@nombreGrupo", cbGrupo.Text);
            object valorGrupo = cmd.ExecuteScalar();
            if (valorGrupo == null)
            {
                MessageBox.Show("El grupo seleccionado no es válido.");
                con.Close();
                return;
            }
            int grupo = Convert.ToInt32(valorGrupo);

            if (imagensubida == true)
            {
                MySqlCommand com = new MySqlCommand("INSERT INTO articulos (nombre, grupo, codigo, precio, unidadMedida, img, descripcion) VALUES (@nombre, @grupo, @codigo, @precio, @unidadMedida, @img, @descripcion)", con);
                com.Parameters.Add("@nombre", MySqlDbType.VarChar).Value = tbNombre.Text;
                com.Parameters.Add("@grupo", MySqlDbType.Int32).Value = grupo;
                com.Parameters.Add("@codigo", MySqlDbType.VarChar).Value = tbCodigo.Text;
                com.Parameters.Add("@precio", MySqlDbType.Double).Value = precio;
                com.Parameters.Add("@unidadMedida", MySqlDbType.VarChar).Value = tbUnidadMedida.Text;
                com.Parameters.Add("@descripcion", MySqlDbType.VarChar).Value = tbDescripcion.Text;
                com.Parameters.AddWithValue("@img", MySqlDbType.VarBinary).Value = data;
                com.ExecuteNonQuery();
                Content = new Productos();
            }
            else
            {
                MessageBox.Show("Debe agregar una foto del artículo para crear el producto");
            }
            con.Close();
        }



        #endregion

        #region Delete
        private void Eliminar(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("¿Está seguro de que desea eliminar este producto?", "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand("DELETE FROM articulos WHERE IdArticulos = @IdArticulos", con);
                    cmd.Parameters.AddWithValue("@IdArticulos", IdArticulos);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Producto eliminado correctamente");
                        Content = new Productos();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo eliminar el producto");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar el producto: " + ex.Message);
                }
                finally
                {
                    con.Close();
                }
            }
        }
        #endregion

        #region Update
        private void Modificar(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbNombre.Text) || string.IsNullOrEmpty(tbCodigo.Text) || string.IsNullOrEmpty(tbPrecio.Text) || string.IsNullOrEmpty(tbUnidadMedida.Text) || string.IsNullOrEmpty(tbDescripcion.Text))
            {
                MessageBox.Show("Ningún campo debe quedar vacío");
                return;
            }

            con.Open();

            try
            {
                MySqlCommand cmdGrupo = new MySqlCommand("SELECT IdGrupo FROM grupo WHERE Nombre=@nombreGrupo", con);
                cmdGrupo.Parameters.AddWithValue("@nombreGrupo", cbGrupo.Text);
                object valorGrupo = cmdGrupo.ExecuteScalar();
                if (valorGrupo == null)
                {
                    MessageBox.Show("El grupo seleccionado no es válido.");
                    return;
                }
                int grupo = Convert.ToInt32(valorGrupo);

                MySqlCommand cmd;
                if (nuevaImagenSubida)
                {
                    cmd = new MySqlCommand("UPDATE articulos SET Nombre = @Nombre, Grupo = @Grupo, Codigo = @Codigo, Precio = @Precio, UnidadMedida = @UnidadMedida, Descripcion = @Descripcion, Img = @Img WHERE IdArticulos = @IdArticulos", con);
                    cmd.Parameters.AddWithValue("@Img", data);
                }
                else
                {
                    cmd = new MySqlCommand("UPDATE articulos SET Nombre = @Nombre, Grupo = @Grupo, Codigo = @Codigo, Precio = @Precio, UnidadMedida = @UnidadMedida, Descripcion = @Descripcion WHERE IdArticulos = @IdArticulos", con);
                }

                cmd.Parameters.AddWithValue("@Nombre", tbNombre.Text);
                cmd.Parameters.AddWithValue("@Grupo", grupo);
                cmd.Parameters.AddWithValue("@Codigo", tbCodigo.Text);

                // Manejo de la excepción al convertir el precio a double y formateo del precio
                try
                {
                    // Formatear el precio con dos decimales y convertirlo a double
                    double precio = double.Parse(tbPrecio.Text) / 100; // Dividir por 100 para obtener el valor correcto
                    cmd.Parameters.AddWithValue("@Precio", precio);
                }
                catch (FormatException)
                {
                    MessageBox.Show("El formato del precio es incorrecto. Debe ser un número decimal válido.");
                    return;
                }

                cmd.Parameters.AddWithValue("@UnidadMedida", tbUnidadMedida.Text);
                cmd.Parameters.AddWithValue("@Descripcion", tbDescripcion.Text);
                cmd.Parameters.AddWithValue("@IdArticulos", IdArticulos);

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Producto actualizado correctamente");
                    Content = new Productos();
                }
                else
                {
                    MessageBox.Show("No se pudo actualizar el producto");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar el producto: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }




        #endregion

        #region IMAGEN

        byte[] data;
        private bool imagensubida = false;
        private bool nuevaImagenSubida = false;

        private void Subir(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                FileStream fs = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read);
                data = new byte[fs.Length];
                fs.Read(data, 0, System.Convert.ToInt32(fs.Length));
                fs.Close();
                ImageSourceConverter imgs = new ImageSourceConverter();
                imagen.SetValue(Image.SourceProperty, imgs.ConvertFromString(ofd.FileName.ToString()));
                nuevaImagenSubida = true; // Establecer la bandera de nueva imagen subida como verdadera
            }
            imagensubida = true;
        }

        #endregion
    }
}
