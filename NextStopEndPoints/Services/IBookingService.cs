using NextStopEndPoints.DTOs;

namespace NextStopEndPoints.Services
{
    public interface IBookingService
    {
        Task<IEnumerable<BusSearchResultDTO>> SearchBus(SearchBusDTO searchBusDto);
        Task<BookingDTO> BookTicket(BookTicketDTO bookTicketDto);
        Task<bool> CancelBooking(CancelBookingDTO cancelBookingDTO);
        Task<IEnumerable<BookingDTO>> ViewBookingsByUserId(ViewBookingsByUserIdDTO viewBookingsByUserIdDTO);
        Task<IEnumerable<BookingDTO>> ViewBookingsBySchdeuleId(ViewBookingsByScheduleIdDTO viewBookingsByScheduleIdDTO);
    }
}
