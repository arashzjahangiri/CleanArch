using System.ComponentModel.DataAnnotations;

namespace DeviceManagementAPI.Dtos;

public class UpdateDeviceRequest
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
    public required string Name { get; set; }

    [MaxLength(50, ErrorMessage = "Type cannot be longer than 50 characters")]
    public string? Type { get; set; }
}
