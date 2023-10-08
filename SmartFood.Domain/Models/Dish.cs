using SmartFood.Domain.Models.Interfaces;

namespace SmartFood.Domain.Models;

public class Dish : IODataEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid SupplierId { get; set; }
    public Supplier Supplier { get; set; }
    public List<StoredDish>? FridgesServedInCount { get; set;}
}