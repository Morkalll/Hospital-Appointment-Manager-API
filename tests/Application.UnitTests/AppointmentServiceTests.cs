using Moq;
using NUnit.Framework;
using TPI_2026.Application.Abstractions.Interfaces.Repositories;
using TPI_2026.Application.Exceptions;
using TPI_2026.Application.Services;
using TPI_2026.Domain.Entities;
using TPI_2026.Domain.Enums;

namespace Application.UnitTests;

[TestFixture]
public class AppointmentServiceTests
{
    // Removed _unitOfWorkMock
    private Mock<IRepository<Patient>> _patientRepoMock;
    private Mock<IRepository<Doctor>> _doctorRepoMock;
    private Mock<IRepository<Room>> _roomRepoMock;
    private Mock<IAppointmentRepository> _appointmentRepoMock;
    private AppointmentService _appointmentService;

    [SetUp]
    public void Setup()
    {
        // Removed _unitOfWorkMock init
        _patientRepoMock = new Mock<IRepository<Patient>>();
        _doctorRepoMock = new Mock<IRepository<Doctor>>();
        _roomRepoMock = new Mock<IRepository<Room>>();
        _appointmentRepoMock = new Mock<IAppointmentRepository>();

        // Removed _unitOfWorkMock setup

        _appointmentService = new AppointmentService(
            _appointmentRepoMock.Object,
            _patientRepoMock.Object,
            _doctorRepoMock.Object,
            _roomRepoMock.Object
        );
    }

    [Test]
    public void CreateAsync_WhenDateTimeIsInvalid_ThrowsValidationException()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var roomId = Guid.NewGuid();
        var dateTime = DateTime.UtcNow.AddDays(1).Date.AddHours(8); // 8 AM is invalid

        // Act & Assert
        var ex = Assert.ThrowsAsync<ValidationException>(async () =>
            await _appointmentService.CreateAsync(patientId, doctorId, roomId, dateTime));
            
        Assert.That(ex.Message, Is.EqualTo("Appointments can only be scheduled between 09:00 and 20:00."));
    }

    [Test]
    public async Task CreateAsync_WhenValid_Succeeds()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var roomId = Guid.NewGuid();
        var dateTime = DateTime.UtcNow.AddDays(1).Date.AddHours(10); // 10 AM is valid

        var patient = new Patient { Id = patientId, Name = "Test Patient" };
        var room = new Room { Id = roomId, Specialty = Specialty.Cardiology };
        var doctor = new Doctor { Id = doctorId, Specialty = Specialty.Cardiology, IsAvailable = true };

        _patientRepoMock.Setup(r => r.GetByIdAsync(patientId, It.IsAny<CancellationToken>())).ReturnsAsync(patient);
        _roomRepoMock.Setup(r => r.GetByIdAsync(roomId, It.IsAny<CancellationToken>())).ReturnsAsync(room);
        _doctorRepoMock.Setup(r => r.GetByIdAsync(doctorId, It.IsAny<CancellationToken>())).ReturnsAsync(doctor);

        _appointmentRepoMock.Setup(r => r.HasDoctorOverlapAsync(doctorId, dateTime, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _appointmentRepoMock.Setup(r => r.HasRoomOverlapAsync(roomId, dateTime, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        var resultId = await _appointmentService.CreateAsync(patientId, doctorId, roomId, dateTime);

        // Assert
        Assert.That(resultId, Is.Not.EqualTo(Guid.Empty));
        _appointmentRepoMock.Verify(r => r.AddAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
