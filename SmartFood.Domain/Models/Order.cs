using SmartFood.Domain.Models.Interfaces;

namespace SmartFood.Domain.Models;

public class Order : IODataEntity
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public User Customer { get; set; }
    public int Count { get; set; }
    public Guid OrderedDishId { get; set; }
    public StoredDish OrderedDish { get; set; }
    public decimal TotalPrice { get; set; }
    public RequestStatus Status { get; set; } = RequestStatus.Approved;
}