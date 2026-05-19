using AutoMapper;
using HotelManagement.API.Modules.PaymentModule.DTOs;
using HotelManagement.API.Modules.PaymentModule.Services;
using HotelManagement.API.Modules.ReservationModule.DTOs;
using HotelManagement.API.Modules.ReservationModule.Exceptions;
using HotelManagement.API.Modules.ReservationModule.Repositories;
using HotelManagement.API.Modules.RoomModule.Services;
using HotelManagement.API.Modules.RoomTypeModule.Services;
using HotelManagement.Common.Models;
using Microsoft.AspNetCore.Identity;

namespace HotelManagement.API.Modules.ReservationModule.Services;

public class ReservationService(
    IReservationRepository reservationRepository, 
    IRoomService roomService, 
    IRoomTypeService roomTypeService, 
    IPaymentService paymentService,
    IMapper mapper,
    UserManager<ApplicationUser> userManager) : IReservationService
{
    private readonly IReservationRepository _reservationRepository = reservationRepository;
    private readonly IRoomService _roomService = roomService;
    private readonly IRoomTypeService _roomTypeService = roomTypeService;
    private readonly IPaymentService _paymentService = paymentService;
    private readonly IMapper _mapper = mapper;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<ReservationDetailsDto> GetReservationDetailsAsync(int reservationId)
    {
        var reservation = await _reservationRepository.GetReservationByIdAsync(reservationId);
        if (reservation == null)
            throw new ReservationNotFoundException(reservationId);

        return await ToReservationDetailsDto(reservation);
    }

    public async Task<ReservationDetailsDto> CreateReservationAsync(CreateReservationDto reservationDto)
    {
        var reservation = _mapper.Map<Reservation>(reservationDto);
        var user = await _userManager.FindByEmailAsync(reservationDto.GuestEmail);
        if (user == null)
            throw new InvalidOperationException($"User with email '{reservationDto.GuestEmail}' does not exist.");

        reservation.ApplicationUsers = new List<ApplicationUser> { user };

        var room = await _roomService.GetRoomDetailsByIdAsync(reservationDto.RoomId)
            ?? throw new ReservationNotFoundException(reservation.ReservationId);
        var roomType = await _roomTypeService.GetRoomTypeDetailsByIdAsync(room.RoomTypeId ?? 0)
            ?? throw new ReservationNotFoundException(reservation.ReservationId);
        
        var createdReservation = await _reservationRepository.CreateReservationAsync(reservation);
        var paymentDetails = new PaymentCreateDto
        {
            ReservationId = createdReservation.ReservationId,
            Amount = roomType.PricePerNight ?? 0m,
            PaymentStatus = "Success"
        };

        await _paymentService.CreatePaymentAsync(paymentDetails);
        return await ToReservationDetailsDto(reservation);
    }

    public async Task<bool> HasReservationDateConflictAsync(int roomId, DateOnly checkIn, DateOnly checkOut)
        => await _reservationRepository.HasReservationDateConflictAsync(roomId, checkIn, checkOut);

    private async Task<ReservationDetailsDto> ToReservationDetailsDto(Reservation reservation)
    {
        var roomId =  reservation.RoomId ?? 
                      throw new ReservationNotFoundException(reservation.ReservationId);
        var room = await _roomService.GetRoomDetailsByIdAsync(roomId)
            ?? throw new ReservationNotFoundException(reservation.ReservationId);
        var roomType = await _roomTypeService.GetRoomTypeDetailsByIdAsync(room.RoomTypeId ?? 0)
            ?? throw new ReservationNotFoundException(reservation.ReservationId);
        var amenities = await _roomService.GetAmenitiesByRoomIdAsync(roomId);
        var paymentDetails = await _paymentService.GetPaymentDetailsByReservationIdAsync(reservation.ReservationId);

        var reservationDetails = new ReservationDetailsDto
        {
            ReservationId = reservation.ReservationId,
            GuestName = reservation.GuestName ?? "",
            GuestEmail = reservation.GuestEmail ?? "",
            GuestPhoneNumber = reservation.GuestPhone ?? "",
            CheckInDate = reservation.CheckInDate ?? new DateOnly(),
            CheckOutDate = reservation.CheckOutDate ?? new DateOnly(),
            BookingDate = paymentDetails?.PaymentDate ?? new DateOnly(),
            RoomNumber = room.RoomNumber ?? 0,
            RoomTypeId = room.RoomTypeId ?? 0,
            RoomType = roomType.TypeName,
            Amenities = amenities,
            PricePerNight = roomType.PricePerNight ?? 0,
            HotelId = 0,
            HotelName = string.Empty,
            HotelLocation = string.Empty,
            TotalPrice = paymentDetails?.Amount ?? 0
        };
        
        return reservationDetails;
    }

    public async Task<List<ReservationDetailsDto>> GetReservationsByUserEmailAsync(string userEmail) 
    {
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null)
            throw new InvalidOperationException($"User with email '{userEmail}' does not exist.");

        var userId = user.Id;
        var reservations = await _reservationRepository.GetReservationsByUserIdAsync(userId);
        var result = new List<ReservationDetailsDto>();

        foreach (var reservation in reservations)
        {
            var data = await ToReservationDetailsDto(reservation);
            result.Add(data);
        }

        return result;
    }
}
