
using System.ComponentModel.DataAnnotations;

namespace DeviceManagementAPI.Protocols;

public class CreateDeviceRequest
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
    public required string Name { get; set; }
    public string? Type { get; set; }
}