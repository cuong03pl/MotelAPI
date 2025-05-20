using Motel.DTO;
using Motel.Models;

namespace Motel.Interfaces
{
    public interface IBookingRepository
    {
       object GetBookings(int page, int pageSize);
        object GetUserBookings(Guid userId);
        object GetBookingByPost(string postId);
        public void CreateBooking(Bookings bookings);
        public Bookings GetBooking(string slug);
        public long GetCount();
        byte[] GeneratePdfReport();
        bool HasPayed( string postId);
        public bool UpdateStatus(Guid userId, string postId);
    }
}
