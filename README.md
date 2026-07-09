# Hospital Appointment Management API

## General Description
This is an API built in .NET (C#) for managing hospital appointments and medical histories. The project is structured using **Clean Architecture** principles, separating responsibilities into different layers to promote scalability, maintainability, and testability.

## Workflow
The system allows the interaction of different types of users (Administrators, Doctors, Patients, and Receptionists), handling authentication and role-based authorization (`UserRole`).
- **User Management:** Creation and administration of the different profiles (`AuthService`, `UserService`).
- **Appointments Management:** Patients or receptionists can schedule appointments. Doctors can view their assigned appointments, and the lifecycle of each appointment is managed via the `AppointmentState` enumeration.
- **Medical History:** Doctors can access and record information in the patients' medical history (`MedicalHistoryService`).
- **Rooms Management:** Administration of the physical medical rooms where appointments take place (`RoomService`).
- **Authentication:** Strict access control to the different API endpoints, requiring valid credentials and specific roles.

## Components and Layers (Architecture)

### 1. Domain
Represents the heart of the software. It contains the core business rules and does not depend on any other layer.
- **Entities:** `User`, `Patient`, `Doctor`, `Administrator`, `Receptionist`, `Appointment`, `MedicalHistory`, `Room`.
- **Enums:** Defines states, medical specialties, and roles (`AppointmentState`, `Specialty`, `UserRole`).

### 2. Application
Contains the logic for the system's use cases. It orchestrates how domain entities are used.
- **Services:** Contains transactional logic (`AppointmentService`, `AuthService`, `MedicalHistoryService`, `RoomService`, `UserService`).
- **Requests & Responses (DTOs):** Defines the input and output data structures for creating and modifying records (e.g. `CreateAppointmentReq`, `LoginReq`).
- **Abstractions:** Interfaces for repositories and external services that will be implemented by the infrastructure layer.

### 3. Infrastructure
Handles the technical implementation and communication with external resources, such as the database.
- **Persistence:** Entity Framework Core configuration via `ApplicationDbContext`, migration management, and schemas (`schema.sql`).
- **Repositories:** Concrete implementations of the data access interfaces defined in the Application layer.

### 4. Presentation
The entry point of the REST API. It handles receiving HTTP requests and returning responses.
- **Controllers:** Map the API endpoints to the services in the Application layer.
- **Authorization & Middleware:** Validations, JWT security handling, and global error handling.
- **Configuration:** Main configuration in `Program.cs` and dependency injection.
