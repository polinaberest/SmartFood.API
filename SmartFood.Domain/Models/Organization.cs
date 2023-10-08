namespace SmartFood.Domain.Models;

public class Organization
{
    public Guid Id { get; set; }
    public bool IsBlocked { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime RegisterDate { get; set; } = DateTime.UtcNow;
    public Guid ManagerId { get; set; }
    public User Manager { get; set; }
    public List<Filial>? Filials { get; set; }
}