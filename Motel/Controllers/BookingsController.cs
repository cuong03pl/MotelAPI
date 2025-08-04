using Microsoft.AspNetCore.Mvc;
using Motel.DTO;
using Motel.Interfaces;
using Motel.Models;
using Motel.Repository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Motel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        public IBookingRepository _bookingRepository { get; set; }
        public BookingsController(IBookingRepository bookingRepository) {
            _bookingRepository = bookingRepository;
        }

        [HttpGet]
        public object Get(int page, int pageSize)
        {
            return _bookingRepository.GetBookings(page, pageSize);
        }


        // [HttpGet("GetUserBookings/{id}")]
        [HttpGet("users/{id}/bookings")]
        public object GetUserBookings(Guid id)
        {
            return _bookingRepository.GetUserBookings(id);
        }

        // [HttpGet("GetBookingByPost/{postId}")]
        [HttpGet("posts/{postId}/bookings")]
        public object GetBookingByPost(string postId)
        {
            return _bookingRepository.GetBookingByPost(postId);
        }

       

        // [HttpGet("CheckPayed")]
        [HttpGet("check-payment")]
        public bool CheckPayed([FromQuery] string postId)
        {
            return _bookingRepository.HasPayed(postId);
        }

        // POST api/<BookingController>
        [HttpPost]
        public void Post([FromBody] Bookings booking)
        {
            _bookingRepository.CreateBooking(booking);
        }

        // PUT api/<BookingController>/5
        // [HttpPut("UpdateStatus")]
        [HttpPut("users/{userId}/posts/{postId}/status")]
        public bool Put(Guid userId, string postId)
        {
            var result = _bookingRepository.UpdateStatus(userId, postId);
            return result;
        }

        // DELETE api/<BookingController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpGet("export-pdf")]
        public IActionResult ExportPdf()
        {
            var fileBytes = _bookingRepository.GeneratePdfReport();
            return File(fileBytes, "application/pdf", "Bookings.pdf");
        }
    }
}
