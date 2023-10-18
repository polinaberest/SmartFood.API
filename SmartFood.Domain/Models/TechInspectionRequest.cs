using SmartFood.Domain.Models.Interfaces;

namespace SmartFood.Domain.Models;

public class TechInspectionRequest : IODataEntity
{
    public Guid Id { get; set; }
    public DateTime RequestTime { get; set; } = DateTime.UtcNow;
    public DateTime? FulfilledTime { get; set; }
    public RequestStatus Status { get; set; } = RequestStatus.Unseen;
    public Guid FridgeId { get; set; }
    public Fridge Fridge { get; set; }
    public decimal Temperature { get; set; }
    public int OpensCount { get; set; }
}