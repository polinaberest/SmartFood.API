using SmartFood.Domain.Models.Interfaces;

namespace SmartFood.Domain.Models;

public class Fridge : IODataEntity
{
    public Guid Id { get; set; }
    public string PlacementDescription { get; set; } = string.Empty;
    public bool IsOpen { get; set; }
    public bool IsDeleted { get; set; } = false;
    public string URI { get; set; } = string.Empty;
    public Guid FilialId { get; set; }
    public Filial? Filial { get; set; }
    public List<StoredDish>? DishesServed { get; set; } = new List<StoredDish>();
}