using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TPI_2026.Application.Abstractions.Interfaces.Services;
using TPI_2026.Application.Requests;



namespace TPI_2026.Presentation.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalHistoryController : ControllerBase
    {
        private readonly IMedicalHistoryService _medicalHistoryService;

        public MedicalHistoryController(IMedicalHistoryService MedicalHistoryService)
        {
            _medicalHistoryService = MedicalHistoryService;
        }

        [HttpGet("{patientId}")]
        [Authorize]
        public async Task<IActionResult> GetMedicalHistory(
            [FromRoute] Guid patientId,
            CancellationToken cancellationToken = default
        )
        {
            var medicalHistory = await _medicalHistoryService.GetPatientMedicalHistoriesAsync(patientId, cancellationToken);
            return Ok(medicalHistory);
        }

        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> CreateDiagnostic(
            [FromBody] CreateMedicalHistoryReq request,
            CancellationToken cancellationToken = default
        )
        {
            var medicalHistoryId = await _medicalHistoryService.CreateMedicalHistoryAsync(
                request.AppointmentId,
                request.Diagnostic,
                cancellationToken
            );
            return Ok(new { Id = medicalHistoryId });
        }
    }
}