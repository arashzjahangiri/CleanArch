using DeviceManagementAPI.Controllers;
using DeviceManagementAPI.Data;
using DeviceManagementAPI.Dtos;
using DeviceManagementAPI.Models;
using DeviceManagementAPI.Protocols;
using DeviceManagementAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeviceManagementAPI.Tests
{
    public class DeviceControllerTests
    {
        private DeviceDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<DeviceDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new DeviceDbContext(options);
        }

        [Fact]
        public async Task GetDevices_ReturnsOkWithDevices_WhenDevicesExist()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Devices.AddRange(new List<Device>
            {
                new Device { Id = 1, Name = "Device1", Type = "Sensor" },
                new Device { Id = 2, Name = "Device2", Type = "Actuator" }
            });
            await context.SaveChangesAsync();
            var service = new DeviceService(context);
            var controller = new DevicesController(service);

            // Act
            var result = await controller.GetDevices();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var devices = Assert.IsType<List<DeviceDto>>(okResult.Value);
            Assert.Equal(2, devices.Count);
            Assert.Contains(devices, d => d.Name == "Device1" && d.Type == "Sensor");
            Assert.Contains(devices, d => d.Name == "Device2" && d.Type == "Actuator");
        }

        [Fact]
        public async Task GetDevices_ReturnsOkWithEmptyList_WhenNoDevicesExist()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new DeviceService(context);
            var controller = new DevicesController(service);

            // Act
            var result = await controller.GetDevices();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var devices = Assert.IsType<List<DeviceDto>>(okResult.Value);
            Assert.Empty(devices);
        }

        [Fact]
        public async Task GetDevice_ReturnsOk_WhenDeviceExists()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var device = new Device { Id = 1, Name = "Device1", Type = "Sensor" };
            context.Devices.Add(device);
            await context.SaveChangesAsync();
            var service = new DeviceService(context);
            var controller = new DevicesController(service);

            // Act
            var result = await controller.GetDevice(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var deviceDto = Assert.IsType<DeviceDto>(okResult.Value);
            Assert.Equal(1, deviceDto.Id);
            Assert.Equal("Device1", deviceDto.Name);
            Assert.Equal("Sensor", deviceDto.Type);
        }

        [Fact]
        public async Task GetDevice_ReturnsNotFound_WhenDeviceDoesNotExist()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new DeviceService(context);
            var controller = new DevicesController(service);

            // Act
            var result = await controller.GetDevice(1);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateDevice_ReturnsCreated_WhenRequestIsValid()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new DeviceService(context);
            var controller = new DevicesController(service);
            var request = new CreateDeviceRequest { Name = "NewDevice", Type = "Sensor" };

            // Act
            var result = await controller.CreateDevice(request);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var deviceDto = Assert.IsType<DeviceDto>(createdResult.Value);
            Assert.Equal("NewDevice", deviceDto.Name);
            Assert.Equal("Sensor", deviceDto.Type);
            Assert.Equal("GetDevice", createdResult.ActionName);
            Assert.Equal(deviceDto.Id, createdResult.RouteValues["id"]);
        }

        [Fact]
        public async Task CreateDevice_ReturnsBadRequest_WhenRequestIsInvalid()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new DeviceService(context);
            var controller = new DevicesController(service);
            controller.ModelState.AddModelError("Name", "Name is required");
            var request = new CreateDeviceRequest { Name = null!, Type = "Sensor" };

            // Act
            var result = await controller.CreateDevice(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdateDevice_ReturnsNoContent_WhenUpdateIsSuccessful()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var device = new Device { Id = 1, Name = "OldDevice", Type = "Sensor" };
            context.Devices.Add(device);
            await context.SaveChangesAsync();
            var service = new DeviceService(context);
            var controller = new DevicesController(service);
            var request = new UpdateDeviceRequest { Name = "UpdatedDevice", Type = "Actuator" };

            // Act
            var result = await controller.UpdateDevice(1, request);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var updatedDevice = await context.Devices.FindAsync(1);
            Assert.Equal("UpdatedDevice", updatedDevice!.Name);
            Assert.Equal("Actuator", updatedDevice.Type);
        }

        [Fact]
        public async Task UpdateDevice_ReturnsNotFound_WhenDeviceDoesNotExist()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new DeviceService(context);
            var controller = new DevicesController(service);
            var request = new UpdateDeviceRequest { Name = "UpdatedDevice", Type = "Actuator" };

            // Act
            var result = await controller.UpdateDevice(1, request);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateDevice_ReturnsBadRequest_WhenRequestIsInvalid()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new DeviceService(context);
            var controller = new DevicesController(service);
            controller.ModelState.AddModelError("Name", "Name is required");
            var request = new UpdateDeviceRequest { Name = null!, Type = "Actuator" };

            // Act
            var result = await controller.UpdateDevice(1, request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteDevice_ReturnsNoContent_WhenDeviceExists()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var device = new Device { Id = 1, Name = "Device1", Type = "Sensor" };
            context.Devices.Add(device);
            await context.SaveChangesAsync();
            var service = new DeviceService(context);
            var controller = new DevicesController(service);

            // Act
            var result = await controller.DeleteDevice(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Null(await context.Devices.FindAsync(1));
        }

        [Fact]
        public async Task DeleteDevice_ReturnsNotFound_WhenDeviceDoesNotExist()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new DeviceService(context);
            var controller = new DevicesController(service);

            // Act
            var result = await controller.DeleteDevice(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}