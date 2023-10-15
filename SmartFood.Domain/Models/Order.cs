using SmartFood.Domain.Models.Interfaces;

namespace SmartFood.Domain.Models;

public class Order : IODataEntity
{
    public Guid Id { get; set; }
    public User Customer { get; set; }
    public int Count { get; set; }
    public Guid DishOrderedId { get; set; }
    public StoredDish DishOrdered { get; set; }
    public decimal TotalPrice { get; set; }
    public RequestStatus Status { get; set; } = RequestStatus.Approved;
}