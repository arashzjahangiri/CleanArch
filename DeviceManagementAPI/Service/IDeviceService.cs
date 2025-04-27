
using DeviceManagementAPI.Dtos;
using DeviceManagementAPI.Protocols;

namespace DeviceManagementAPI.Services;

public interface IDeviceService
{
    public Task<IEnumerable<DeviceDto>> GetAllDevicesAsync();
    Task<DeviceDto?> GetDeviceByIdAsync(int id);
    Task<DeviceDto> CreateDeviceAsync(CreateDeviceRequest deviceRequest);
    Task<bool> UpdateDeviceAsync(int id, UpdateDeviceRequest request);
    Task<bool> DeleteDeviceAsync(int id);
}