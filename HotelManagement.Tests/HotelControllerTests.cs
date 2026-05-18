using Xunit;
using Moq;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using HotelManagement.API.Modules.HotelModule.Controllers;
using HotelManagement.API.Modules.HotelModule.DTOs;
using HotelManagement.API.Modules.HotelModule.Services;
using HotelManagement.Common.DTOs;
using HotelManagement.API.Exceptions;

namespace HotelManagement.Tests;

public class HotelControllerTests
{
    private readonly Mock<IHotelService> _mockHotelService;
    private readonly Mock<IValidator<HotelDto>> _mockCreateValidator;
    private readonly Mock<IValidator<HotelDto>> _mockUpdateValidator;
    private readonly HotelController _controller;

    public HotelControllerTests()
    {
        _mockHotelService = new Mock<IHotelService>();
        _mockCreateValidator = new Mock<IValidator<HotelDto>>();
        _mockUpdateValidator = new Mock<IValidator<HotelDto>>();
        _controller = new HotelController(_mockHotelService.Object, _mockCreateValidator.Object, _mockUpdateValidator.Object);
    }

    #region POSITIVE TEST CASES

    /// <summary>
    /// POSITIVE TEST 1: GetAll should return all hotels successfully
    /// </summary>
    [Fact]
    public async Task GetAll_ShouldReturnAllHotels_WhenSuccess()
    {
        // Arrange
        var hotels = new List<HotelResponseDto>
        {
            new() { HotelId = 1, Name = "Hotel One", Location = "New York", Description = "Luxury Hotel" },
            new() { HotelId = 2, Name = "Hotel Two", Location = "Los Angeles", Description = "Budget Hotel" }
        };

        _mockHotelService.Setup(s => s.GetAllAsync()).ReturnsAsync(hotels);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);

        var response = okResult.Value.Should().BeOfType<ApiResponse<IEnumerable<HotelResponseDto>>>().Subject;
        response.Success.Should().BeTrue();
        response.Data.Should().HaveCount(2);
        response.Data?.First().Name.Should().Be("Hotel One");

