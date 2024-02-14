using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Tienda_Rodrigo.Views
{
    /// <summary>
    /// Lógica de interacción para LogIn.xaml
    /// </summary>
    public partial class LogIn : Window
    {
        public LogIn()
        {
            InitializeComponent();
            tbusuario.Focus();
        }

        private void Acceder(object sender, RoutedEventArgs e)
        {
            if (tbusuario.Text != "" && tbcontra.Text != "")
            {
                Usuarios usuarios = new Usuarios(); // Crear una instancia de la clase Usuarios
                if (usuarios.LogIn(tbusuario.Text, tbcontra.Text)) // Llamar al método LogIn en la instancia creada
                {
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Credenciales incorrectas");
                }
            }
            else
            {
                MessageBox.Show("Los campos no pueden quedar vacíos");
            }
        }

        private void Cerrar(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
