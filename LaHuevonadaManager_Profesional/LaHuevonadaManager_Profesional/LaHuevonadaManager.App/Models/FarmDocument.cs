namespace LaHuevonadaManager.Models;

public class FarmDocument
{
    public string FarmName { get; set; } = "LaHuevonada";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public List<Customer> Customers { get; set; } = new();
    public List<Sale> Sales { get; set; } = new();
    public List<Expense> Expenses { get; set; } = new();
    public List<WeeklyProductionLot> ProductionLots { get; set; } = new();
    public Inventory Inventory { get; set; } = new();
}

public class Customer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Address { get; set; } = "";
    public string Notes { get; set; } = "";
}

public class Sale
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Date { get; set; } = DateTime.Today;
    public Guid? CustomerId { get; set; }
    public string CustomerName { get; set; } = "Cliente ocasional";
    public int Packs { get; set; }
    public int EggsPerPack { get; set; } = 30;
    public decimal UnitPackPrice { get; set; }
    public decimal PaidAmount { get; set; }
    public string PaymentMethod { get; set; } = "Efectivo";
    public string Notes { get; set; } = "";
    public decimal Total => Packs * UnitPackPrice;
    public decimal Pending => Math.Max(0, Total - PaidAmount);
    public string Status => Pending <= 0 ? "Pagado" : PaidAmount <= 0 ? "Pendiente" : "Parcial";
}

public class Expense
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Date { get; set; } = DateTime.Today;
    public string Category { get; set; } = "Comida";
    public string Description { get; set; } = "";
    public decimal Amount { get; set; }
}

public class WeeklyProductionLot
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime WeekStart { get; set; } = DateTime.Today;
    public int EggsProduced { get; set; }
    public int DamagedEggs { get; set; }
    public string Notes { get; set; } = "";
    public int NetEggs => Math.Max(0, EggsProduced - DamagedEggs);
}

public class Inventory
{
    public int AvailableEggs { get; set; }
    public int AvailablePacks => AvailableEggs / 30;
    public int DamagedEggsTotal { get; set; }
}
