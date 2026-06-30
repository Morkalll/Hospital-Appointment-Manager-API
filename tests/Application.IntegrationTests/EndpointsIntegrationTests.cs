using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using System.Net;
using System.Net.Http.Json;
using TPI_2026.Application.Requests;
using TPI_2026.Application.Responses;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;

namespace Application.IntegrationTests;

[TestFixture]
public class EndpointsIntegrationTests
{
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;
    private string? _doctorToken;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
        });
        _client = _factory.CreateClient();

        // Login as Admin to get the token since endpoints are protected
        var loginReq = new LoginReq("admin@hospital.com", "Admin1234!");
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginReq);
        if (loginResponse.IsSuccessStatusCode)
        {
            var authResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResult!.Token);
        }
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    // 1. Create Patient
    [Test, Order(1)]
    public async Task CreatePatient_ReturnsOk()
    {
        var req = new CreatePatientReq("Test Patient", $"patient{Guid.NewGuid()}@test.com", "11111111", new DateOnly(1990, 1, 1), "12345678", "Address");
        var response = await _client.PostAsJsonAsync("/api/User/create-patient", req);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    // 2. Create Doctor
    [Test, Order(2)]
    public async Task CreateDoctor_ReturnsOk()
    {
        var email = $"doctor{Guid.NewGuid()}@test.com";
        var req = new CreateDoctorReq("Test Doctor", email, "Password123!", "CRED123", TPI_2026.Domain.Enums.Specialty.Cardiology, "555-5555");
        var response = await _client.PostAsJsonAsync("/api/User/create-doctor", req);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        // 4. Login Doctor for subsequent tests
        var loginReq = new LoginReq(email, "Password123!");
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginReq);
        Assert.That(loginResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var authResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        _doctorToken = authResult!.Token;
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _doctorToken);
    }

    // 3. Create Receptionist
    [Test, Order(3)]
    public async Task CreateReceptionist_ReturnsOk()
    {
        var req = new CreateReceptionistReq("Test Receptionist", $"rec{Guid.NewGuid()}@test.com", "Password123!", "EMP001", "Morning", "Front");
        var response = await _client.PostAsJsonAsync("/api/User/create-receptionist", req);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    // 5. Get All Users
    [Test, Order(4)]
    public async Task GetAllUsers_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/User");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    // 8. Get Rooms
    [Test, Order(5)]
    public async Task GetRooms_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/Room");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    // Other endpoints can be complex since they require specific DB states
    // but we can test that they return appropriate status codes instead of 500
    [Test, Order(6)]
    public async Task GetUserById_ReturnsNotFoundOrOk()
    {
        var response = await _client.GetAsync($"/api/User/{Guid.NewGuid()}");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound).Or.EqualTo(HttpStatusCode.OK));
    }

    [Test, Order(7)]
    public async Task DeleteUser_ReturnsNotFoundOrOk()
    {
        var response = await _client.DeleteAsync($"/api/User/{Guid.NewGuid()}");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound).Or.EqualTo(HttpStatusCode.NoContent).Or.EqualTo(HttpStatusCode.OK));
    }
}
