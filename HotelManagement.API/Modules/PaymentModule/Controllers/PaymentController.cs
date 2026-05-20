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
        try
        {
            var payments = await _service.GetAllPaymentsAsync();

            return Ok(new ApiResponse<IEnumerable<object>>
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Payments fetched successfully.",
                Data = payments.Select(p => new
                {
                    p.PaymentId,
                    p.ReservationId,
                    p.Amount,
                    p.PaymentStatus,
                    p.PaymentDate
                })
            });
        }
        catch (Exception ex)
        {
            throw new Exception(
                "An unexpected error occurred while fetching payments.",
                ex);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetPayment(int id)
    {
        try
        {
            if (id <= 0)
                throw new BadRequestException(
                    "Payment id must be greater than 0.");

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
        catch (BadRequestException)
        {
            throw;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception(
                "An unexpected error occurred while fetching payment.",
                ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreatePayment(PaymentCreateDto dto)
    {
        try
        {
            var payment = await _service.CreatePaymentAsync(dto);

            if (payment == null)
                throw new BadRequestException(
                    "Reservation does not exist.");

            return CreatedAtAction(
                nameof(GetPayment),
                new { id = payment.PaymentId },
                new ApiResponse<Payment>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status201Created,
                    Message = "Payment created successfully.",
                    Data = payment
                });
        }
        catch (BadRequestException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception(
                "An unexpected error occurred while creating payment.",
                ex);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdatePayment(
        int id,
        PaymentUpdateDto dto)
    {
        try
        {
            if (id <= 0)
                throw new BadRequestException(
                    "Payment id must be greater than 0.");

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
        catch (BadRequestException)
        {
            throw;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception(
                "An unexpected error occurred while updating payment.",
                ex);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePayment(int id)
    {
        try
        {
            if (id <= 0)
                throw new BadRequestException(
                    "Payment id must be greater than 0.");

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
        catch (BadRequestException)
        {
            throw;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception(
                "An unexpected error occurred while deleting payment.",
                ex);
        }
    }

    [HttpGet("by-reservation/{reservationId:int}")]
    public async Task<IActionResult> GetPaymentsByReservation(
        int reservationId)
    {
        try
        {
            if (reservationId <= 0)
                throw new BadRequestException(
                    "Reservation id must be greater than 0.");

            var payments =
                await _service.GetPaymentsByReservationAsync(reservationId);

            return Ok(new ApiResponse<IEnumerable<Payment>>
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Reservation payments fetched successfully.",
                Data = payments
            });
        }
        catch (BadRequestException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception(
                "An unexpected error occurred while fetching reservation payments.",
                ex);
        }
    }

    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdatePaymentStatus(
        int id,
        PaymentStatusUpdateDto dto)
    {
        try
        {
            if (id <= 0)
                throw new BadRequestException(
                    "Payment id must be greater than 0.");

            var payment =
                await _service.UpdatePaymentStatusAsync(id, dto);

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
        catch (BadRequestException)
        {
            throw;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception(
                "An unexpected error occurred while updating payment status.",
                ex);
        }
    }

    [HttpGet("successful")]
    public async Task<IActionResult> GetSuccessfulPayments()
    {
        try
        {
            var payments =
                await _service.GetSuccessfulPaymentsAsync();

            return Ok(new ApiResponse<IEnumerable<Payment>>
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Successful payments fetched successfully.",
                Data = payments
            });
        }
        catch (Exception ex)
        {
            throw new Exception(
                "An unexpected error occurred while fetching successful payments.",
                ex);
        }
    }

    [HttpGet("failed")]
    public async Task<IActionResult> GetFailedPayments()
    {
        try
        {
            var payments =
                await _service.GetFailedPaymentsAsync();

            return Ok(new ApiResponse<IEnumerable<Payment>>
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Failed payments fetched successfully.",
                Data = payments
            });
        }
        catch (Exception ex)
        {
            throw new Exception(
                "An unexpected error occurred while fetching failed payments.",
                ex);
        }
    }

    [HttpPost("{id:int}/refund")]
    public async Task<IActionResult> RefundPayment(int id)
    {
        try
        {
            if (id <= 0)
                throw new BadRequestException(
                    "Payment id must be greater than 0.");

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
        catch (BadRequestException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception(
                "An unexpected error occurred while refunding payment.",
                ex);
        }
    }
}