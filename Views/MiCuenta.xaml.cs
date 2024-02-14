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
    /// Lógica de interacción para MiCuenta.xaml
    /// </summary>
    public partial class MiCuenta : Window
    {
        public MiCuenta()
        {
            InitializeComponent();
        }

        public void CargarDatos(string usuario, int privilegio, string nombres, string apellidos, string correo)
        {
            lblnombre.Text += nombres;
            lblApellidos.Text += apellidos;
            lblCorreo.Text += correo;
            lblPrivilegio.Text += privilegio.ToString();
        }

        private void Cerrar(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
