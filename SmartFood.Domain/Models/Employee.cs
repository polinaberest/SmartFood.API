namespace SmartFood.Domain.Models;

public class Employee
{
    public Guid Id { get; set; }

    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
}