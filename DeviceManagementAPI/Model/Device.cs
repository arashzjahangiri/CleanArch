using System.ComponentModel.DataAnnotations;

namespace DeviceManagementAPI.Models;
public class Device
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Name is required")]
    public required string Name { get; set; }
    public string? Type { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}