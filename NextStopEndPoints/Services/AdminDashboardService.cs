using Microsoft.EntityFrameworkCore;
using NextStopEndPoints.Data;
using NextStopEndPoints.DTOs;
using NextStopEndPoints.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextStopEndPoints.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly NextStopDbContext _context;

        public AdminDashboardService(NextStopDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AssignRole(AssignRoleDTO assignRoleDto)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == assignRoleDto.UserId);

                if (user == null)
                {
                    throw new InvalidOperationException("User not found.");
                }

                user.Role = assignRoleDto.Role;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error assigning role: {ex.Message}");
            }
        }

        public async Task<ReportDTO> GenerateReports(GenerateReportsDTO generateReportsDto)
        {
            try
            {
                // Filter bookings by date range, route, and operator
                var bookingsQuery = _context.Bookings
                    .Include(b => b.Schedule)
                    .ThenInclude(s => s.Route)
                    .Include(b => b.Schedule.Bus)
                    .Include(b => b.Seats)
                    .Where(b => b.Status != "cancelled")
                    .AsQueryable();

                if (generateReportsDto.StartDate.HasValue && generateReportsDto.EndDate.HasValue)
                {
                    bookingsQuery = bookingsQuery.Where(b =>
                        b.BookingDate >= generateReportsDto.StartDate.Value &&
                        b.BookingDate <= generateReportsDto.EndDate.Value);
                }

                if (!string.IsNullOrEmpty(generateReportsDto.Route))
                {
                    bookingsQuery = bookingsQuery.Where(b =>
                        b.Schedule.Route.Origin + "-" + b.Schedule.Route.Destination == generateReportsDto.Route);
                }

                if (!string.IsNullOrEmpty(generateReportsDto.Operator))
                {
                    bookingsQuery = bookingsQuery.Where(b => b.Schedule.Bus.Operator.Name == generateReportsDto.Operator);
                }

                var bookings = await bookingsQuery.ToListAsync();

                // Generate total bookings and revenue
                var totalBookings = bookings.Count;
                var totalRevenue = bookings.Sum(b => b.TotalFare); 

                // Create booking details
                var bookingDetails = bookings.Select(b => new BookingDetailDTO
                {
                    BookingId = b.BookingId,
                    UserId = b.UserId,
                    ScheduleId = b.ScheduleId,
                    ReservedSeats = b.Seats.Select(s => s.SeatNumber).ToList(),
                    TotalFare = b.TotalFare,
                    Status = b.Status,
                    BookingDate = b.BookingDate
                }).ToList();

                return new ReportDTO
                {
                    TotalBookings = totalBookings,
                    TotalRevenue = totalRevenue,
                    Route = generateReportsDto.Route,
                    Operator = generateReportsDto.Operator,
                    BookingDetails = bookingDetails
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating reports: {ex.Message}");
            }
        }
    }
}
