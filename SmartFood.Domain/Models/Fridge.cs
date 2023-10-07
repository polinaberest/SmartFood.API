namespace SmartFood.Domain.Models;

public class Fridge
{
    public Guid Id { get; set; }
    public string PlacementDescription { get; set; } = string.Empty;
    public bool IsOpen { get; set; }
    public bool IsDeleted { get; set; }
    public Guid FilialId { get; set; }
    public Filial Filial { get; set; } = new();
    public List<StoredDish>? DishesServed { get; set; }
}