using FluentAssertions;
using HotelManagement.API.DTOs;
using HotelManagement.API.Exceptions;
using HotelManagement.API.Modules.PaymentModule.Controllers;
using HotelManagement.API.Modules.PaymentModule.DTOs;
using HotelManagement.API.Modules.PaymentModule.Services;
using HotelManagement.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HotelManagement.Tests;

public class PaymentControllerTest
{
    private readonly Mock<IPaymentService> _serviceMock;
    private readonly PaymentController _controller;

    public PaymentControllerTest()
    {
        _serviceMock = new Mock<IPaymentService>();
        _controller = new PaymentController(_serviceMock.Object);
    }

    // =========================
    // 4 POSITIVE TEST CASES
    // =========================

    [Fact]
    public async Task GetPayments_ShouldReturnOk_WithApiResponse()
    {
        var payments = new List<Payment>
        {
            new() { PaymentId = 1, ReservationId = 1, Amount = 100, PaymentStatus = "Paid" },
            new() { PaymentId = 2, ReservationId = 2, Amount = 160, PaymentStatus = "Refunded" }
        };

        _serviceMock.Setup(s => s.GetAllPaymentsAsync())
            .ReturnsAsync(payments);

        var result = await _controller.GetPayments();

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should()
            .BeOfType<ApiResponse<IEnumerable<Payment>>>()
            .Subject;

        response.Success.Should().BeTrue();
        response.StatusCode.Should().Be(200);
        response.Message.Should().Be("Payments fetched successfully.");
        response.Data.Should().BeEquivalentTo(payments);

        _serviceMock.Verify(s => s.GetAllPaymentsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetPayment_ShouldReturnOk_WhenPaymentExists()
    {
        var payment = new Payment
        {
            PaymentId = 1,
            ReservationId = 1,
            Amount = 100,
            PaymentStatus = "Paid"
        };

        _serviceMock.Setup(s => s.GetPaymentByIdAsync(1))
            .ReturnsAsync(payment);

        var result = await _controller.GetPayment(1);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should()
            .BeOfType<ApiResponse<Payment>>()
            .Subject;

        response.Success.Should().BeTrue();
        response.StatusCode.Should().Be(200);
        response.Message.Should().Be("Payment fetched successfully.");
        response.Data.Should().BeEquivalentTo(payment);

        _serviceMock.Verify(s => s.GetPaymentByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task CreatePayment_ShouldReturnCreated_WhenPaymentCreated()
    {
        var dto = new PaymentCreateDto
        {
            ReservationId = 2,
            Amount = 1000,
            PaymentStatus = "Success"
        };

        var payment = new Payment
        {
            PaymentId = 1002,
            ReservationId = 2,
            Amount = 1000,
            PaymentStatus = "Success"
        };

        _serviceMock.Setup(s => s.CreatePaymentAsync(dto))
            .ReturnsAsync(payment);

        var result = await _controller.CreatePayment(dto);

        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var response = createdResult.Value.Should()
            .BeOfType<ApiResponse<Payment>>()
            .Subject;

        createdResult.StatusCode.Should().Be(201);
        createdResult.ActionName.Should().Be(nameof(PaymentController.GetPayment));
        createdResult.RouteValues!["id"].Should().Be(payment.PaymentId);

        response.Success.Should().BeTrue();
        response.StatusCode.Should().Be(201);
        response.Message.Should().Be("Payment created successfully.");
        response.Data.Should().BeEquivalentTo(payment);

        _serviceMock.Verify(s => s.CreatePaymentAsync(dto), Times.Once);
    }

    [Fact]
    public async Task UpdatePayment_ShouldReturnOk_WhenPaymentUpdated()
    {
        var dto = new PaymentUpdateDto
        {
            Amount = 1500,
            PaymentStatus = "Paid"
        };

        var payment = new Payment
        {
            PaymentId = 1,
            ReservationId = 1,
            Amount = 1500,
            PaymentStatus = "Paid"
        };

        _serviceMock.Setup(s => s.UpdatePaymentAsync(1, dto))
            .ReturnsAsync(payment);

        var result = await _controller.UpdatePayment(1, dto);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should()
            .BeOfType<ApiResponse<Payment>>()
            .Subject;

        response.Success.Should().BeTrue();
        response.StatusCode.Should().Be(200);
        response.Message.Should().Be("Payment updated successfully.");
        response.Data.Should().BeEquivalentTo(payment);

        _serviceMock.Verify(s => s.UpdatePaymentAsync(1, dto), Times.Once);
    }

    // =========================
    // 4 NEGATIVE TEST CASES
    // =========================

    [Fact]
    public async Task GetPayment_ShouldThrowNotFoundException_WhenPaymentDoesNotExist()
    {
        _serviceMock.Setup(s => s.GetPaymentByIdAsync(99))
            .ReturnsAsync((Payment?)null);

        var act = async () => await _controller.GetPayment(99);

        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("Payment not found.");

        _serviceMock.Verify(s => s.GetPaymentByIdAsync(99), Times.Once);
    }

    [Fact]
    public async Task CreatePayment_ShouldThrowBadRequestException_WhenReservationDoesNotExist()
    {
        var dto = new PaymentCreateDto
        {
            ReservationId = 999,
            Amount = 1000,
            PaymentStatus = "Paid"
        };

        _serviceMock.Setup(s => s.CreatePaymentAsync(dto))
            .ReturnsAsync((Payment?)null);

        var act = async () => await _controller.CreatePayment(dto);

        await act.Should()
            .ThrowAsync<BadRequestException>()
            .WithMessage("Reservation does not exist.");

        _serviceMock.Verify(s => s.CreatePaymentAsync(dto), Times.Once);
    }

    [Fact]
    public async Task UpdatePayment_ShouldThrowNotFoundException_WhenPaymentDoesNotExist()
    {
        var dto = new PaymentUpdateDto
        {
            Amount = 1500,
            PaymentStatus = "Paid"
        };

        _serviceMock.Setup(s => s.UpdatePaymentAsync(99, dto))
            .ReturnsAsync((Payment?)null);

        var act = async () => await _controller.UpdatePayment(99, dto);

        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("Payment not found.");

        _serviceMock.Verify(s => s.UpdatePaymentAsync(99, dto), Times.Once);
    }

    [Fact]
    public async Task DeletePayment_ShouldThrowNotFoundException_WhenPaymentDoesNotExist()
    {
        _serviceMock.Setup(s => s.DeletePaymentAsync(99))
            .ReturnsAsync(false);

        var act = async () => await _controller.DeletePayment(99);

        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("Payment not found.");

        _serviceMock.Verify(s => s.DeletePaymentAsync(99), Times.Once);
    }
}