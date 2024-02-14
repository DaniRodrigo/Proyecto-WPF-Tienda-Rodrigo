using Microsoft.Win32;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
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
    /// Lógica de interacción para CRUDUsuarios.xaml
    /// </summary>
    public partial class CRUDUsuarios : Page
    {
        public CRUDUsuarios()
        {
            InitializeComponent();
            CargarCB();
        }

        private void Regresar (object sender, RoutedEventArgs e)
        {
            Content = new Usuarios();
        }

        readonly MySqlConnector.MySqlConnection con = new MySqlConnector.MySqlConnection(ConfigurationManager.ConnectionStrings["conexionDB"].ConnectionString);
        void CargarCB()
        {
            con.Open();
            MySqlCommand cmd = new MySqlCommand("select NombrePrivilegio from Privilegios", con);
            MySqlDataReader dr = cmd.ExecuteReader();
            while(dr.Read()) 
            {
                cbPrivilegio.Items.Add(dr["NombrePrivilegio"].ToString());
            }
            con.Close();
        }
        #region CRUD
        #region CREATE
        private void Crear(object sender, RoutedEventArgs e)
        {
            // Validar que los campos no estén vacíos
            if (string.IsNullOrWhiteSpace(tbNombres.Text) || string.IsNullOrWhiteSpace(tbApellidos.Text) ||
                string.IsNullOrWhiteSpace(tbDUI.Text) || string.IsNullOrWhiteSpace(tbNIT.Text) ||
                string.IsNullOrWhiteSpace(tbCorreo.Text) || string.IsNullOrWhiteSpace(tbTelefono.Text) ||
                string.IsNullOrWhiteSpace(tbFecha.Text) || string.IsNullOrWhiteSpace(cbPrivilegio.Text) ||
                string.IsNullOrWhiteSpace(tbUsuario.Text) || string.IsNullOrWhiteSpace(tbContrasenia.Text))
            {
                MessageBox.Show("Ningún campo debe quedar vacío");
                return;
            }

            // Validar que DUI sea un número válido
            if (!int.TryParse(tbDUI.Text, out int dui))
            {
                MessageBox.Show("El valor ingresado en el campo DUI no es un número válido.");
                return;
            }

            // Validar que NIT sea un número válido
            if (!int.TryParse(tbNIT.Text, out int nit))
            {
                MessageBox.Show("El valor ingresado en el campo NIT no es un número válido.");
                return;
            }

            // Validar que teléfono sea un número válido
            if (!int.TryParse(tbTelefono.Text, out int telefono))
            {
                MessageBox.Show("El valor ingresado en el campo Teléfono no es un número válido.");
                return;
            }

            // Validar que la fecha sea válida
            if (!DateTime.TryParse(tbFecha.Text, out _))
            {
                MessageBox.Show("El valor ingresado en el campo Fecha de Nacimiento no es una fecha válida.");
                return;
            }

            con.Open();
            MySqlCommand cmd = new MySqlCommand("select IdPrivilegio from privilegios where NombrePrivilegio='" + cbPrivilegio.Text + "'", con);
            object valor = cmd.ExecuteScalar();
            int privilegio = (int)valor;

            if (imagensubida == true)
            {
                MySqlCommand com = new MySqlCommand("INSERT INTO usuarios (nombres, apellidos, DUI, NIT, fecha_nac, telefono, correo, privilegio, img, usuario, contrasenia) VALUES (@nombres, @apellidos, @DUI, @NIT, @fecha_nac, @telefono, @correo, @privilegio, @img, @usuario, AES_ENCRYPT(@contrasenia, 'patron'))", con);
                com.Parameters.Add("@nombres", MySqlDbType.VarChar).Value = tbNombres.Text;
                com.Parameters.Add("@apellidos", MySqlDbType.VarChar).Value = tbApellidos.Text;
                com.Parameters.Add("@DUI", MySqlDbType.Int32).Value = dui;
                com.Parameters.Add("@NIT", MySqlDbType.Int32).Value = nit;
                com.Parameters.Add("@fecha_nac", MySqlDbType.Date).Value = DateTime.Parse(tbFecha.Text);
                com.Parameters.Add("@telefono", MySqlDbType.Int32).Value = telefono;
                com.Parameters.Add("@correo", MySqlDbType.VarChar).Value = tbCorreo.Text;
                com.Parameters.Add("@privilegio", MySqlDbType.Int32).Value = privilegio;
                com.Parameters.Add("@usuario", MySqlDbType.VarChar).Value = tbUsuario.Text;
                com.Parameters.Add("@contrasenia", MySqlDbType.VarChar).Value = tbContrasenia.Text;
                com.Parameters.AddWithValue("@img", MySqlDbType.VarBinary).Value = data;
                com.ExecuteNonQuery();
                Content = new Usuarios();
            }
            else
            {
                MessageBox.Show("Debe agregar una foto de perfil para crear el usuario");
            }
            con.Close();
        }


        #endregion
        #region CONSULTAR

        public MySqlConnection GetConnection()
        {
            string connectionString = "server=myServerAddress;user=myUsername;password=myPassword;database=myDatabase;AllowZeroDateTime=True;";
            MySqlConnection con = new MySqlConnection(connectionString);
            return con;
        }

        public int IdUsuario;
        public void Consultar()
        {
            con.Open();
            MySqlCommand com = new MySqlCommand("select * from Usuarios inner join Privilegios on Usuarios.Privilegio = Privilegios.IdPrivilegio where IdUsuario =" + IdUsuario, con);
            MySqlDataReader rdr = com.ExecuteReader(System.Data.CommandBehavior.CloseConnection);

            if (rdr.Read())
            {
                this.tbNombres.Text = rdr["Nombres"].ToString();
                this.tbApellidos.Text = rdr["Apellidos"].ToString();
                this.tbDUI.Text = rdr["DUI"].ToString();
                this.tbNIT.Text = rdr["NIT"].ToString();

                // Obtener la fecha directamente como DateTime
                if (rdr["Fecha_nac"] != DBNull.Value)
                {
                    DateTime fechaNacimiento = rdr.GetDateTime(rdr.GetOrdinal("Fecha_nac"));
                    this.tbFecha.Text = fechaNacimiento.ToString("yyyy/MM/dd");
                }
                else
                {
                    this.tbFecha.Text = string.Empty;
                }

                this.tbTelefono.Text = rdr["Telefono"].ToString();
                this.tbCorreo.Text = rdr["Correo"].ToString();
                this.cbPrivilegio.SelectedItem = rdr["NombrePrivilegio"];
                this.tbUsuario.Text = rdr["Usuario"].ToString();
                

                // Leer la imagen como un arreglo de bytes desde la base de datos
                byte[] imgData = rdr["img"] as byte[];

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
                        imagen.Source = imgSource; // Asigna la imagen al control de imagen
                    }
                }
            }
            else
            {
                // Si no se encontró el usuario, podrías mostrar un mensaje o realizar alguna otra acción
                MessageBox.Show("Usuario no encontrado");
            }

            rdr.Close();
            con.Close();
        }



        #endregion
        #region MODIFICAR
        private void Modificar(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbNombres.Text) || string.IsNullOrEmpty(tbApellidos.Text) || string.IsNullOrEmpty(tbDUI.Text) || string.IsNullOrEmpty(tbNIT.Text) || string.IsNullOrEmpty(tbCorreo.Text) || string.IsNullOrEmpty(tbTelefono.Text) || string.IsNullOrEmpty(tbFecha.Text) || string.IsNullOrEmpty(cbPrivilegio.Text) || string.IsNullOrEmpty(tbUsuario.Text))
            {
                MessageBox.Show("Ningún campo debe quedar vacío");
                return;
            }

            con.Open();

            // Obtener el ID del privilegio seleccionado
            MySqlCommand cmdPrivilegio = new MySqlCommand("SELECT IdPrivilegio FROM Privilegios WHERE NombrePrivilegio = @NombrePrivilegio", con);
            cmdPrivilegio.Parameters.AddWithValue("@NombrePrivilegio", cbPrivilegio.Text);
            object privilegioId = cmdPrivilegio.ExecuteScalar();
            if (privilegioId == null)
            {
                MessageBox.Show("Privilegio no válido");
                con.Close();
                return;
            }

            int idPrivilegio = Convert.ToInt32(privilegioId);

            // Actualizar el usuario
            MySqlCommand cmd;

            if (nuevaImagenSubida)
            {
                // Actualizar el usuario con la nueva imagen
                cmd = new MySqlCommand("UPDATE usuarios SET Nombres = @Nombres, Apellidos = @Apellidos, DUI = @DUI, NIT = @NIT, fecha_nac = @FechaNacimiento, telefono = @Telefono, correo = @Correo, privilegio = @Privilegio, img = @Img", con);
                cmd.Parameters.AddWithValue("@Img", data); // Aquí debes pasar el nuevo arreglo de bytes de la imagen
            }
            else
            {
                // Si no se subió una nueva imagen, actualizar el usuario sin modificar la imagen
                cmd = new MySqlCommand("UPDATE usuarios SET Nombres = @Nombres, Apellidos = @Apellidos, DUI = @DUI, NIT = @NIT, fecha_nac = @FechaNacimiento, telefono = @Telefono, correo = @Correo, privilegio = @Privilegio", con);
            }

            // Agregar contraseña a la consulta solo si no está vacía
            if (!string.IsNullOrEmpty(tbContrasenia.Text))
            {
                cmd.CommandText += ", contrasenia = @Contrasenia";
                cmd.Parameters.AddWithValue("@Contrasenia", tbContrasenia.Text);
            }

            cmd.CommandText += " WHERE IdUsuario = @IdUsuario";

            cmd.Parameters.AddWithValue("@Nombres", tbNombres.Text);
            cmd.Parameters.AddWithValue("@Apellidos", tbApellidos.Text);
            cmd.Parameters.AddWithValue("@DUI", tbDUI.Text);
            cmd.Parameters.AddWithValue("@NIT", tbNIT.Text);
            cmd.Parameters.AddWithValue("@FechaNacimiento", Convert.ToDateTime(tbFecha.Text));
            cmd.Parameters.AddWithValue("@Telefono", tbTelefono.Text);
            cmd.Parameters.AddWithValue("@Correo", tbCorreo.Text);
            cmd.Parameters.AddWithValue("@Privilegio", idPrivilegio);
            cmd.Parameters.AddWithValue("@IdUsuario", IdUsuario);

            try
            {
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Usuario actualizado correctamente");
                    Content = new Usuarios();
                }
                else
                {
                    MessageBox.Show("No se pudo actualizar el usuario");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar el usuario: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }








        #endregion
        #region ELIMINAR
        private void Eliminar(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("¿Está seguro de que desea eliminar este usuario?", "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand("DELETE FROM usuarios WHERE IdUsuario = @IdUsuario", con);
                    cmd.Parameters.AddWithValue("@IdUsuario", IdUsuario);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Usuario eliminado correctamente");
                        Content = new Usuarios();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo eliminar el usuario");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar el usuario: " + ex.Message);
                }
                finally
                {
                    con.Close();
                }
            }
        }

        #endregion
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
