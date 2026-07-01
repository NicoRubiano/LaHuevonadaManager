using System.Collections.ObjectModel;

namespace LaHuevonadaManager.Models;

public sealed class AppData
{
    public string FarmName { get; set; } = "LaHuevonada";
    public ObservableCollection<Customer> Customers { get; set; } = new();
    public ObservableCollection<Sale> Sales { get; set; } = new();
    public ObservableCollection<WeeklyProduction> Productions { get; set; } = new();
    public ObservableCollection<Expense> Expenses { get; set; } = new();
    public int CurrentEggStock { get; set; }
    public int EggsPerPaca { get; set; } = 30;
}

public sealed class Customer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Address { get; set; } = "";
    public string Notes { get; set; } = "";
    public decimal PendingBalance { get; set; }
}

public enum SaleStatus { Pagada, Pendiente, Parcial }

public sealed class Sale
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Date { get; set; } = DateTime.Now;
    public string CustomerName { get; set; } = "Cliente ocasional";
    public int Pacas { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal PaidAmount { get; set; }
    public SaleStatus Status { get; set; } = SaleStatus.Pagada;
    public decimal Total => Pacas * UnitPrice;
    public decimal Pending => Math.Max(0, Total - PaidAmount);
}

public sealed class WeeklyProduction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime WeekStart { get; set; } = DateTime.Today;
    public int EggsProduced { get; set; }
    public int DamagedEggs { get; set; }
    public string Notes { get; set; } = "";
    public int NetEggs => Math.Max(0, EggsProduced - DamagedEggs);
}

public sealed class Expense
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Date { get; set; } = DateTime.Now;
    public string Category { get; set; } = "Comida";
    public string Description { get; set; } = "";
    public decimal Amount { get; set; }
}
