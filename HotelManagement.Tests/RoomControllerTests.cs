using HotelManagement.API.Controllers;
using HotelManagement.API.DTOs;
using HotelManagement.API.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HotelManagement.Tests;

public class RoomControllerTests
{
    [Fact]
    public async Task GetAll_ReturnsOkWithRooms()
    {
        var rooms = new List<RoomDto>
        {
            new() { RoomId = 1, RoomNumber = 101, RoomTypeId = 2, IsAvailable = true }
        };
        var roomService = new Mock<IRoomService>();
        roomService.Setup(service => service.GetAllAsync()).ReturnsAsync(rooms);
        var controller = new RoomController(roomService.Object);

        var result = await controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(ok.Value);
        Assert.True(response.Success);
        Assert.Same(rooms, response.Data);
        roomService.Verify(service => service.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetById_WhenRoomExists_ReturnsOk()
    {
        var room = new RoomDto { RoomId = 1, RoomNumber = 101, RoomTypeId = 2 };
        var roomService = new Mock<IRoomService>();
        roomService.Setup(service => service.GetByIdAsync(1)).ReturnsAsync(room);
        var controller = new RoomController(roomService.Object);

        var result = await controller.GetById(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(ok.Value);
        Assert.Equal("Room fetched.", response.Message);
        Assert.Same(room, response.Data);
        roomService.Verify(service => service.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction()
    {
        var dto = new RoomCreateDto { RoomNumber = 205, RoomTypeId = 3 };
        var createdRoom = new RoomDto { RoomId = 5, RoomNumber = 205, RoomTypeId = 3 };
        var roomService = new Mock<IRoomService>();
        roomService.Setup(service => service.CreateAsync(dto)).ReturnsAsync(createdRoom);
        var controller = new RoomController(roomService.Object);

        var result = await controller.Create(dto);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(created.Value);
        Assert.Equal(nameof(RoomController.GetById), created.ActionName);
        Assert.Equal(createdRoom.RoomId, created.RouteValues?["id"]);
        Assert.Same(createdRoom, response.Data);
        roomService.Verify(service => service.CreateAsync(dto), Times.Once);
    }

    [Fact]
    public async Task UpdateAvailability_WhenRoomExists_ReturnsOk()
    {
        var dto = new RoomAvailabilityUpdateDto { IsAvailable = false };
        var roomService = new Mock<IRoomService>();
        roomService.Setup(service => service.UpdateAvailabilityAsync(1, dto)).ReturnsAsync(true);
        var controller = new RoomController(roomService.Object);

        var result = await controller.UpdateAvailability(1, dto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(ok.Value);
        Assert.Equal("Room availability updated.", response.Message);
        roomService.Verify(service => service.UpdateAvailabilityAsync(1, dto), Times.Once);
    }

    [Fact]
    public async Task GetById_WhenRoomMissing_ReturnsNotFound()
    {
        var roomService = new Mock<IRoomService>();
        roomService.Setup(service => service.GetByIdAsync(404)).ReturnsAsync((RoomDto?)null);
        var controller = new RoomController(roomService.Object);

        var result = await controller.GetById(404);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(notFound.Value);
        Assert.False(response.Success);
        roomService.Verify(service => service.GetByIdAsync(404), Times.Once);
    }

    [Fact]
    public async Task Update_WhenRoomMissing_ReturnsNotFound()
    {
        var dto = new RoomUpdateDto { RoomNumber = 201, RoomTypeId = 2 };
        var roomService = new Mock<IRoomService>();
        roomService.Setup(service => service.UpdateAsync(404, dto)).ReturnsAsync(false);
        var controller = new RoomController(roomService.Object);

        var result = await controller.Update(404, dto);

        Assert.IsType<NotFoundObjectResult>(result);
        roomService.Verify(service => service.UpdateAsync(404, dto), Times.Once);
    }

    [Fact]
    public async Task Delete_WhenRoomMissing_ReturnsNotFound()
    {
        var roomService = new Mock<IRoomService>();
        roomService.Setup(service => service.DeleteAsync(404)).ReturnsAsync(false);
        var controller = new RoomController(roomService.Object);

        var result = await controller.Delete(404);

        Assert.IsType<NotFoundObjectResult>(result);
        roomService.Verify(service => service.DeleteAsync(404), Times.Once);
    }

    [Fact]
    public async Task UpdateAvailability_WhenRoomMissing_ReturnsNotFound()
    {
        var dto = new RoomAvailabilityUpdateDto { IsAvailable = true };
        var roomService = new Mock<IRoomService>();
        roomService.Setup(service => service.UpdateAvailabilityAsync(404, dto)).ReturnsAsync(false);
        var controller = new RoomController(roomService.Object);

        var result = await controller.UpdateAvailability(404, dto);

        Assert.IsType<NotFoundObjectResult>(result);
        roomService.Verify(service => service.UpdateAvailabilityAsync(404, dto), Times.Once);
    }
}
