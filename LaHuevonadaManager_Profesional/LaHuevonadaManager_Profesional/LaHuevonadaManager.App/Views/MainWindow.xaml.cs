using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using LaHuevonadaManager.Models;
using LaHuevonadaManager.Services;

namespace LaHuevonadaManager.Views;

public partial class MainWindow : Window
{
    private readonly FarmDocumentService _service = new();
    private FarmDocument D => AppState.Document;

    public MainWindow()
    {
        InitializeComponent();
        ShowDashboard();
    }

    private Border Card(UIElement child) => new() { Style = (Style)FindResource("Card"), Child = child };
    private TextBlock Text(string value, int size = 14, bool bold = false) => new() { Text = value, FontSize = size, FontWeight = bold ? FontWeights.Bold : FontWeights.Normal, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(2) };
    private TextBox Box(string placeholder = "") => new() { MinHeight = 36, Margin = new Thickness(4), ToolTip = placeholder };
    private void SetHeader(string title, string subtitle) { TitleText.Text = title; SubtitleText.Text = subtitle; ContentArea.Children.Clear(); }

    private async Task AutoSave()
    {
        if (!string.IsNullOrWhiteSpace(AppState.CurrentFilePath))
            await _service.SaveAsync(AppState.CurrentFilePath, D);
    }

    private async void Save_Click(object sender, RoutedEventArgs e) { await AutoSave(); MessageBox.Show("Datos guardados correctamente."); }

    private void Dashboard_Click(object s, RoutedEventArgs e) => ShowDashboard();
    private void Production_Click(object s, RoutedEventArgs e) => ShowProduction();
    private void Sales_Click(object s, RoutedEventArgs e) => ShowSales();
    private void Customers_Click(object s, RoutedEventArgs e) => ShowCustomers();
    private void Expenses_Click(object s, RoutedEventArgs e) => ShowExpenses();
    private void Inventory_Click(object s, RoutedEventArgs e) => ShowInventory();
    private void Reports_Click(object s, RoutedEventArgs e) => ShowReports();
    private void Backup_Click(object s, RoutedEventArgs e) => ShowBackup();

    private void ShowDashboard()
    {
        SetHeader("Dashboard", "Resumen automático de producción, ventas, gastos e inventario.");
        var sales = D.Sales.Sum(x => x.Total);
        var pending = D.Sales.Sum(x => x.Pending);
        var expenses = D.Expenses.Sum(x => x.Amount);
        var utility = sales - expenses;
        var grid = new UniformGrid { Columns = 4 };
        grid.Children.Add(Kpi("🥚 Inventario", $"{D.Inventory.AvailableEggs:N0} huevos", $"{D.Inventory.AvailablePacks:N0} pacas disponibles"));
        grid.Children.Add(Kpi("💰 Ventas", sales.ToString("C0"), $"Pendiente: {pending:C0}"));
        grid.Children.Add(Kpi("🧾 Gastos", expenses.ToString("C0"), "Costos acumulados"));
        grid.Children.Add(Kpi("📈 Utilidad", utility.ToString("C0"), utility >= 0 ? "Resultado positivo" : "Resultado negativo"));
        ContentArea.Children.Add(grid);
        var alerts = new StackPanel();
        alerts.Children.Add(Text("Alertas inteligentes", 22, true));
        alerts.Children.Add(Text(D.Inventory.AvailablePacks < 10 ? "⚠ Inventario bajo: quedan menos de 10 pacas." : "✅ Inventario estable."));
        alerts.Children.Add(Text(pending > 0 ? $"⚠ Hay cartera pendiente por {pending:C0}." : "✅ No hay deudas pendientes."));
        ContentArea.Children.Add(Card(alerts));
        AddList("Últimas ventas", D.Sales.OrderByDescending(x => x.Date).Take(8).Select(x => $"{x.Date:dd/MM/yyyy} - {x.CustomerName} - {x.Packs} pacas - {x.Total:C0} - {x.Status}"));
    }

    private Border Kpi(string title, string value, string sub)
    {
        var sp = new StackPanel();
        sp.Children.Add(Text(title, 15, true));
        sp.Children.Add(Text(value, 25, true));
        sp.Children.Add(Text(sub, 13));
        return Card(sp);
    }

