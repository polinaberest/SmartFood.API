namespace SmartFood.Common.Constants;

public static class UserRoles
{
    public static IEnumerable<string> AvailableRoles = new[] {Administrator, Supplier, OrganizationManager, Employee};

    // Must be lowercase to avoid case sensitivity issues in the future.
    public const string Administrator = "administrator";
    public const string Supplier = "supplier";
    public const string OrganizationManager = "organizationmanager";
    public const string Employee = "employee";
}