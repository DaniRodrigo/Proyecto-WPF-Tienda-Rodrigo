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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tienda_Rodrigo.SCS;
using Tienda_Rodrigo.Views;

namespace Tienda_Rodrigo
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

          
        }

        private void TBShow(object sender, RoutedEventArgs e)
        {
            GridContent.Opacity = 0.8;
        }

        private void TBHide(object sender, RoutedEventArgs e)
        {
            GridContent.Opacity = 1;
        }

        private void PreviewMouseLeftBottonDownBG(object sender, MouseButtonEventArgs e)
        {
            BtnShowHide.IsChecked = false;
        }

        private void Minimizar(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Cerrar(object sender, RoutedEventArgs e)
        {
            LogIn lg = new LogIn();
            lg.Show();
            this.Close();
        }

        private void Usuarios(object sender, RoutedEventArgs e)
        {
            DataContext = new Usuarios();
        }


        private void Productos(object sender, RoutedEventArgs e)
        {
            DataContext = new Productos();
        }

        private void Dashboard(object sender, RoutedEventArgs e)
        {
            DataContext = new Dashboard();
        }

        private void POS(object sender, RoutedEventArgs e)
        {
            DataContext = new POS();
        }

        private void MiCuenta(object sender, RoutedEventArgs e)
        {
            MiCuenta mc = new MiCuenta();   
            mc.ShowDialog();
        }

        private void AcercaDe(object sender, RoutedEventArgs e)
        {
            AcercaDe ac = new AcercaDe();
            ac.ShowDialog();
        }

        #region MOVER VENTANA

        private void Mover(Border header)
        {
            var restaurar = false;

            header.MouseLeftButtonDown += (s, e) =>
            {
                if (e.ClickCount == 2)
                {
                    if ((ResizeMode == ResizeMode.CanResize) || (ResizeMode == ResizeMode.CanResizeWithGrip))
                    {
                        CambiarEstado();
                    }
                }
                else
                {
                    if (WindowState == WindowState.Maximized)
                    {
                        restaurar = true;
                    }
                    DragMove();
                }
            };

            header.MouseLeftButtonUp += (s, e) => 
            {
                restaurar = false;
            };

            header.MouseMove -= (s, e) =>
            {
                if (restaurar)
                {
                    try
                    {
                        restaurar = false;
                        var mouseX = e.GetPosition(this).X;
                        var width = RestoreBounds.Width;
                        var x = mouseX - width / 2;

                        if(x< 0)
                        {
                            x = 0;
                        }
                        else if(x+width > SystemParameters.PrimaryScreenWidth)
                        {
                            x = SystemParameters.PrimaryScreenWidth - width;
                        }

                        WindowState = WindowState.Normal;
                        Left = x;
                        Top = 0;
                        DragMove();
                    }
                    catch (System.Exception)
                    {
                        throw;
                    }
                }
            };
        }

        private void CambiarEstado()
        {
            switch (WindowState)
            {
                case WindowState.Normal:
                    {
                        WindowState = WindowState.Maximized;
                        break;
                    }
                case WindowState.Maximized:
                    {
                        WindowState = WindowState.Normal;
                        break;
                    }
            }
        }
        private void RestaurarVentana(object sender, RoutedEventArgs e)
        {
            Mover(sender as Border);
        }

        #endregion
        
        
    }
}
