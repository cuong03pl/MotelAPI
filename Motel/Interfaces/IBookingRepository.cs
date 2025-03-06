using Motel.DTO;
using Motel.Models;

namespace Motel.Interfaces
{
    public interface IBookingRepository
    {
       object GetBookings(int page, int pageSize);
        object GetUserBookings(Guid userId);
        public void CreateBooking(Bookings bookings);
        public Bookings GetBooking(string slug);
        public long GetCount();
        bool HasUserBooked(Guid userId, string postId);
    }
}
