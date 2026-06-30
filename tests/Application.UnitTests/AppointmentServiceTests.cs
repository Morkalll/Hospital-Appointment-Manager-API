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
    public void CreateAsync_WhenRoomMismatch_ThrowsValidationException()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var roomId = Guid.NewGuid();
        var otherRoomId = Guid.NewGuid();
        var dateTime = DateTime.UtcNow.AddDays(1);

        var patient = new Patient { Id = patientId, Name = "Test Patient" };
        var room = new Room { Id = roomId, Specialty = Specialty.Cardiology };
        var doctor = new Doctor { Id = doctorId, Specialty = Specialty.Cardiology, IsAvailable = true };

        _patientRepoMock.Setup(r => r.GetByIdAsync(patientId, It.IsAny<CancellationToken>())).ReturnsAsync(patient);
        _roomRepoMock.Setup(r => r.GetByIdAsync(roomId, It.IsAny<CancellationToken>())).ReturnsAsync(room);
        _doctorRepoMock.Setup(r => r.GetByIdAsync(doctorId, It.IsAny<CancellationToken>())).ReturnsAsync(doctor);

        // The appointment is in a DIFFERENT room
        var appointment = Appointment.CreateAvailable(doctorId, otherRoomId, dateTime);
        _appointmentRepoMock.Setup(r => r.GetAvailableAsync(doctorId, dateTime, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ValidationException>(async () =>
            await _appointmentService.CreateAsync(patientId, doctorId, roomId, dateTime));
            
        Assert.That(ex.Message, Is.EqualTo("The selected room does not match the available appointment."));
    }

    [Test]
    public async Task CreateAsync_WhenValid_Succeeds()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var roomId = Guid.NewGuid();
        var dateTime = DateTime.UtcNow.AddDays(1);

        var patient = new Patient { Id = patientId, Name = "Test Patient" };
        var room = new Room { Id = roomId, Specialty = Specialty.Cardiology };
        var doctor = new Doctor { Id = doctorId, Specialty = Specialty.Cardiology, IsAvailable = true };

        _patientRepoMock.Setup(r => r.GetByIdAsync(patientId, It.IsAny<CancellationToken>())).ReturnsAsync(patient);
        _roomRepoMock.Setup(r => r.GetByIdAsync(roomId, It.IsAny<CancellationToken>())).ReturnsAsync(room);
        _doctorRepoMock.Setup(r => r.GetByIdAsync(doctorId, It.IsAny<CancellationToken>())).ReturnsAsync(doctor);

        // The appointment is in the SAME room
        var appointment = Appointment.CreateAvailable(doctorId, roomId, dateTime);
        _appointmentRepoMock.Setup(r => r.GetAvailableAsync(doctorId, dateTime, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);

        // Act
        var resultId = await _appointmentService.CreateAsync(patientId, doctorId, roomId, dateTime);

        // Assert
        Assert.That(resultId, Is.Not.EqualTo(Guid.Empty));
        _appointmentRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.That(appointment.PatientId, Is.EqualTo(patientId));
    }
}
