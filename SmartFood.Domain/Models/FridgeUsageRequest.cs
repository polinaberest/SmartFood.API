namespace SmartFood.Domain.Models;

public class FridgeUsageRequest
{
    public Guid Id { get; set; }
    public string? RequestMessage { get; set; }
    public DateTime RequestTime { get; set; } = DateTime.UtcNow;
    public DateTime? AnsweredTime { get; set; }
    public RequestStatus Status { get; set; } = RequestStatus.Unseen;
    public Guid SupplierId { get; set; }
    public Supplier Supplier { get; set; }
    public Guid FridgeId { get; set; }
    public Fridge Fridge { get; set; }
    public Guid DishId { get; set; }
    public Dish Dish { get; set; }
}