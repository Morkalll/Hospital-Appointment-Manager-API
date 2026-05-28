using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TPI_2026.Domain.Enums;
using TPI_2026.Application.Abstractions.Interfaces.Services;



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
        public async Task<IActionResult> GetMedicalHistory(
            [FromRoute] Guid patientId,
            CancellationToken cancellationToken = default
        )
        {
            var medicalHistory = await _MedicalHistoryService.GetPatientByIdAsync(patientId, cancellationToken);
            return Ok(medicalHistory);
        }


    }
}