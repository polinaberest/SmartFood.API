namespace SmartFood.Domain.Models;

public class FridgeInstallationRequest
{
    public Guid Id { get; set; }
    public string PlacementDescription { get; set; } = string.Empty;
    public DateTime RequestTime { get; set; } = DateTime.UtcNow;
    public DateTime? AnsweredTime { get; set; }
    public RequestStatus Status { get; set; } = RequestStatus.Unseen;
    public Guid FilialId { get; set; }
    public Filial Filial { get; set; }
}