using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TPI_2026.Domain.Enums;
using TPI_2026.Application.Abstractions.Interfaces.Services;
using TPI_2026.Application.Responses;
using TPI_2026.Application.Requests;



namespace TPI_2026.Presentation.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalHistoryController : ControllerBase
    {
        private readonly IMedicalHistoryService _MedicalHistoryService;

        public MedicalHistoryController(IMedicalHistoryService MedicalHistoryService)
        {
            _MedicalHistoryService = MedicalHistoryService;
        }

        [HttpGet("{patientId}")]
        [Authorize]
        public async Task<IActionResult> GetMedicalHistory(
            [FromRoute] Guid patientId,
            CancellationToken cancellationToken = default
        )
        {
            var medicalHistory = await _MedicalHistoryService.GetPatientByIdAsync(patientId, cancellationToken);
            return Ok(medicalHistory);
        }

        [HttpPut]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> UpdateDiagnostic(
            [FromBody] UpdateDiagnosticReq request,
            CancellationToken cancellationToken = default
        )
        {
            var medicalHistoryId = await _MedicalHistoryService.UpdateDiagnosticAsync(
                request.AppointmentId,
                request.Diagnostic,
                cancellationToken
            );
            return Ok(new { Id = medicalHistoryId });
        }
    }
}