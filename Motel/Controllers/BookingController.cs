using Microsoft.AspNetCore.Mvc;
using Motel.DTO;
using Motel.Interfaces;
using Motel.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Motel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        public IBookingRepository _bookingRepository { get; set; }
        public BookingController(IBookingRepository bookingRepository) {
            _bookingRepository = bookingRepository;
        }

        [HttpGet]
        public object Get(int page, int pageSize)
        {
            return _bookingRepository.GetBookings(page, pageSize);
        }


        [HttpGet("GetUserBookings/{id}")]
        public object GetUserBookings(Guid id)
        {
            return _bookingRepository.GetUserBookings(id);
        }
        // GET api/<BookingController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpGet("CheckUserBooking")]
        public bool CheckUserBooking( Guid userId, string postId)
        {
            return _bookingRepository.HasUserBooked(userId, postId);
        }

        // POST api/<BookingController>
        [HttpPost]
        public void Post([FromBody] Bookings booking)
        {
            _bookingRepository.CreateBooking(booking);
        }

        // PUT api/<BookingController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<BookingController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