    private void ShowProduction()
    {
        SetHeader("Producción semanal", "Registra el lote semanal. El inventario se actualiza automáticamente.");
        var eggs = Box("Huevos producidos"); var damaged = Box("Huevos dañados"); var notes = Box("Observaciones");
        var btn = new Button { Content = "Registrar producción", Style = (Style)FindResource("PrimaryButton"), Height = 44 };
        btn.Click += async (_, _) =>
        {
            if (!int.TryParse(eggs.Text, out var e) || e <= 0) { MessageBox.Show("Ingresa huevos producidos válidos."); return; }
            int.TryParse(damaged.Text, out var d);
            var lot = new WeeklyProductionLot { EggsProduced = e, DamagedEggs = Math.Max(0, d), Notes = notes.Text, WeekStart = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1) };
            D.ProductionLots.Add(lot);
            D.Inventory.AvailableEggs += lot.NetEggs;
            D.Inventory.DamagedEggsTotal += lot.DamagedEggs;
            await AutoSave(); ShowProduction();
        };
        var form = new StackPanel(); form.Children.Add(Text("Nuevo lote semanal", 22, true)); form.Children.Add(eggs); form.Children.Add(damaged); form.Children.Add(notes); form.Children.Add(btn); ContentArea.Children.Add(Card(form));
        AddList("Historial", D.ProductionLots.OrderByDescending(x => x.WeekStart).Select(x => $"Semana {x.WeekStart:dd/MM/yyyy}: {x.EggsProduced:N0} producidos, {x.DamagedEggs:N0} dañados, neto {x.NetEggs:N0}"));
    }

    private void ShowSales()
    {
        SetHeader("Ventas", "Registra ventas pagadas, pendientes o parciales. El inventario se descuenta automáticamente.");
        var customer = Box("Cliente"); var packs = Box("Pacas"); var price = Box("Precio por paca"); var paid = Box("Valor pagado"); var notes = Box("Observaciones");
        var btn = new Button { Content = "Registrar venta", Style = (Style)FindResource("PrimaryButton"), Height = 44 };
        btn.Click += async (_, _) =>
        {
            if (!int.TryParse(packs.Text, out var p) || p <= 0) { MessageBox.Show("Ingresa cantidad de pacas válida."); return; }
            if (p * 30 > D.Inventory.AvailableEggs && MessageBox.Show("No hay suficiente inventario. ¿Registrar de todas formas?", "Inventario bajo", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
            if (!decimal.TryParse(price.Text, out var pr) || pr <= 0) { MessageBox.Show("Ingresa precio válido."); return; }
            decimal.TryParse(paid.Text, out var pa);
            var name = string.IsNullOrWhiteSpace(customer.Text) ? "Cliente ocasional" : customer.Text.Trim();
            D.Sales.Add(new Sale { CustomerName = name, Packs = p, UnitPackPrice = pr, PaidAmount = Math.Max(0, pa), Notes = notes.Text });
            if (!D.Customers.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) && name != "Cliente ocasional") D.Customers.Add(new Customer { Name = name });
            D.Inventory.AvailableEggs = Math.Max(0, D.Inventory.AvailableEggs - p * 30);
            await AutoSave(); ShowSales();
        };
        var form = new StackPanel(); form.Children.Add(Text("Nueva venta", 22, true)); form.Children.Add(customer); form.Children.Add(packs); form.Children.Add(price); form.Children.Add(paid); form.Children.Add(notes); form.Children.Add(btn); ContentArea.Children.Add(Card(form));
        AddList("Ventas registradas", D.Sales.OrderByDescending(x => x.Date).Select(x => $"{x.Date:dd/MM/yyyy} - {x.CustomerName} - {x.Packs} pacas - Total {x.Total:C0} - Pagado {x.PaidAmount:C0} - {x.Status}"));
    }

    private void ShowCustomers()
    {
        SetHeader("Clientes", "Clientes frecuentes y cartera pendiente calculada automáticamente.");
        var name = Box("Nombre"); var phone = Box("Teléfono"); var address = Box("Dirección"); var notes = Box("Notas");
        var btn = new Button { Content = "Agregar cliente", Style = (Style)FindResource("PrimaryButton"), Height = 44 };
        btn.Click += async (_, _) => { if (string.IsNullOrWhiteSpace(name.Text)) return; D.Customers.Add(new Customer { Name = name.Text, Phone = phone.Text, Address = address.Text, Notes = notes.Text }); await AutoSave(); ShowCustomers(); };
        var form = new StackPanel(); form.Children.Add(Text("Nuevo cliente", 22, true)); form.Children.Add(name); form.Children.Add(phone); form.Children.Add(address); form.Children.Add(notes); form.Children.Add(btn); ContentArea.Children.Add(Card(form));
        AddList("Clientes frecuentes", D.Customers.OrderBy(x => x.Name).Select(c => $"{c.Name} - {c.Phone} - Debe: {D.Sales.Where(s => s.CustomerName.Equals(c.Name, StringComparison.OrdinalIgnoreCase)).Sum(s => s.Pending):C0}"));
    }

    private void ShowExpenses()
    {
        SetHeader("Gastos", "Controla comida, mantenimiento, transporte, veterinaria y otros costos.");
        var cat = Box("Categoría"); var desc = Box("Descripción"); var amount = Box("Valor");
        var btn = new Button { Content = "Registrar gasto", Style = (Style)FindResource("PrimaryButton"), Height = 44 };
        btn.Click += async (_, _) => { if (!decimal.TryParse(amount.Text, out var a) || a <= 0) return; D.Expenses.Add(new Expense { Category = string.IsNullOrWhiteSpace(cat.Text) ? "Otros" : cat.Text, Description = desc.Text, Amount = a }); await AutoSave(); ShowExpenses(); };
        var form = new StackPanel(); form.Children.Add(Text("Nuevo gasto", 22, true)); form.Children.Add(cat); form.Children.Add(desc); form.Children.Add(amount); form.Children.Add(btn); ContentArea.Children.Add(Card(form));
        AddList("Gastos registrados", D.Expenses.OrderByDescending(x => x.Date).Select(x => $"{x.Date:dd/MM/yyyy} - {x.Category} - {x.Description} - {x.Amount:C0}"));
    }

    private void ShowInventory()
    {
        SetHeader("Inventario", "Inventario automático de huevos criollos.");
        var grid = new UniformGrid { Columns = 3 };
        grid.Children.Add(Kpi("Huevos disponibles", D.Inventory.AvailableEggs.ToString("N0"), "Actualizado automáticamente"));
        grid.Children.Add(Kpi("Pacas disponibles", D.Inventory.AvailablePacks.ToString("N0"), "30 huevos por paca"));
        grid.Children.Add(Kpi("Huevos dañados", D.Inventory.DamagedEggsTotal.ToString("N0"), "Acumulado histórico"));
        ContentArea.Children.Add(grid);
    }

    private void ShowReports()
    {
        SetHeader("Reportes", "Genera un resumen CSV que se puede abrir en Excel.");
        var btn = new Button { Content = "Exportar reporte CSV", Style = (Style)FindResource("PrimaryButton"), Height = 46 };
        btn.Click += (_, _) => ExportCsv();
        ContentArea.Children.Add(Card(new StackPanel { Children = { Text("Reporte general", 22, true), Text("Incluye ventas, gastos, cartera e inventario."), btn } }));
    }

    private async void ShowBackup()
    {
        SetHeader("Guardar y copias", "Protege la información y comparte archivos .lahuevonada.");
        var save = new Button { Content = "Guardar ahora", Style = (Style)FindResource("PrimaryButton"), Height = 46 };
        save.Click += async (_, _) => { await AutoSave(); MessageBox.Show("Guardado."); };
        var backup = new Button { Content = "Crear copia de seguridad", Height = 46, Margin = new Thickness(4) };
        backup.Click += async (_, _) =>
        {
            if (AppState.CurrentFilePath == null) return;
            var path = _service.CreateBackupPath(AppState.CurrentFilePath);
            await _service.SaveAsync(path, D);
            MessageBox.Show($"Copia creada:\n{path}");
        };
        var export = new Button { Content = "Guardar como otro archivo", Height = 46, Margin = new Thickness(4) };
        export.Click += async (_, _) =>
        {
            var dialog = new SaveFileDialog { Filter = "Archivo LaHuevonada (*.lahuevonada)|*.lahuevonada", FileName = "Copia_LaHuevonada.lahuevonada" };
            if (dialog.ShowDialog() == true) await _service.SaveAsync(dialog.FileName, D);
        };
        var sp = new StackPanel(); sp.Children.Add(Text("Archivo actual", 22, true)); sp.Children.Add(Text(AppState.CurrentFilePath ?? "Sin archivo")); sp.Children.Add(save); sp.Children.Add(backup); sp.Children.Add(export); ContentArea.Children.Add(Card(sp));
    }

    private void AddList(string title, IEnumerable<string> rows)
    {
        var sp = new StackPanel(); sp.Children.Add(Text(title, 22, true));
        foreach (var r in rows.DefaultIfEmpty("Sin registros todavía.")) sp.Children.Add(Text("• " + r));
        ContentArea.Children.Add(Card(sp));
    }

    private void ExportCsv()
    {
        var dialog = new SaveFileDialog { Filter = "CSV (*.csv)|*.csv", FileName = "Reporte_LaHuevonada.csv" };
        if (dialog.ShowDialog() != true) return;
        var sb = new StringBuilder();
        sb.AppendLine("REPORTE LAHUEVONADA");
        sb.AppendLine($"Ventas;{D.Sales.Sum(x => x.Total):C0}");
        sb.AppendLine($"Pendiente;{D.Sales.Sum(x => x.Pending):C0}");
        sb.AppendLine($"Gastos;{D.Expenses.Sum(x => x.Amount):C0}");
        sb.AppendLine($"Huevos disponibles;{D.Inventory.AvailableEggs}");
        sb.AppendLine(); sb.AppendLine("VENTAS"); sb.AppendLine("Fecha;Cliente;Pacas;Total;Pagado;Estado");
        foreach (var s in D.Sales) sb.AppendLine($"{s.Date:dd/MM/yyyy};{s.CustomerName};{s.Packs};{s.Total};{s.PaidAmount};{s.Status}");
        File.WriteAllText(dialog.FileName, sb.ToString(), Encoding.UTF8);
        MessageBox.Show("Reporte exportado.");
    }
}
