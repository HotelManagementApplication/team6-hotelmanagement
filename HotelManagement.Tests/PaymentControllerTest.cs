using FluentAssertions;
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

    #region POSITIVE TEST CASES

    [Fact]
    public async Task GetPayments_ShouldReturnOk_WhenPaymentsExist()
    {
        var payments = new List<Payment>
        {
            new() { PaymentId = 1, ReservationId = 11, Amount = 150m, PaymentStatus = "Completed" },
            new() { PaymentId = 2, ReservationId = 12, Amount = 250m, PaymentStatus = "Pending" }
        };

        _serviceMock.Setup(service => service.GetAllPaymentsAsync()).ReturnsAsync(payments);

        var result = await _controller.GetPayments();

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeSameAs(payments);
        _serviceMock.Verify(service => service.GetAllPaymentsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetPayment_ShouldReturnOk_WhenPaymentExists()
    {
        const int paymentId = 1;
        var payment = new Payment { PaymentId = paymentId, ReservationId = 5, Amount = 300m, PaymentStatus = "Completed" };

        _serviceMock.Setup(service => service.GetPaymentByIdAsync(paymentId)).ReturnsAsync(payment);

        var result = await _controller.GetPayment(paymentId);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeSameAs(payment);
        _serviceMock.Verify(service => service.GetPaymentByIdAsync(paymentId), Times.Once);
    }

    [Fact]
    public async Task CreatePayment_ShouldReturnCreated_WhenReservationExists()
    {
        var dto = new PaymentCreateDto { ReservationId = 8, Amount = 125m, PaymentStatus = "Pending" };
        var createdPayment = new Payment { PaymentId = 10, ReservationId = dto.ReservationId, Amount = dto.Amount, PaymentStatus = dto.PaymentStatus };

        _serviceMock.Setup(service => service.CreatePaymentAsync(dto)).ReturnsAsync(createdPayment);

        var result = await _controller.CreatePayment(dto);

        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.StatusCode.Should().Be(201);
        createdResult.ActionName.Should().Be(nameof(PaymentController.GetPayment));
        createdResult.RouteValues.Should().ContainKey("id");
        createdResult.RouteValues!["id"].Should().Be(createdPayment.PaymentId);
        createdResult.Value.Should().BeSameAs(createdPayment);
        _serviceMock.Verify(service => service.CreatePaymentAsync(dto), Times.Once);
    }

    [Fact]
    public async Task UpdatePayment_ShouldReturnOk_WhenPaymentExists()
    {
        const int paymentId = 2;
        var dto = new PaymentUpdateDto { Amount = 175m, PaymentStatus = "Completed" };
        var updatedPayment = new Payment { PaymentId = paymentId, ReservationId = 9, Amount = dto.Amount, PaymentStatus = dto.PaymentStatus };

        _serviceMock.Setup(service => service.UpdatePaymentAsync(paymentId, dto)).ReturnsAsync(updatedPayment);

        var result = await _controller.UpdatePayment(paymentId, dto);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeSameAs(updatedPayment);
        _serviceMock.Verify(service => service.UpdatePaymentAsync(paymentId, dto), Times.Once);
    }

    #endregion

    #region NEGATIVE TEST CASES

    [Fact]
    public async Task GetPayment_ShouldReturnNotFound_WhenPaymentDoesNotExist()
    {
        const int paymentId = 99;

        _serviceMock.Setup(service => service.GetPaymentByIdAsync(paymentId)).ReturnsAsync((Payment?)null);

        var result = await _controller.GetPayment(paymentId);

        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.StatusCode.Should().Be(404);
        notFoundResult.Value.Should().Be("Payment not found.");
        _serviceMock.Verify(service => service.GetPaymentByIdAsync(paymentId), Times.Once);
    }

    [Fact]
    public async Task CreatePayment_ShouldReturnBadRequest_WhenReservationDoesNotExist()
    {
        var dto = new PaymentCreateDto { ReservationId = 404, Amount = 200m, PaymentStatus = "Pending" };

        _serviceMock.Setup(service => service.CreatePaymentAsync(dto)).ReturnsAsync((Payment?)null);

        var result = await _controller.CreatePayment(dto);

        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.StatusCode.Should().Be(400);
        badRequestResult.Value.Should().Be("Reservation does not exist.");
        _serviceMock.Verify(service => service.CreatePaymentAsync(dto), Times.Once);
    }

    [Fact]
    public async Task UpdatePayment_ShouldReturnNotFound_WhenPaymentDoesNotExist()
    {
        const int paymentId = 77;
        var dto = new PaymentUpdateDto { Amount = 210m, PaymentStatus = "Completed" };

        _serviceMock.Setup(service => service.UpdatePaymentAsync(paymentId, dto)).ReturnsAsync((Payment?)null);

        var result = await _controller.UpdatePayment(paymentId, dto);

        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.StatusCode.Should().Be(404);
        notFoundResult.Value.Should().Be("Payment not found.");
        _serviceMock.Verify(service => service.UpdatePaymentAsync(paymentId, dto), Times.Once);
    }

    [Fact]
    public async Task DeletePayment_ShouldReturnNotFound_WhenDeleteFails()
    {
        const int paymentId = 55;

        _serviceMock.Setup(service => service.DeletePaymentAsync(paymentId)).ReturnsAsync(false);

        var result = await _controller.DeletePayment(paymentId);

        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.StatusCode.Should().Be(404);
        notFoundResult.Value.Should().Be("Payment not found.");
        _serviceMock.Verify(service => service.DeletePaymentAsync(paymentId), Times.Once);
    }

    #endregion
}