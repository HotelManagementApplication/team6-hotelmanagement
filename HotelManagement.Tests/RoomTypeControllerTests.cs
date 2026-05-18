using HotelManagement.API.Controllers;
using HotelManagement.API.DTOs;
using HotelManagement.API.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HotelManagement.Tests;

public class RoomTypeControllerTests
{
    [Fact]
    public async Task GetAll_ReturnsOkWithRoomTypes()
    {
        var roomTypes = new List<RoomTypeDto>
        {
            new() { RoomTypeId = 1, TypeName = "Deluxe", MaxOccupancy = 2, PricePerNight = 1500 }
        };
        var roomTypeService = new Mock<IRoomTypeService>();
        roomTypeService.Setup(service => service.GetAllAsync()).ReturnsAsync(roomTypes);
        var controller = new RoomTypeController(roomTypeService.Object);

        var result = await controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(ok.Value);
        Assert.True(response.Success);
        Assert.Same(roomTypes, response.Data);
        roomTypeService.Verify(service => service.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetById_WhenRoomTypeExists_ReturnsOk()
    {
        var roomType = new RoomTypeDto { RoomTypeId = 1, TypeName = "Suite" };
        var roomTypeService = new Mock<IRoomTypeService>();
        roomTypeService.Setup(service => service.GetByIdAsync(1)).ReturnsAsync(roomType);
        var controller = new RoomTypeController(roomTypeService.Object);

        var result = await controller.GetById(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(ok.Value);
        Assert.Equal("Room type fetched.", response.Message);
        Assert.Same(roomType, response.Data);
        roomTypeService.Verify(service => service.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction()
    {
        var dto = new RoomTypeCreateDto { TypeName = "Family", MaxOccupancy = 4, PricePerNight = 3000 };
        var createdRoomType = new RoomTypeDto { RoomTypeId = 3, TypeName = "Family" };
        var roomTypeService = new Mock<IRoomTypeService>();
        roomTypeService.Setup(service => service.CreateAsync(dto)).ReturnsAsync(createdRoomType);
        var controller = new RoomTypeController(roomTypeService.Object);

        var result = await controller.Create(dto);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(created.Value);
        Assert.Equal(nameof(RoomTypeController.GetById), created.ActionName);
        Assert.Equal(createdRoomType.RoomTypeId, created.RouteValues?["id"]);
        Assert.Same(createdRoomType, response.Data);
        roomTypeService.Verify(service => service.CreateAsync(dto), Times.Once);
    }

    [Fact]
    public async Task UpdatePrice_WhenRoomTypeExists_ReturnsOk()
    {
        var dto = new RoomTypePriceUpdateDto { PricePerNight = 2500 };
        var roomTypeService = new Mock<IRoomTypeService>();
        roomTypeService.Setup(service => service.UpdatePriceAsync(1, dto)).ReturnsAsync(true);
        var controller = new RoomTypeController(roomTypeService.Object);

        var result = await controller.UpdatePrice(1, dto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(ok.Value);
        Assert.Equal("Price updated.", response.Message);
        roomTypeService.Verify(service => service.UpdatePriceAsync(1, dto), Times.Once);
    }

    [Fact]
    public async Task GetById_WhenRoomTypeMissing_ReturnsNotFound()
    {
        var roomTypeService = new Mock<IRoomTypeService>();
        roomTypeService.Setup(service => service.GetByIdAsync(404)).ReturnsAsync((RoomTypeDto?)null);
        var controller = new RoomTypeController(roomTypeService.Object);

        var result = await controller.GetById(404);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(notFound.Value);
        Assert.False(response.Success);
        roomTypeService.Verify(service => service.GetByIdAsync(404), Times.Once);
    }

    [Fact]
    public async Task Update_WhenRoomTypeMissing_ReturnsNotFound()
    {
        var dto = new RoomTypeUpdateDto { TypeName = "Suite" };
        var roomTypeService = new Mock<IRoomTypeService>();
        roomTypeService.Setup(service => service.UpdateAsync(404, dto)).ReturnsAsync(false);
        var controller = new RoomTypeController(roomTypeService.Object);

        var result = await controller.Update(404, dto);

        Assert.IsType<NotFoundObjectResult>(result);
        roomTypeService.Verify(service => service.UpdateAsync(404, dto), Times.Once);
    }

    [Fact]
    public async Task Delete_WhenRoomTypeMissing_ReturnsNotFound()
    {
        var roomTypeService = new Mock<IRoomTypeService>();
        roomTypeService.Setup(service => service.DeleteAsync(404)).ReturnsAsync(false);
        var controller = new RoomTypeController(roomTypeService.Object);

        var result = await controller.Delete(404);

        Assert.IsType<NotFoundObjectResult>(result);
        roomTypeService.Verify(service => service.DeleteAsync(404), Times.Once);
    }

    [Fact]
    public async Task UpdatePrice_WhenRoomTypeMissing_ReturnsNotFound()
    {
        var dto = new RoomTypePriceUpdateDto { PricePerNight = 2500 };
        var roomTypeService = new Mock<IRoomTypeService>();
        roomTypeService.Setup(service => service.UpdatePriceAsync(404, dto)).ReturnsAsync(false);
        var controller = new RoomTypeController(roomTypeService.Object);

        var result = await controller.UpdatePrice(404, dto);

        Assert.IsType<NotFoundObjectResult>(result);
        roomTypeService.Verify(service => service.UpdatePriceAsync(404, dto), Times.Once);
    }
}
