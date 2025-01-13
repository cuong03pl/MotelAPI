using Microsoft.AspNetCore.Mvc;
using Motel.Interfaces;
using Motel.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Motel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        public IReportRepository _reportRepository { get; set; }
        public ReportsController(IReportRepository reportRepository) {
        _reportRepository = reportRepository;
        }
        [HttpGet]
        public IEnumerable<Reports> Get()
        {
            return _reportRepository.GetReports();
        }

        [HttpGet("{id}")]
        public Reports Get(string id)
        {
            return _reportRepository.GetReport(id);
        }

        [HttpPost]
        public void Post([FromForm] Reports reports)
        {
            _reportRepository.CreateReport(reports);
        }

        [HttpPut("{id}")]
        public void Put(string id, [FromForm] Reports reports)
        {
            _reportRepository.UpdateReport(id, reports);
        }

        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            _reportRepository.DeleteReport(id);
        }
    }
}
