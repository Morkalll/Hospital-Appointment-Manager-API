using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System.Linq.Expressions;
using TPI_2026.Application.Abstractions.Interfaces.Repositories;
using TPI_2026.Application.Exceptions;
using TPI_2026.Application.Services;
using TPI_2026.Domain.Entities;

namespace Application.UnitTests;

[TestFixture]
public class AuthServiceTests
{
    // Removed _unitOfWorkMock
    private Mock<IPasswordHasher<Doctor>> _doctorHasherMock;
    private Mock<IPasswordHasher<Receptionist>> _receptionistHasherMock;
    private Mock<IPasswordHasher<Administrator>> _adminHasherMock;
    private Mock<IConfiguration> _configMock;

    private Mock<IRepository<Doctor>> _doctorRepoMock;
    private Mock<IRepository<Receptionist>> _receptionistRepoMock;
    private Mock<IRepository<Administrator>> _adminRepoMock;
    private Mock<IRepository<Patient>> _patientRepoMock;

    private AuthService _authService;

    [SetUp]
    public void Setup()
    {
        // Removed _unitOfWorkMock init
        _doctorHasherMock = new Mock<IPasswordHasher<Doctor>>();
        _receptionistHasherMock = new Mock<IPasswordHasher<Receptionist>>();
        _adminHasherMock = new Mock<IPasswordHasher<Administrator>>();
        _configMock = new Mock<IConfiguration>();

        _doctorRepoMock = new Mock<IRepository<Doctor>>();
        _receptionistRepoMock = new Mock<IRepository<Receptionist>>();
        _adminRepoMock = new Mock<IRepository<Administrator>>();
        _patientRepoMock = new Mock<IRepository<Patient>>();

        // Removed _unitOfWorkMock setup

        _configMock.Setup(c => c["Jwt:Key"]).Returns("ThisIsASecretKeyForTestingPurposesOnly");
        _configMock.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
        _configMock.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");

        _authService = new AuthService(
            _doctorRepoMock.Object,
            _receptionistRepoMock.Object,
            _adminRepoMock.Object,
            _patientRepoMock.Object,
            _doctorHasherMock.Object,
            _receptionistHasherMock.Object,
            _adminHasherMock.Object,
            _configMock.Object
        );
    }

    [Test]
    public void LoginAsync_WhenPatientAttemptsLogin_ThrowsForbiddenException()
    {
        // Arrange
        var email = "patient@test.com";
        var password = "password123";

        _doctorRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Doctor, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Doctor?)null);
        _receptionistRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Receptionist, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Receptionist?)null);
        _adminRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Administrator, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Administrator?)null);

        _patientRepoMock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Patient, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ForbiddenException>(async () =>
            await _authService.LoginAsync(email, password));

        Assert.That(ex.Message, Is.EqualTo("Patients cannot log in to the system."));
    }

    [Test]
    public void LoginAsync_WhenUserNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var email = "unknown@test.com";
        var password = "password123";

        _doctorRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Doctor, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Doctor?)null);
        _receptionistRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Receptionist, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Receptionist?)null);
        _adminRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Administrator, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Administrator?)null);

        _patientRepoMock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Patient, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () =>
            await _authService.LoginAsync(email, password));
    }
}
