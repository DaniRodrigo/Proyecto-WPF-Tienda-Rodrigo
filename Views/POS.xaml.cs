using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Lógica de interacción para POS.xaml
    /// </summary>
    public partial class POS : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<Producto> _productos;
        public ObservableCollection<Producto> Productos
        {
            get { return _productos; }
            set
            {
                _productos = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Productos)));
            }
        }

        public POS()
        {
            InitializeComponent();
            // Inicializamos la lista de productos
            Productos = new ObservableCollection<Producto>
            {
                new Producto { Codigo = 1, Nombre = "Dragón", Precio = 9.5m, Cantidad = 10 },
                new Producto { Codigo = 2, Nombre = "Tataki de atún", Precio = 14.75m, Cantidad = 15 },
                new Producto { Codigo = 3, Nombre = "Pollo Katsu", Precio = 10.0m, Cantidad = 20 }
                // Agrega más productos según sea necesario
            };
        }

        // Clase Producto encapsulada dentro de la clase POS
        public class Producto : INotifyPropertyChanged
        {
            public int Codigo { get; set; }
            public string Nombre { get; set; }
            public decimal Precio { get; set; }
            public int Cantidad { get; set; }
            public decimal PrecioTotal { get { return Precio * Cantidad; } }

            public event PropertyChangedEventHandler PropertyChanged;

            private decimal _efectivoIntroducido;
            public decimal EfectivoIntroducido
            {
                get { return _efectivoIntroducido; }
                set
                {
                    _efectivoIntroducido = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EfectivoIntroducido)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Cambio)));
                }
            }

            public decimal Cambio
            {
                get
                {
                    decimal total = PrecioTotal;
                    if (EfectivoIntroducido >= total)
                        return EfectivoIntroducido - total;
                    else
                        return 0;
                }
            }
        }

        // Método para buscar productos por nombre o código
        private void BuscarProducto(object sender, RoutedEventArgs e)
        {
            // Obtenemos el término de búsqueda del TextBox
            string terminoBusqueda = tbbuscar.Text.Trim();

            // Validamos que el término de búsqueda no esté vacío
            if (string.IsNullOrEmpty(terminoBusqueda))
            {
                MessageBox.Show("Por favor ingrese un término de búsqueda válido.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Realizamos la búsqueda en la lista de productos
            // Puedes reemplazar esta lógica con la que se ajuste a tu implementación
            var resultados = Productos.Where(p => p.Nombre.ToLower().Contains(terminoBusqueda.ToLower()) || p.Codigo.ToString() == terminoBusqueda).ToList();

            // Actualizamos el DataGrid con los resultados de la búsqueda
            GridProductos.ItemsSource = resultados;
        }

        private void EliminarProducto(object sender, RoutedEventArgs e)
        {
            Producto productoSeleccionado = GridProductos.SelectedItem as Producto;

            if (productoSeleccionado == null)
            {
                MessageBox.Show("Por favor, seleccione un producto para eliminar de la cesta.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ProductosCesta.Remove(productoSeleccionado);

            ActualizarPrecioTotalCesta();
        }

        private ObservableCollection<Producto> ProductosCesta { get; } = new ObservableCollection<Producto>();

        private void AgregarProducto(object sender, RoutedEventArgs e)
        {
            Producto productoSeleccionado = GridProductos.SelectedItem as Producto;

            if (productoSeleccionado == null)
            {
                MessageBox.Show("Por favor, seleccione un producto para agregar a la cesta.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ProductosCesta.Add(productoSeleccionado);

            ActualizarPrecioTotalCesta();
        }

        private void ActualizarPrecioTotalCesta()
        {
            decimal precioTotalCesta = ProductosCesta.Sum(p => p.PrecioTotal);
            lbltotal.Content = $"Total: € {precioTotalCesta}";
        }

        private void Efectivo(object sender, RoutedEventArgs e)
        {
            if (ProductosCesta.Count == 0)
            {
                MessageBox.Show("La cesta está vacía. Por favor, agregue productos primero.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Obtener el total de la cesta
            decimal totalCesta = CalcularTotalCesta();

            // Obtener la cantidad introducida en efectivo
            if (!decimal.TryParse(TbPrecio.Text, out decimal efectivo))
            {
                MessageBox.Show("Por favor, introduzca una cantidad válida en el campo de Efectivo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Calcular el cambio
            decimal cambio = efectivo - totalCesta;

            // Actualizar los labels
            lbltotal.Content = $"Total: €{totalCesta}";
            lblefectivo.Content = $"Efectivo: €{efectivo}";
            lblcambio.Content = $"Cambio: €{cambio}";
        }

        private void ActualizarLabels()
        {
            // Obtener el total de la cesta
            decimal totalCesta = CalcularTotalCesta();

            // Obtener la cantidad introducida en efectivo
            if (!decimal.TryParse(TbPrecio.Text, out decimal efectivo))
            {
                MessageBox.Show("Por favor, introduzca una cantidad válida en el campo de Efectivo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Calcular el cambio
            decimal cambio = efectivo - totalCesta;

            // Actualizar los labels
            lbltotal.Content = $"Total: €{totalCesta}";
            lblefectivo.Content = $"Efectivo: €{efectivo}";
            lblcambio.Content = $"Cambio: €{cambio}";
        }


        private void AnularOrden(object sender, RoutedEventArgs e)
        {
            if (ProductosCesta.Count > 0)
            {
                MessageBoxResult result = MessageBox.Show("¿Está seguro de que desea anular la orden y vaciar la cesta?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    ProductosCesta.Clear();
                    ActualizarPrecioTotalCesta();
                    LimpiarEfectivoYTotal();
                }
            }
            else
            {
                MessageBox.Show("La cesta ya está vacía.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Pagar(object sender, RoutedEventArgs e)
        {
            decimal totalCesta = ProductosCesta.Sum(p => p.PrecioTotal);

            if (totalCesta <= 0)
            {
                MessageBox.Show("No hay productos en la cesta para pagar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal efectivoIntroducido = 0;

            if (!decimal.TryParse(TbPrecio.Text, out efectivoIntroducido))
            {
                MessageBox.Show("Por favor, introduzca una cantidad válida en el campo de Efectivo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (efectivoIntroducido < totalCesta)
            {
                MessageBox.Show("El efectivo introducido es insuficiente.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal cambio = efectivoIntroducido - totalCesta;
            MessageBox.Show($"Se ha pagado un total de €{totalCesta}. El cambio es de €{cambio}. Tras haber pagado €{efectivoIntroducido} ¡Gracias por su compra!", "Pago exitoso", MessageBoxButton.OK, MessageBoxImage.Information);
            ProductosCesta.Clear();
            ActualizarPrecioTotalCesta();
            LimpiarEfectivoYTotal(); // Aquí llamamos al método para limpiar los valores
        }

        private void LimpiarEfectivoYTotal()
        {
            TbPrecio.Text = "";
            lbltotal.Content = "Total: €0.00";
            lblefectivo.Content = "Efectivo: €0.00"; // Limpia el label de Efectivo
            lblcambio.Content = "Cambio: €0.00"; // Limpia el label de Cambio
        }

        private void MostrarCesta(object sender, RoutedEventArgs e)
        {
            // Limpiamos el DataGrid antes de mostrar la cesta para evitar duplicados
            GridProductos.ItemsSource = null;

            // Asignamos la colección de productos de la cesta al DataGrid
            GridProductos.ItemsSource = ProductosCesta;
        }

        private decimal CalcularTotalCesta()
        {
            decimal total = 0;
            foreach (var producto in ProductosCesta)
            {
                total += producto.PrecioTotal;
            }
            return total;
        }
    }
}
