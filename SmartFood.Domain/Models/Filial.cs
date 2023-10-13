using SmartFood.Domain.Models.Interfaces;

namespace SmartFood.Domain.Models;

public class Filial : IODataEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public Guid OrganizationId { get; set; }
    public Organization OwnerOrganization { get; set; }
    public List<Fridge>? FridgesInstalled { get; set; } = new List<Fridge>();
    public bool IsDeleted { get; set; } = false;
}