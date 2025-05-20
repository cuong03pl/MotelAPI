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
    public class ReportsController : ControllerBase
    {
        public IReportRepository _reportRepository { get; set; }
        public ReportsController(IReportRepository reportRepository) {
        _reportRepository = reportRepository;
        }
        [HttpGet]
        public async Task<object> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 1)
        {
            return await _reportRepository.GetReports(page, pageSize);
        }

        [HttpGet("{id}")]
        public ReportsDTO Get(string id)
        {
            return _reportRepository.GetReport(id);
        }

        [HttpPost]
        public void Post([FromBody] Reports reports)
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

        [HttpPut("Browse")]
        public void Browse(string id)
        {
            _reportRepository.Browse(id);
        }
        [HttpGet("GetCount")]
        public long GetCount()
        {
            return _reportRepository.CountReport();
        }

        [HttpGet("GetReportsByReason")]
        public async Task<ActionResult<List<ReportReasonCount>>> GetReportsByReason()
        {
            var reportsByReason = await _reportRepository.GetReportsByReason();
            return Ok(reportsByReason);
        }

        [HttpGet("export-pdf")]
        public IActionResult ExportPdf()
        {
            var fileBytes = _reportRepository.GeneratePdfReport();
            return File(fileBytes, "application/pdf", "Reports.pdf");
        }
    }
}
