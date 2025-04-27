using DeviceManagementAPI.Data;
using DeviceManagementAPI.Dtos;
using DeviceManagementAPI.Models;
using DeviceManagementAPI.Protocols;
using DeviceManagementAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace DeviceManagementAPI.Tests
{
    public class DeviceServiceTests
    {
        private DeviceDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<DeviceDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new DeviceDbContext(options);
        }

        [Fact]
        public async Task GetAllDevicesAsync_ReturnsAllDevices_WhenDevicesExist()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var devices = new List<Device>
            {
                new Device { Id = 1, Name = "Device1", Type = "Sensor", CreatedAt = DateTime.UtcNow },
                new Device { Id = 2, Name = "Device2", Type = "Actuator", CreatedAt = DateTime.UtcNow }
            };
            context.Devices.AddRange(devices);
            await context.SaveChangesAsync();
            var service = new DeviceService(context);

            // Act
            var result = await service.GetAllDevicesAsync();

            // Assert
            var deviceList = result.ToList();
            Assert.Equal(2, deviceList.Count);
            Assert.Contains(deviceList, d => d.Id == 1 && d.Name == "Device1" && d.Type == "Sensor");
            Assert.Contains(deviceList, d => d.Id == 2 && d.Name == "Device2" && d.Type == "Actuator");
            foreach (var device in deviceList)
            {
                Assert.False(device.CreatedAt == default(DateTime), "CreatedAt should be set");
            }
        }

        [Fact]
        public async Task GetAllDevicesAsync_ReturnsEmptyList_WhenNoDevicesExist()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new DeviceService(context);

            // Act
            var result = await service.GetAllDevicesAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetDeviceByIdAsync_ReturnsDevice_WhenDeviceExists()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var device = new Device { Id = 1, Name = "Device1", Type = "Sensor", CreatedAt = DateTime.UtcNow };
            context.Devices.Add(device);
            await context.SaveChangesAsync();
            var service = new DeviceService(context);

            // Act
            var result = await service.GetDeviceByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Device1", result.Name);
            Assert.Equal("Sensor", result.Type);
            Assert.False(result.CreatedAt == default(DateTime), "CreatedAt should be set");
        }

        [Fact]
        public async Task GetDeviceByIdAsync_ReturnsNull_WhenDeviceDoesNotExist()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new DeviceService(context);

            // Act
            var result = await service.GetDeviceByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateDeviceAsync_CreatesAndReturnsDevice_WhenRequestIsValid()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new DeviceService(context);
            var request = new CreateDeviceRequest { Name = "NewDevice", Type = "Sensor" };

            // Act
            var result = await service.CreateDeviceAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("NewDevice", result.Name);
            Assert.Equal("Sensor", result.Type);
            Assert.False(result.CreatedAt == default(DateTime), "CreatedAt should be set");
            var savedDevice = await context.Devices.FindAsync(result.Id);
            Assert.NotNull(savedDevice);
            Assert.Equal("NewDevice", savedDevice.Name);
            Assert.Equal("Sensor", savedDevice.Type);
            Assert.False(savedDevice.CreatedAt == default(DateTime), "CreatedAt should be set");
        }

        [Fact]
        public async Task UpdateDeviceAsync_UpdatesDevice_WhenDeviceExists()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var device = new Device { Id = 1, Name = "OldDevice", Type = "Sensor", CreatedAt = DateTime.UtcNow };
            context.Devices.Add(device);
            await context.SaveChangesAsync();
            var service = new DeviceService(context);
            var request = new UpdateDeviceRequest { Name = "UpdatedDevice", Type = "Actuator" };

            // Act
            var result = await service.UpdateDeviceAsync(1, request);

            // Assert
            Assert.True(result);
            var updatedDevice = await context.Devices.FindAsync(1);
            Assert.NotNull(updatedDevice);
            Assert.Equal("UpdatedDevice", updatedDevice.Name);
            Assert.Equal("Actuator", updatedDevice.Type);
            Assert.False(updatedDevice.CreatedAt == default(DateTime), "CreatedAt should be unchanged");
        }

        [Fact]
        public async Task UpdateDeviceAsync_ReturnsFalse_WhenDeviceDoesNotExist()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new DeviceService(context);
            var request = new UpdateDeviceRequest { Name = "UpdatedDevice", Type = "Actuator" };

            // Act
            var result = await service.UpdateDeviceAsync(1, request);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteDeviceAsync_DeletesDevice_WhenDeviceExists()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var device = new Device { Id = 1, Name = "Device1", Type = "Sensor", CreatedAt = DateTime.UtcNow };
            context.Devices.Add(device);
            await context.SaveChangesAsync();
            var service = new DeviceService(context);

            // Act
            var result = await service.DeleteDeviceAsync(1);

            // Assert
            Assert.True(result);
            Assert.Null(await context.Devices.FindAsync(1));
        }

        [Fact]
        public async Task DeleteDeviceAsync_ReturnsFalse_WhenDeviceDoesNotExist()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new DeviceService(context);

            // Act
            var result = await service.DeleteDeviceAsync(1);

            // Assert
            Assert.False(result);
        }
    }
}