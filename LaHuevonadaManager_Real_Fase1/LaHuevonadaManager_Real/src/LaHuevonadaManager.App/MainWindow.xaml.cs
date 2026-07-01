using System.Windows;
using Microsoft.Win32;
using LaHuevonadaManager.Models;
using LaHuevonadaManager.Services;

namespace LaHuevonadaManager;

public partial class MainWindow : Window
{
    private readonly LahuevonadaFileService _fileService = new();
    private AppData _data = new();
    private string? _currentPath;

    public MainWindow()
    {
        InitializeComponent();
        BindAll();
    }

    private void BindAll()
    {
        SalesGrid.ItemsSource = _data.Sales;
        CustomersGrid.ItemsSource = _data.Customers;
        ProductionGrid.ItemsSource = _data.Productions;
        ExpensesGrid.ItemsSource = _data.Expenses;
        RefreshDashboard();
    }

    private void RefreshDashboard()
    {
        var sales = _data.Sales.Sum(s => s.Total);
        var pending = _data.Sales.Sum(s => s.Pending);
        var expenses = _data.Expenses.Sum(e => e.Amount);
        StockText.Text = $"{_data.CurrentEggStock:N0} huevos";
        SalesText.Text = $"{sales:C0}";
        PendingText.Text = $"{pending:C0}";
        ProfitText.Text = $"{sales - expenses:C0}";
    }

    private void New_Click(object sender, RoutedEventArgs e)
    {
        _data = new AppData();
        _currentPath = null;
        BindAll();
    }

    private void Open_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog { Filter = "Archivos LaHuevonada (*.lahuevonada)|*.lahuevonada" };
        if (dialog.ShowDialog() != true) return;
        _data = _fileService.Load(dialog.FileName);
        _currentPath = dialog.FileName;
        BindAll();
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (_currentPath is null)
        {
            var dialog = new SaveFileDialog { Filter = "Archivos LaHuevonada (*.lahuevonada)|*.lahuevonada", FileName = "MiGranja.lahuevonada" };
            if (dialog.ShowDialog() != true) return;
            _currentPath = dialog.FileName;
        }
        _fileService.Save(_currentPath, _data);
        MessageBox.Show("Archivo guardado correctamente.", "LaHuevonada Manager", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void AddCustomer_Click(object sender, RoutedEventArgs e)
    {
        var name = SimpleInput.Show("Nuevo cliente", "Nombre del cliente:");
        if (string.IsNullOrWhiteSpace(name)) return;
        _data.Customers.Add(new Customer { Name = name.Trim() });
        RefreshDashboard();
    }

    private void AddSale_Click(object sender, RoutedEventArgs e)
    {
        var customer = SimpleInput.Show("Nueva venta", "Cliente:");
        if (string.IsNullOrWhiteSpace(customer)) return;
        var pacas = SimpleInput.ShowInt("Nueva venta", "Cantidad de pacas:");
        var price = SimpleInput.ShowDecimal("Nueva venta", "Precio por paca:");
        var paid = SimpleInput.ShowDecimal("Nueva venta", "Valor pagado ahora:");
        var sale = new Sale { CustomerName = customer.Trim(), Pacas = pacas, UnitPrice = price, PaidAmount = paid };
        sale.Status = sale.Pending <= 0 ? SaleStatus.Pagada : paid <= 0 ? SaleStatus.Pendiente : SaleStatus.Parcial;
        _data.Sales.Add(sale);
        _data.CurrentEggStock = Math.Max(0, _data.CurrentEggStock - pacas * _data.EggsPerPaca);
        RefreshDashboard();
    }

    private void AddProduction_Click(object sender, RoutedEventArgs e)
    {
        var produced = SimpleInput.ShowInt("Producción semanal", "Huevos producidos:");
        var damaged = SimpleInput.ShowInt("Producción semanal", "Huevos dañados:");
        var production = new WeeklyProduction { WeekStart = DateTime.Today, EggsProduced = produced, DamagedEggs = damaged };
        _data.Productions.Add(production);
        _data.CurrentEggStock += production.NetEggs;
        RefreshDashboard();
    }

    private void AddExpense_Click(object sender, RoutedEventArgs e)
    {
        var category = SimpleInput.Show("Nuevo gasto", "Categoría:");
        if (string.IsNullOrWhiteSpace(category)) return;
        var amount = SimpleInput.ShowDecimal("Nuevo gasto", "Valor:");
        _data.Expenses.Add(new Expense { Category = category.Trim(), Amount = amount, Description = category.Trim() });
        RefreshDashboard();
    }
}
