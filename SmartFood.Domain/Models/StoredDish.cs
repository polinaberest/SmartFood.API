namespace SmartFood.Domain.Models;

public class StoredDish
{
    public Guid Id { get; set; }
    public int CountAvailable { get; set; }
    public Guid DishId { get; set; }
    public Dish Dish { get; set; }
    public Guid FridgeId { get; set; }
    public Fridge Fridge { get; set; }
}