using HotelManagement.API.DTOs;
using HotelManagement.API.Exceptions;
using HotelManagement.API.Modules.PaymentModule.DTOs;
using HotelManagement.API.Modules.PaymentModule.Services;
using HotelManagement.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.API.Modules.PaymentModule.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentController(IPaymentService service) : ControllerBase
{
    private readonly IPaymentService _service = service;

    [HttpGet]
    public async Task<IActionResult> GetPayments()
    {
        var payments = await _service.GetAllPaymentsAsync();

        return Ok(new ApiResponse<IEnumerable<Payment>>
        {
            Success = true,
            StatusCode = StatusCodes.Status200OK,
            Message = "Payments fetched successfully.",
            Data = payments
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPayment(int id)
    {
        var payment = await _service.GetPaymentByIdAsync(id);

        if (payment == null)
            throw new NotFoundException("Payment not found.");

        return Ok(new ApiResponse<Payment>
        {
            Success = true,
            StatusCode = StatusCodes.Status200OK,
            Message = "Payment fetched successfully.",
            Data = payment
        });
    }

    [HttpPost]
    public async Task<IActionResult> CreatePayment(PaymentCreateDto dto)
    {
        var payment = await _service.CreatePaymentAsync(dto);

        if (payment == null)
            throw new BadRequestException("Reservation does not exist.");

        var response = new ApiResponse<Payment>
        {
            Success = true,
            StatusCode = StatusCodes.Status201Created,
            Message = "Payment created successfully.",
            Data = payment
        };

        return CreatedAtAction(
            nameof(GetPayment),
            new { id = payment.PaymentId },
            response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePayment(int id, PaymentUpdateDto dto)
    {
        var payment = await _service.UpdatePaymentAsync(id, dto);

        if (payment == null)
            throw new NotFoundException("Payment not found.");

        return Ok(new ApiResponse<Payment>
        {
            Success = true,
            StatusCode = StatusCodes.Status200OK,
            Message = "Payment updated successfully.",
            Data = payment
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePayment(int id)
    {
        var result = await _service.DeletePaymentAsync(id);

        if (!result)
            throw new NotFoundException("Payment not found.");

        return Ok(new ApiResponse<object>
        {
            Success = true,
            StatusCode = StatusCodes.Status200OK,
            Message = "Payment deleted successfully.",
            Data = null
        });
    }

    [HttpGet("by-reservation/{reservationId}")]
    public async Task<IActionResult> GetPaymentsByReservation(int reservationId)
    {
        var payments = await _service.GetPaymentsByReservationAsync(reservationId);

        return Ok(new ApiResponse<IEnumerable<Payment>>
        {
            Success = true,
            StatusCode = StatusCodes.Status200OK,
            Message = "Reservation payments fetched successfully.",
            Data = payments
        });
    }

    [HttpGet("successful")]
    public async Task<IActionResult> GetSuccessfulPayments()
    {
        var payments = await _service.GetSuccessfulPaymentsAsync();

        return Ok(new ApiResponse<IEnumerable<Payment>>
        {
            Success = true,
            StatusCode = StatusCodes.Status200OK,
            Message = "Successful payments fetched successfully.",
            Data = payments
        });
    }

    [HttpGet("failed")]
    public async Task<IActionResult> GetFailedPayments()
    {
        var payments = await _service.GetFailedPaymentsAsync();

        return Ok(new ApiResponse<IEnumerable<Payment>>
        {
            Success = true,
            StatusCode = StatusCodes.Status200OK,
            Message = "Failed payments fetched successfully.",
            Data = payments
        });
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdatePaymentStatus(
        int id,
        PaymentStatusUpdateDto dto)
    {
        var payment = await _service.UpdatePaymentStatusAsync(id, dto);

        if (payment == null)
            throw new NotFoundException("Payment not found.");

        return Ok(new ApiResponse<Payment>
        {
            Success = true,
            StatusCode = StatusCodes.Status200OK,
            Message = "Payment status updated successfully.",
            Data = payment
        });
    }

    [HttpPost("{id}/refund")]
    public async Task<IActionResult> RefundPayment(int id)
    {
        var payment = await _service.RefundPaymentAsync(id);

        if (payment == null)
            throw new BadRequestException(
                "Payment not found or payment is not eligible for refund.");

        return Ok(new ApiResponse<Payment>
        {
            Success = true,
            StatusCode = StatusCodes.Status200OK,
            Message = "Payment refunded successfully.",
            Data = payment
        });
    }
}