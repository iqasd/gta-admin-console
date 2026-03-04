using GtaAdminReportsApp.Enums;

namespace GtaAdminReportsApp.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public AdminClass AdminClass { get; set; } = AdminClass.Junior;
    public bool IsAdmin { get; set; }
}