        _mockHotelService.Verify(s => s.GetAllAsync(), Times.Once);
    }
    
    /// <summary>
    /// POSITIVE TEST 2: GetById should return hotel when it exists
    /// </summary>
    [Fact]
    public async Task GetById_ShouldReturnHotel_WhenHotelExists()
    {
        // Arrange
        int hotelId = 1;
        var hotel = new HotelResponseDto { HotelId = 1, Name = "Hotel One", Location = "New York", Description = "Luxury Hotel" };

        _mockHotelService.Setup(s => s.GetByIdAsync(hotelId)).ReturnsAsync(hotel);

        // Act
        var result = await _controller.GetById(hotelId);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);

        var response = okResult.Value!.Should().BeOfType<ApiResponse<HotelResponseDto>>().Subject;
        response.Success.Should().BeTrue();
        response.Data!.HotelId.Should().Be(1);
        response.Data.Name.Should().Be("Hotel One");

        _mockHotelService.Verify(s => s.GetByIdAsync(hotelId), Times.Once);
    }

    /// <summary>
    /// POSITIVE TEST 3: Create should return created hotel when valid data provided
    /// </summary>
    [Fact]
    public async Task Create_ShouldReturnCreatedHotel_WhenValidDataProvided()
    {
        // Arrange
        var hotelDto = new HotelDto { Name = "New Hotel", Location = "Chicago", Description = "Beautiful Hotel" };
        var createdHotel = new HotelResponseDto { HotelId = 3, Name = "New Hotel", Location = "Chicago", Description = "Beautiful Hotel" };

        var validationResult = new FluentValidation.Results.ValidationResult();
        _mockCreateValidator.Setup(v => v.ValidateAsync(It.IsAny<HotelDto>(), CancellationToken.None))
            .ReturnsAsync(validationResult);

        _mockHotelService.Setup(s => s.CreateAsync(hotelDto)).ReturnsAsync(createdHotel);

        // Act
        var result = await _controller.Create(hotelDto);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.StatusCode.Should().Be(201);
        createdResult.ActionName.Should().Be(nameof(HotelController.GetById));

        var response = createdResult.Value!.Should().BeOfType<ApiResponse<HotelResponseDto>>().Subject;
        response.Success.Should().BeTrue();
        response.Data!.Name.Should().Be("New Hotel");

        _mockCreateValidator.Verify(v => v.ValidateAsync(hotelDto, CancellationToken.None), Times.Once);
        _mockHotelService.Verify(s => s.CreateAsync(hotelDto), Times.Once);
    }

    /// <summary>
    /// POSITIVE TEST 4: Delete should return success when hotel exists
    /// </summary>
    [Fact]
    public async Task Delete_ShouldReturnSuccess_WhenHotelExists()
    {
        // Arrange
        int hotelId = 1;
        _mockHotelService.Setup(s => s.DeleteAsync(hotelId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(hotelId);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);

        var response = okResult.Value.Should().BeOfType<ApiResponse<object>>().Subject;
        response.Success.Should().BeTrue();
        response.Message.Should().Contain("deleted successfully");

        _mockHotelService.Verify(s => s.DeleteAsync(hotelId), Times.Once);
    }

    #endregion

    #region NEGATIVE TEST CASES

    /// <summary>
    /// NEGATIVE TEST 1: GetById should throw NotFoundException when hotel does not exist
    /// </summary>
    [Fact]
    public async Task GetById_ShouldThrowNotFoundException_WhenHotelDoesNotExist()
    {
        // Arrange
        int hotelId = 999;
        _mockHotelService.Setup(s => s.GetByIdAsync(hotelId))
            .ThrowsAsync(new NotFoundException($"Hotel with ID {hotelId} not found."));

        // Act
        var act = async () => await _controller.GetById(hotelId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _mockHotelService.Verify(s => s.GetByIdAsync(hotelId), Times.Once);
    }

    /// <summary>
    /// NEGATIVE TEST 2: Create should return UnprocessableEntity when validation fails
    /// </summary>
    [Fact]
    public async Task Create_ShouldReturnUnprocessableEntity_WhenValidationFails()
    {
        // Arrange
        var hotelDto = new HotelDto { Name = "", Location = "", Description = "" };

        var validationFailures = new List<FluentValidation.Results.ValidationFailure>
        {
            new("Name", "Name is required."),
            new("Location", "Location is required.")
        };
        var validationResult = new FluentValidation.Results.ValidationResult(validationFailures);

        _mockCreateValidator.Setup(v => v.ValidateAsync(It.IsAny<HotelDto>(), CancellationToken.None))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _controller.Create(hotelDto);

        // Assert
        var unprocessableResult = result.Should().BeOfType<UnprocessableEntityObjectResult>().Subject;
        unprocessableResult.StatusCode.Should().Be(422);

        var response = unprocessableResult.Value.Should().BeOfType<ApiResponse<object>>().Subject;
        response.Success.Should().BeFalse();
        response.Errors.Should().NotBeNull();
        response.Errors?.Count().Should().Be(2);

        _mockCreateValidator.Verify(v => v.ValidateAsync(hotelDto, CancellationToken.None), Times.Once);
        _mockHotelService.Verify(s => s.CreateAsync(It.IsAny<HotelDto>()), Times.Never);
    }

    /// <summary>
    /// NEGATIVE TEST 3: Delete should throw NotFoundException when hotel does not exist
    /// </summary>
    [Fact]
    public async Task Delete_ShouldThrowNotFoundException_WhenHotelDoesNotExist()
    {
        // Arrange
        int hotelId = 999;
        _mockHotelService.Setup(s => s.DeleteAsync(hotelId))
            .ThrowsAsync(new NotFoundException($"Hotel with ID {hotelId} not found."));

        // Act
        var act = async () => await _controller.Delete(hotelId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _mockHotelService.Verify(s => s.DeleteAsync(hotelId), Times.Once);
    }

    /// <summary>
    /// NEGATIVE TEST 4: Create should return UnprocessableEntity when required fields are missing
    /// </summary>
    [Fact]
    public async Task Create_ShouldReturnUnprocessableEntity_WhenRequiredFieldsMissing()
    {
        // Arrange
        var hotelDto = new HotelDto { Name = null, Location = null, Description = "Some description" };

        var validationFailures = new List<FluentValidation.Results.ValidationFailure>
        {
            new("Name", "Name cannot be empty."),
            new("Location", "Location cannot be empty.")
        };
        var validationResult = new FluentValidation.Results.ValidationResult(validationFailures);

        _mockCreateValidator.Setup(v => v.ValidateAsync(It.IsAny<HotelDto>(), CancellationToken.None))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _controller.Create(hotelDto);

        // Assert
        var unprocessableResult = result.Should().BeOfType<UnprocessableEntityObjectResult>().Subject;
        unprocessableResult.StatusCode.Should().Be(422);

        var response = unprocessableResult.Value.Should().BeOfType<ApiResponse<object>>().Subject;
        response.Success.Should().BeFalse();
        response.StatusCode.Should().Be(422);

        _mockCreateValidator.Verify(v => v.ValidateAsync(hotelDto, CancellationToken.None), Times.Once);
    }
     

    #endregion
}
