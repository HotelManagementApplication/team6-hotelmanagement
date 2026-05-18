using System.Security.Claims;
using FluentAssertions;
using HotelManagement.API.DTOs;
using HotelManagement.API.Modules.ReservationModule.Controllers;
using HotelManagement.API.Modules.ReservationModule.DTOs;
using HotelManagement.API.Modules.ReservationModule.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HotelManagement.Tests;

public class ReservationControllerTest
{
    private readonly Mock<IReservationService> _serviceMock;
    private readonly ReservationController _controller;

    public ReservationControllerTest()
    {
        _serviceMock = new Mock<IReservationService>();
        _controller = new ReservationController(_serviceMock.Object);
    }

    private void SetUserEmail(string email)
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                    [new Claim(ClaimTypes.Email, email)],
                    authenticationType: "TestAuth"))
            }
        };
    }

    #region POSITIVE TEST CASES

    [Fact]
    public async Task GetReservationById_ShouldReturnOk_WhenReservationExists()
    {
        const int reservationId = 1;
        var reservation = new ReservationDetailsDto
        {
            ReservationId = reservationId,
            GuestName = "John Doe",
            GuestEmail = "john@example.com",
            GuestPhoneNumber = "1234567890",
            RoomNumber = 101,
            RoomType = "Deluxe",
            HotelName = "Grand Hotel"
        };

        _serviceMock.Setup(service => service.GetReservationDetailsAsync(reservationId)).ReturnsAsync(reservation);

        var result = await _controller.GetReservationById(reservationId);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);

        var response = okResult.Value.Should().BeOfType<ApiResponse<ReservationDetailsDto>>().Subject;
        response.Success.Should().BeTrue();
        response.StatusCode.Should().Be(200);
        response.Message.Should().Be("Successfully retrieved reservation.");
        response.Data.Should().BeSameAs(reservation);

        _serviceMock.Verify(service => service.GetReservationDetailsAsync(reservationId), Times.Once);
    }

    [Fact]
    public async Task GetMyReservations_ShouldReturnOk_WhenUserHasReservations()
    {
        const string email = "guest@example.com";
        SetUserEmail(email);

        var reservations = new List<ReservationDetailsDto>
        {
            new() { ReservationId = 1, GuestEmail = email, GuestName = "Guest One", RoomNumber = 201, RoomType = "Standard" },
            new() { ReservationId = 2, GuestEmail = email, GuestName = "Guest Two", RoomNumber = 202, RoomType = "Suite" }
        };

        _serviceMock.Setup(service => service.GetReservationsByUserEmailAsync(email)).ReturnsAsync(reservations);

        var result = await _controller.GetMyReservations();

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);

        var response = okResult.Value.Should().BeOfType<ApiResponse<List<ReservationDetailsDto>>>().Subject;
        response.Success.Should().BeTrue();
        response.StatusCode.Should().Be(200);
        response.Data.Should().HaveCount(2);
        response.Data![0].GuestEmail.Should().Be(email);

        _serviceMock.Verify(service => service.GetReservationsByUserEmailAsync(email), Times.Once);
    }

    [Fact]
    public async Task CreateReservation_ShouldReturnCreated_WhenReservationIsCreated()
    {
        var dto = new CreateReservationDto
        {
            GuestName = "Alice",
            GuestEmail = "alice@example.com",
            GuestPhoneNumber = "5551234567",
            CheckInDate = new DateOnly(2026, 6, 10),
            CheckOutDate = new DateOnly(2026, 6, 12),
            RoomId = 12
        };

        var createdReservation = new ReservationDetailsDto
        {
            ReservationId = 42,
            GuestName = dto.GuestName,
            GuestEmail = dto.GuestEmail,
            GuestPhoneNumber = dto.GuestPhoneNumber,
            CheckInDate = dto.CheckInDate,
            CheckOutDate = dto.CheckOutDate,
            RoomNumber = 12,
            RoomType = "Deluxe"
        };

        _serviceMock.Setup(service => service.CreateReservationAsync(dto)).ReturnsAsync(createdReservation);

        var result = await _controller.CreateReservation(dto);

        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.StatusCode.Should().Be(201);
        createdResult.ActionName.Should().Be(nameof(ReservationController.CreateReservation));
        createdResult.RouteValues.Should().ContainKey("ReservationId");
        createdResult.RouteValues!["ReservationId"].Should().Be(createdReservation.ReservationId);

        var response = createdResult.Value.Should().BeOfType<ApiResponse<string?>>().Subject;
        response.Success.Should().BeTrue();
        response.StatusCode.Should().Be(201);
        response.Message.Should().Be("Successfully created reservation.");

        _serviceMock.Verify(service => service.CreateReservationAsync(dto), Times.Once);
    }

    [Fact]
    public async Task GetMyReservations_ShouldReturnOk_WhenUserHasSingleReservation()
    {
        const string email = "single@example.com";
        SetUserEmail(email);

        var reservations = new List<ReservationDetailsDto>
        {
            new() { ReservationId = 3, GuestEmail = email, GuestName = "Single Guest", RoomNumber = 303, RoomType = "Standard" }
        };

        _serviceMock.Setup(service => service.GetReservationsByUserEmailAsync(email)).ReturnsAsync(reservations);

        var result = await _controller.GetMyReservations();

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);

        var response = okResult.Value.Should().BeOfType<ApiResponse<List<ReservationDetailsDto>>>().Subject;
        response.Success.Should().BeTrue();
        response.Data.Should().HaveCount(1);
        response.Data![0].ReservationId.Should().Be(3);

        _serviceMock.Verify(service => service.GetReservationsByUserEmailAsync(email), Times.Once);
    }

    #endregion

    #region NEGATIVE TEST CASES

    [Fact]
    public async Task GetReservationById_ShouldThrowWrappedException_WhenServiceFails()
    {
        const int reservationId = 9;

        _serviceMock.Setup(service => service.GetReservationDetailsAsync(reservationId))
            .ThrowsAsync(new InvalidOperationException("Database failure"));

        Func<Task> act = async () => await _controller.GetReservationById(reservationId);

        var exception = await act.Should().ThrowAsync<Exception>();
        exception.Which.Message.Should().Be("Internal server error occured while fetching reservation detals.");
        exception.Which.InnerException.Should().BeOfType<InvalidOperationException>();

        _serviceMock.Verify(service => service.GetReservationDetailsAsync(reservationId), Times.Once);
    }

    [Fact]
    public async Task GetMyReservations_ShouldThrowWrappedException_WhenEmailClaimMissing()
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity())
            }
        };

        Func<Task> act = async () => await _controller.GetMyReservations();

        var exception = await act.Should().ThrowAsync<Exception>();
        exception.Which.Message.Should().Be("Internal server error occured while fetching reservations.");
        exception.Which.InnerException.Should().BeOfType<NullReferenceException>();

        _serviceMock.Verify(service => service.GetReservationsByUserEmailAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetMyReservations_ShouldThrowWrappedException_WhenServiceFails()
    {
        const string email = "error@example.com";
        SetUserEmail(email);

        _serviceMock.Setup(service => service.GetReservationsByUserEmailAsync(email))
            .ThrowsAsync(new InvalidOperationException("Database failure"));

        Func<Task> act = async () => await _controller.GetMyReservations();

        var exception = await act.Should().ThrowAsync<Exception>();
        exception.Which.Message.Should().Be("Internal server error occured while fetching reservations.");
        exception.Which.InnerException.Should().BeOfType<InvalidOperationException>();

        _serviceMock.Verify(service => service.GetReservationsByUserEmailAsync(email), Times.Once);
    }

    [Fact]
    public async Task CreateReservation_ShouldThrowWrappedException_WhenServiceFails()
    {
        var dto = new CreateReservationDto
        {
            GuestName = "Bob",
            GuestEmail = "bob@example.com",
            GuestPhoneNumber = "4445556666",
            CheckInDate = new DateOnly(2026, 7, 1),
            CheckOutDate = new DateOnly(2026, 7, 3),
            RoomId = 20
        };

        _serviceMock.Setup(service => service.CreateReservationAsync(dto))
            .ThrowsAsync(new InvalidOperationException("Reservation conflict"));

        Func<Task> act = async () => await _controller.CreateReservation(dto);

        var exception = await act.Should().ThrowAsync<Exception>();
        exception.Which.Message.Should().Be("Internal server error occured while making reservation.");
        exception.Which.InnerException.Should().BeOfType<InvalidOperationException>();

        _serviceMock.Verify(service => service.CreateReservationAsync(dto), Times.Once);
    }

    #endregion
}