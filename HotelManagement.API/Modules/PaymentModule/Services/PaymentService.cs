using HotelManagement.API.Modules.PaymentModule.DTOs;
using HotelManagement.Common.Models;
using HotelManagement.API.Modules.PaymentModule.Repositories;
using HotelManagement.API.Exceptions;

namespace HotelManagement.API.Modules.PaymentModule.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _repository;

    public PaymentService(IPaymentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Payment?> GetPaymentByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Payment?> CreatePaymentAsync(PaymentCreateDto dto)
    {
        var reservationExists = await _repository.ReservationExistsAsync(dto.ReservationId);

        if (!reservationExists)
            return null; 

        var payment = new Payment
        {
            ReservationId = dto.ReservationId,
            Amount = dto.Amount,
            PaymentDate = DateOnly.FromDateTime(DateTime.Now),
            PaymentStatus = dto.PaymentStatus
        };

        return await _repository.AddAsync(payment);
    }

    public async Task<Payment?> UpdatePaymentAsync(int id, PaymentUpdateDto dto)
    {
        var payment = await _repository.GetByIdAsync(id);

        if (payment == null)
            return null;

        payment.Amount = dto.Amount;
        payment.PaymentStatus = dto.PaymentStatus;

        return await _repository.UpdateAsync(payment);
    }

    public async Task<bool> DeletePaymentAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<Payment>> GetPaymentsByReservationAsync(int reservationId)
    {
        return await _repository.GetByReservationAsync(reservationId);
    }

    public async Task<IEnumerable<Payment>> GetSuccessfulPaymentsAsync()
    {
        var payments = await _repository.GetAllAsync();

        return payments.Where(p =>
            p.PaymentStatus == "Paid" ||
            p.PaymentStatus == "Success" ||
            p.PaymentStatus == "Successful");
    }

    public async Task<IEnumerable<Payment>> GetFailedPaymentsAsync()
    {
        var payments = await _repository.GetAllAsync();

        return payments.Where(p =>
            p.PaymentStatus == "Pending" ||
            p.PaymentStatus == "Failed" ||
            string.IsNullOrWhiteSpace(p.PaymentStatus) ||
            p.PaymentStatus == "string");
    }

    public async Task<Payment?> UpdatePaymentStatusAsync(int id, PaymentStatusUpdateDto dto)
    {
        var payment = await _repository.GetByIdAsync(id);

        if (payment == null)
            return null;

        payment.PaymentStatus = dto.PaymentStatus;

        return await _repository.UpdateAsync(payment);
    }

    public async Task<Payment?> RefundPaymentAsync(int id)
    {
        var payment = await _repository.GetByIdAsync(id);

        if (payment == null)
            return null;

        if (payment.PaymentStatus != "Paid" &&
            payment.PaymentStatus != "Success" &&
            payment.PaymentStatus != "Successful")
            return null;

        payment.PaymentStatus = "Refunded";

        return await _repository.UpdateAsync(payment);
    }

    public async Task<PaymentDetailsDto?> GetPaymentDetailsByReservationIdAsync(int reservationId)
    {
        var payment = (await _repository.GetByReservationAsync(reservationId))
            .OrderByDescending(p => p.PaymentDate)
            .FirstOrDefault();

        return payment is null
            ? null
            : new PaymentDetailsDto
            {
                ReservationId = payment.ReservationId ?? 0,
                Amount = payment.Amount ?? 0m,
                PaymentDate = payment.PaymentDate ?? DateOnly.MinValue,
                PaymentStatus = payment.PaymentStatus ?? string.Empty
            };
    }
}