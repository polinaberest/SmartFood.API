namespace SmartFood.Domain.Models;

public class Dish
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid SupplierId { get; set; }
    public Supplier Supplier { get; set; } = new();
    public List<StoredDish>? FridgesServedInCount { get; set;}
}