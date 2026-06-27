$baseUrl = "http://localhost:5163/api"
$Headers = @{ "Content-Type" = "application/json" }

Write-Host "Disabling SSL validation for local tests..." -ForegroundColor Yellow
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = { $true }
Start-Sleep -Seconds 3

Write-Host "0. Admin Login..." -ForegroundColor Cyan
try {
    $adminLoginBody = @{
        email = "admin@hospital.com"
        password = "Doctor1234!"
    } | ConvertTo-Json
    $adminLoginResponse = Invoke-RestMethod -Uri "$baseUrl/auth/login" -Method Post -Headers $Headers -Body $adminLoginBody
    $adminToken = $adminLoginResponse.token
    $Headers.Add("Authorization", "Bearer $adminToken")
    Write-Host "Success! Retrieved Admin JWT Token." -ForegroundColor Green
} catch {
    Write-Host "Failed to login as admin: $_" -ForegroundColor Red
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $reader.BaseStream.Position = 0
        $reader.DiscardBufferedData()
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response Body: $responseBody" -ForegroundColor Red
    }
}

# 1. Create Patient
Write-Host "1. Testing Create Patient..." -ForegroundColor Cyan
try {
    $patientBody = @{
        name = "Bob Marley"
        email = "bob.marley@example.com"
        dni = "87654321"
        birthDate = "1980-01-01"
        phoneNumber = "555-4321"
        address = "456 Side St"
    } | ConvertTo-Json
    $patientResponse = Invoke-RestMethod -Uri "$baseUrl/User/create-patient" -Method Post -Headers $Headers -Body $patientBody
    $patientId = $patientResponse.id
    Write-Host "Success! Created Patient ID: $patientId" -ForegroundColor Green
} catch {
    Write-Host "Failed to create patient: $_" -ForegroundColor Red
}

# 2. Get Rooms (Need RoomId for Doctor)
Write-Host "2. Testing Get Rooms..." -ForegroundColor Cyan
try {
    $roomsResponse = Invoke-RestMethod -Uri "$baseUrl/Rooms" -Method Get -Headers $Headers
    $roomId = $roomsResponse[0].id
    Write-Host "Success! Found Room ID: $roomId" -ForegroundColor Green
} catch {
    Write-Host "Failed to get rooms: $_" -ForegroundColor Red
}

# 3. Create Doctor
Write-Host "3. Testing Create Doctor..." -ForegroundColor Cyan
try {
    $doctorBody = @{
        name = "Dr. House"
        email = "dr.house@hospital.com"
        credential = "MED-12345"
        specialty = 0
        password = "Password123!"
        phoneNumber = "123456789"
    } | ConvertTo-Json
    $doctorResponse = Invoke-RestMethod -Uri "$baseUrl/User/create-doctor" -Method Post -Headers $Headers -Body $doctorBody
    Write-Host "Success! Created Doctor." -ForegroundColor Green
} catch {
    Write-Host "Failed to create doctor: $_" -ForegroundColor Red
}

# 4. Create Receptionist
Write-Host "4. Testing Create Receptionist..." -ForegroundColor Cyan
try {
    $recBody = @{
        name = "Alice Rec"
        email = "alice@hospital.com"
        employeeNumber = "REC-001"
        password = "Password123!"
    } | ConvertTo-Json
    $recResponse = Invoke-RestMethod -Uri "$baseUrl/User/create-receptionist" -Method Post -Headers $Headers -Body $recBody
    Write-Host "Success! Created Receptionist." -ForegroundColor Green
} catch {
    Write-Host "Failed to create receptionist: $_" -ForegroundColor Red
}

# 5. Login (Auth)
Write-Host "5. Testing Login (Auth)..." -ForegroundColor Cyan
try {
    $loginBody = @{
        email = "dr.house@hospital.com"
        password = "Password123!"
    } | ConvertTo-Json
    $loginResponse = Invoke-RestMethod -Uri "$baseUrl/auth/login" -Method Post -Headers $Headers -Body $loginBody
    Write-Host "Success! Logged in as: $($loginResponse.email)" -ForegroundColor Green
} catch {
    Write-Host "Failed to login: $_" -ForegroundColor Red
}

# 6. Get All Users
Write-Host "6. Testing Get All Users..." -ForegroundColor Cyan
try {
    $usersResponse = Invoke-RestMethod -Uri "$baseUrl/User" -Method Get -Headers $Headers
    Write-Host "Success! Retrieved $($usersResponse.Count) users." -ForegroundColor Green
} catch {
    Write-Host "Failed to get users: $_" -ForegroundColor Red
}

# 7. Get User By ID
Write-Host "6. Testing Get User By ID..." -ForegroundColor Cyan
try {
    $user = Invoke-RestMethod -Uri "$baseUrl/User/$patientId" -Method Get -Headers $Headers
    Write-Host "Success! Retrieved user: $($user.name)" -ForegroundColor Green
} catch {
    Write-Host "Failed to get user by ID: $_" -ForegroundColor Red
}

# 7. Get Rooms
Write-Host "8. Testing Get Rooms..." -ForegroundColor Cyan
$roomId = $null
try {
    $rooms = Invoke-RestMethod -Uri "$baseUrl/Room" -Method Get -Headers $Headers
    if ($rooms.Count -gt 0) {
        $roomId = $rooms[0].id
        Write-Host "Success! Found Room ID: $roomId" -ForegroundColor Green
    } else {
        Write-Host "No rooms found. This might cause appointment creation to fail." -ForegroundColor Yellow
    }
} catch {
    Write-Host "Failed to get rooms: $_" -ForegroundColor Red
}

Write-Host "NOTE: Skipping remaining endpoint validations (Appointment/MedicalHistory) in PS script as they require specific time/room constraints seeded in the DB." -ForegroundColor Yellow
Write-Host "Endpoints mapping complete!" -ForegroundColor Cyan
