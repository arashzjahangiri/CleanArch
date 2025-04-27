using DeviceManagementAPI.Data;
using DeviceManagementAPI.Dtos;
using DeviceManagementAPI.Models;
using DeviceManagementAPI.Protocols;
using Microsoft.EntityFrameworkCore;

namespace DeviceManagementAPI.Services;

public class DeviceService : IDeviceService
{
    private readonly DeviceDbContext _context;

    public DeviceService(DeviceDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DeviceDto>> GetAllDevicesAsync()
    {
        return await _context.Devices
            .Select(d => new DeviceDto
            {
                Id = d.Id,
                Name = d.Name,
                Type = d.Type,
                CreatedAt = d.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<DeviceDto?> GetDeviceByIdAsync(int id)
    {
        var device = await _context.Devices.FindAsync(id);
        if (device == null)
        {
            return null;
        }

        return new DeviceDto
        {
            Id = device.Id,
            Name = device.Name,
            Type = device.Type,
            CreatedAt = device.CreatedAt
        };
    }

    public async Task<DeviceDto> CreateDeviceAsync(CreateDeviceRequest request)
    {
        var device = new Device
        {
            Name = request.Name,
            Type = request.Type,
            CreatedAt = DateTime.UtcNow
        };

        _context.Devices.Add(device);
        await _context.SaveChangesAsync();

        return new DeviceDto
        {
            Id = device.Id,
            Name = device.Name,
            Type = device.Type,
            CreatedAt = device.CreatedAt
        };
    }

    public async Task<bool> DeleteDeviceAsync(int id)
    {
        var device = await _context.Devices.FindAsync(id);
        if (device == null)
        {
            return false;
        }

        _context.Devices.Remove(device);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateDeviceAsync(int id, UpdateDeviceRequest request)
    {
        var device = await _context.Devices.FindAsync(id);
        if (device == null)
        {
            return false;
        }

        device.Name = request.Name;
        device.Type = request.Type;

        _context.Devices.Update(device);
        await _context.SaveChangesAsync();
        return true;
    }

}
