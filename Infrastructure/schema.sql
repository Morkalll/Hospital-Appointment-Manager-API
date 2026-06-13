IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Users] (
    [Id] TEXT NOT NULL,
    [Name] TEXT NOT NULL,
    [Email] TEXT NOT NULL,
    [Password] TEXT NOT NULL,
    [Role] TEXT NOT NULL,
    [UserType] TEXT NOT NULL,
    [Credential] TEXT NULL,
    [Specialty] TEXT NULL,
    [Disponible] INTEGER NULL,
    [Dni] TEXT NULL,
    [BirthDate] TEXT NULL,
    [PhoneNumber] TEXT NULL,
    [Adress] TEXT NULL,
    [EmployeeNumber] TEXT NULL,
    [Shift] TEXT NULL,
    [Sector] TEXT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);

CREATE TABLE [Rooms] (
    [Id] TEXT NOT NULL,
    [Number] TEXT NOT NULL,
    [Floor] INTEGER NOT NULL,
    [Specialty] TEXT NOT NULL,
    [DoctorId] TEXT NULL,
    CONSTRAINT [PK_Rooms] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Rooms_Users_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [Users] ([Id]) ON DELETE SET NULL
);

CREATE TABLE [Appointments] (
    [Id] TEXT NOT NULL,
    [PatientId] TEXT NOT NULL,
    [DoctorId] TEXT NOT NULL,
    [RoomId] TEXT NOT NULL,
    [DateTime] TEXT NOT NULL,
    [State] TEXT NOT NULL,
    CONSTRAINT [PK_Appointments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Appointments_Rooms_RoomId] FOREIGN KEY ([RoomId]) REFERENCES [Rooms] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Appointments_Users_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Appointments_Users_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [MedicalHistories] (
    [Id] TEXT NOT NULL,
    [AppointmentId] TEXT NOT NULL,
    [PatientId] TEXT NOT NULL,
    [Diagnostic] TEXT NOT NULL,
    [DateTime] TEXT NOT NULL,
    CONSTRAINT [PK_MedicalHistories] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MedicalHistories_Appointments_AppointmentId] FOREIGN KEY ([AppointmentId]) REFERENCES [Appointments] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_MedicalHistories_Users_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_Appointments_DoctorId] ON [Appointments] ([DoctorId]);

CREATE INDEX [IX_Appointments_PatientId] ON [Appointments] ([PatientId]);

CREATE INDEX [IX_Appointments_RoomId] ON [Appointments] ([RoomId]);

CREATE UNIQUE INDEX [IX_MedicalHistories_AppointmentId] ON [MedicalHistories] ([AppointmentId]);

CREATE INDEX [IX_MedicalHistories_PatientId] ON [MedicalHistories] ([PatientId]);

CREATE INDEX [IX_Rooms_DoctorId] ON [Rooms] ([DoctorId]);

CREATE UNIQUE INDEX [IX_Users_Credential] ON [Users] ([Credential]);

CREATE UNIQUE INDEX [IX_Users_Dni] ON [Users] ([Dni]);

CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260520234830_FirstMigrationAfterRefactor', N'10.0.8');

COMMIT;
GO

BEGIN TRANSACTION;
ALTER TABLE [Appointments] DROP CONSTRAINT [FK_Appointments_Users_DoctorId];

ALTER TABLE [Appointments] DROP CONSTRAINT [FK_Appointments_Users_PatientId];

ALTER TABLE [MedicalHistories] DROP CONSTRAINT [FK_MedicalHistories_Users_PatientId];

ALTER TABLE [Rooms] DROP CONSTRAINT [FK_Rooms_Users_DoctorId];

ALTER TABLE [Users] DROP CONSTRAINT [PK_Users];

EXEC sp_rename N'[Users]', N'User', 'OBJECT';

EXEC sp_rename N'[User].[IX_Users_Email]', N'IX_User_Email', 'INDEX';

EXEC sp_rename N'[User].[IX_Users_Dni]', N'IX_User_Dni', 'INDEX';

EXEC sp_rename N'[User].[IX_Users_Credential]', N'IX_User_Credential', 'INDEX';

ALTER TABLE [User] ADD CONSTRAINT [PK_User] PRIMARY KEY ([Id]);

ALTER TABLE [Appointments] ADD CONSTRAINT [FK_Appointments_User_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [User] ([Id]) ON DELETE NO ACTION;

ALTER TABLE [Appointments] ADD CONSTRAINT [FK_Appointments_User_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [User] ([Id]) ON DELETE NO ACTION;

ALTER TABLE [MedicalHistories] ADD CONSTRAINT [FK_MedicalHistories_User_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [User] ([Id]) ON DELETE CASCADE;

ALTER TABLE [Rooms] ADD CONSTRAINT [FK_Rooms_User_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [User] ([Id]) ON DELETE SET NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260522235743_MoveConfigurationsToDbContext', N'10.0.8');

COMMIT;
GO

BEGIN TRANSACTION;
EXEC sp_rename N'[User].[Shift]', N'WorkingShift', 'COLUMN';

EXEC sp_rename N'[User].[Sector]', N'Area', 'COLUMN';

EXEC sp_rename N'[User].[Disponible]', N'IsAvailable', 'COLUMN';

ALTER TABLE [User] ADD [CreatedAt] TEXT NOT NULL DEFAULT '0001-01-01 00:00:00';

ALTER TABLE [User] ADD [DeletedAt] TEXT NOT NULL DEFAULT '0001-01-01 00:00:00';

ALTER TABLE [User] ADD [IsDeleted] INTEGER NOT NULL DEFAULT CAST(0 AS INTEGER);

ALTER TABLE [User] ADD [UpdatedAt] TEXT NOT NULL DEFAULT '0001-01-01 00:00:00';

ALTER TABLE [Rooms] ADD [CreatedAt] TEXT NOT NULL DEFAULT '0001-01-01 00:00:00';

ALTER TABLE [Rooms] ADD [DeletedAt] TEXT NOT NULL DEFAULT '0001-01-01 00:00:00';

ALTER TABLE [Rooms] ADD [IsDeleted] INTEGER NOT NULL DEFAULT CAST(0 AS INTEGER);

ALTER TABLE [Rooms] ADD [UpdatedAt] TEXT NOT NULL DEFAULT '0001-01-01 00:00:00';

ALTER TABLE [MedicalHistories] ADD [CreatedAt] TEXT NOT NULL DEFAULT '0001-01-01 00:00:00';

ALTER TABLE [MedicalHistories] ADD [DeletedAt] TEXT NOT NULL DEFAULT '0001-01-01 00:00:00';

ALTER TABLE [MedicalHistories] ADD [IsDeleted] INTEGER NOT NULL DEFAULT CAST(0 AS INTEGER);

ALTER TABLE [MedicalHistories] ADD [UpdatedAt] TEXT NOT NULL DEFAULT '0001-01-01 00:00:00';

ALTER TABLE [Appointments] ADD [CreatedAt] TEXT NOT NULL DEFAULT '0001-01-01 00:00:00';

ALTER TABLE [Appointments] ADD [DeletedAt] TEXT NOT NULL DEFAULT '0001-01-01 00:00:00';

ALTER TABLE [Appointments] ADD [IsDeleted] INTEGER NOT NULL DEFAULT CAST(0 AS INTEGER);

ALTER TABLE [Appointments] ADD [UpdatedAt] TEXT NOT NULL DEFAULT '0001-01-01 00:00:00';

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260528194624_MigrationPreviousToEmailServiceTest', N'10.0.8');

COMMIT;
GO

BEGIN TRANSACTION;
EXEC sp_rename N'[User].[Adress]', N'Address', 'COLUMN';

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260531235015_MigrationAfterNamespacesUpdate', N'10.0.8');

COMMIT;
GO

BEGIN TRANSACTION;
DECLARE @var nvarchar(max);
SELECT @var = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[User]') AND [c].[name] = N'DeletedAt');
IF @var IS NOT NULL EXEC(N'ALTER TABLE [User] DROP CONSTRAINT ' + @var + ';');
ALTER TABLE [User] ALTER COLUMN [DeletedAt] TEXT NULL;

DECLARE @var1 nvarchar(max);
SELECT @var1 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Rooms]') AND [c].[name] = N'DeletedAt');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Rooms] DROP CONSTRAINT ' + @var1 + ';');
ALTER TABLE [Rooms] ALTER COLUMN [DeletedAt] TEXT NULL;

DECLARE @var2 nvarchar(max);
SELECT @var2 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MedicalHistories]') AND [c].[name] = N'DeletedAt');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [MedicalHistories] DROP CONSTRAINT ' + @var2 + ';');
ALTER TABLE [MedicalHistories] ALTER COLUMN [DeletedAt] TEXT NULL;

DECLARE @var3 nvarchar(max);
SELECT @var3 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'DeletedAt');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT ' + @var3 + ';');
ALTER TABLE [Appointments] ALTER COLUMN [DeletedAt] TEXT NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260602204915_AddCompletedStateAndAddNullable', N'10.0.8');

COMMIT;
GO

BEGIN TRANSACTION;
CREATE UNIQUE INDEX [IX_User_EmployeeNumber] ON [User] ([EmployeeNumber]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260604215541_FixHistoriesAndExceptions', N'10.0.8');

COMMIT;
GO

BEGIN TRANSACTION;
DROP INDEX [IX_User_Credential] ON [User];

DROP INDEX [IX_User_Dni] ON [User];

DROP INDEX [IX_User_EmployeeNumber] ON [User];

DECLARE @var4 nvarchar(max);
SELECT @var4 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[User]') AND [c].[name] = N'WorkingShift');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [User] DROP CONSTRAINT ' + @var4 + ';');
ALTER TABLE [User] ALTER COLUMN [WorkingShift] nvarchar(max) NULL;

DECLARE @var5 nvarchar(max);
SELECT @var5 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[User]') AND [c].[name] = N'UserType');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [User] DROP CONSTRAINT ' + @var5 + ';');
ALTER TABLE [User] ALTER COLUMN [UserType] nvarchar(13) NOT NULL;

DECLARE @var6 nvarchar(max);
SELECT @var6 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[User]') AND [c].[name] = N'UpdatedAt');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [User] DROP CONSTRAINT ' + @var6 + ';');
ALTER TABLE [User] ALTER COLUMN [UpdatedAt] datetime2 NOT NULL;

DECLARE @var7 nvarchar(max);
SELECT @var7 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[User]') AND [c].[name] = N'Specialty');
IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [User] DROP CONSTRAINT ' + @var7 + ';');
ALTER TABLE [User] ALTER COLUMN [Specialty] nvarchar(max) NULL;

DECLARE @var8 nvarchar(max);
SELECT @var8 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[User]') AND [c].[name] = N'Role');
IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [User] DROP CONSTRAINT ' + @var8 + ';');
ALTER TABLE [User] ALTER COLUMN [Role] nvarchar(max) NOT NULL;

DECLARE @var9 nvarchar(max);
SELECT @var9 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[User]') AND [c].[name] = N'PhoneNumber');
IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [User] DROP CONSTRAINT ' + @var9 + ';');
ALTER TABLE [User] ALTER COLUMN [PhoneNumber] nvarchar(max) NULL;

DECLARE @var10 nvarchar(max);
SELECT @var10 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[User]') AND [c].[name] = N'Password');
IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [User] DROP CONSTRAINT ' + @var10 + ';');
ALTER TABLE [User] ALTER COLUMN [Password] nvarchar(max) NOT NULL;

DECLARE @var11 nvarchar(max);
SELECT @var11 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[User]') AND [c].[name] = N'Name');
IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [User] DROP CONSTRAINT ' + @var11 + ';');
ALTER TABLE [User] ALTER COLUMN [Name] nvarchar(max) NOT NULL;

DECLARE @var12 nvarchar(max);
SELECT @var12 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[User]') AND [c].[name] = N'IsDeleted');
IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [User] DROP CONSTRAINT ' + @var12 + ';');
ALTER TABLE [User] ALTER COLUMN [IsDeleted] bit NOT NULL;

DECLARE @var13 nvarchar(max);
SELECT @var13 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[User]') AND [c].[name] = N'IsAvailable');
IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [User] DROP CONSTRAINT ' + @var13 + ';');
ALTER TABLE [User] ALTER COLUMN [IsAvailable] bit NULL;

DECLARE @var14 nvarchar(max);
SELECT @var14 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[User]') AND [c].[name] = N'EmployeeNumber');
IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [User] DROP CONSTRAINT ' + @var14 + ';');
ALTER TABLE [User] ALTER COLUMN [EmployeeNumber] nvarchar(450) NULL;

DROP INDEX [IX_User_Email] ON [User];
DECLARE @var15 nvarchar(max);
SELECT @var15 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[User]') AND [c].[name] = N'Email');
IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [User] DROP CONSTRAINT ' + @var15 + ';');
ALTER TABLE [User] ALTER COLUMN [Email] nvarchar(450) NOT NULL;
CREATE UNIQUE INDEX [IX_User_Email] ON [User] ([Email]);

DECLARE @var16 nvarchar(max);
SELECT @var16 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[User]') AND [c].[name] = N'Dni');
IF @var16 IS NOT NULL EXEC(N'ALTER TABLE [User] DROP CONSTRAINT ' + @var16 + ';');
ALTER TABLE [User] ALTER COLUMN [Dni] nvarchar(450) NULL;

DECLARE @var17 nvarchar(max);
SELECT @var17 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[User]') AND [c].[name] = N'DeletedAt');
IF @var17 IS NOT NULL EXEC(N'ALTER TABLE [User] DROP CONSTRAINT ' + @var17 + ';');
ALTER TABLE [User] ALTER COLUMN [DeletedAt] datetime2 NULL;

DECLARE @var18 nvarchar(max);
SELECT @var18 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[User]') AND [c].[name] = N'Credential');
IF @var18 IS NOT NULL EXEC(N'ALTER TABLE [User] DROP CONSTRAINT ' + @var18 + ';');
ALTER TABLE [User] ALTER COLUMN [Credential] nvarchar(450) NULL;

DECLARE @var19 nvarchar(max);
SELECT @var19 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[User]') AND [c].[name] = N'CreatedAt');
IF @var19 IS NOT NULL EXEC(N'ALTER TABLE [User] DROP CONSTRAINT ' + @var19 + ';');
ALTER TABLE [User] ALTER COLUMN [CreatedAt] datetime2 NOT NULL;

DECLARE @var20 nvarchar(max);
SELECT @var20 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[User]') AND [c].[name] = N'BirthDate');
IF @var20 IS NOT NULL EXEC(N'ALTER TABLE [User] DROP CONSTRAINT ' + @var20 + ';');
ALTER TABLE [User] ALTER COLUMN [BirthDate] date NULL;

DECLARE @var21 nvarchar(max);
SELECT @var21 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[User]') AND [c].[name] = N'Area');
IF @var21 IS NOT NULL EXEC(N'ALTER TABLE [User] DROP CONSTRAINT ' + @var21 + ';');
ALTER TABLE [User] ALTER COLUMN [Area] nvarchar(max) NULL;

DECLARE @var22 nvarchar(max);
SELECT @var22 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[User]') AND [c].[name] = N'Address');
IF @var22 IS NOT NULL EXEC(N'ALTER TABLE [User] DROP CONSTRAINT ' + @var22 + ';');
ALTER TABLE [User] ALTER COLUMN [Address] nvarchar(max) NULL;

DECLARE @var23 nvarchar(max);
SELECT @var23 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[User]') AND [c].[name] = N'Id');
IF @var23 IS NOT NULL EXEC(N'ALTER TABLE [User] DROP CONSTRAINT ' + @var23 + ';');
ALTER TABLE [User] ALTER COLUMN [Id] uniqueidentifier NOT NULL;

DECLARE @var24 nvarchar(max);
SELECT @var24 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Rooms]') AND [c].[name] = N'UpdatedAt');
IF @var24 IS NOT NULL EXEC(N'ALTER TABLE [Rooms] DROP CONSTRAINT ' + @var24 + ';');
ALTER TABLE [Rooms] ALTER COLUMN [UpdatedAt] datetime2 NOT NULL;

DECLARE @var25 nvarchar(max);
SELECT @var25 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Rooms]') AND [c].[name] = N'Specialty');
IF @var25 IS NOT NULL EXEC(N'ALTER TABLE [Rooms] DROP CONSTRAINT ' + @var25 + ';');
ALTER TABLE [Rooms] ALTER COLUMN [Specialty] nvarchar(max) NOT NULL;

DECLARE @var26 nvarchar(max);
SELECT @var26 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Rooms]') AND [c].[name] = N'Number');
IF @var26 IS NOT NULL EXEC(N'ALTER TABLE [Rooms] DROP CONSTRAINT ' + @var26 + ';');
ALTER TABLE [Rooms] ALTER COLUMN [Number] nvarchar(max) NOT NULL;

DECLARE @var27 nvarchar(max);
SELECT @var27 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Rooms]') AND [c].[name] = N'IsDeleted');
IF @var27 IS NOT NULL EXEC(N'ALTER TABLE [Rooms] DROP CONSTRAINT ' + @var27 + ';');
ALTER TABLE [Rooms] ALTER COLUMN [IsDeleted] bit NOT NULL;

DECLARE @var28 nvarchar(max);
SELECT @var28 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Rooms]') AND [c].[name] = N'Floor');
IF @var28 IS NOT NULL EXEC(N'ALTER TABLE [Rooms] DROP CONSTRAINT ' + @var28 + ';');
ALTER TABLE [Rooms] ALTER COLUMN [Floor] int NOT NULL;

DROP INDEX [IX_Rooms_DoctorId] ON [Rooms];
DECLARE @var29 nvarchar(max);
SELECT @var29 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Rooms]') AND [c].[name] = N'DoctorId');
IF @var29 IS NOT NULL EXEC(N'ALTER TABLE [Rooms] DROP CONSTRAINT ' + @var29 + ';');
ALTER TABLE [Rooms] ALTER COLUMN [DoctorId] uniqueidentifier NULL;
CREATE INDEX [IX_Rooms_DoctorId] ON [Rooms] ([DoctorId]);

DECLARE @var30 nvarchar(max);
SELECT @var30 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Rooms]') AND [c].[name] = N'DeletedAt');
IF @var30 IS NOT NULL EXEC(N'ALTER TABLE [Rooms] DROP CONSTRAINT ' + @var30 + ';');
ALTER TABLE [Rooms] ALTER COLUMN [DeletedAt] datetime2 NULL;

DECLARE @var31 nvarchar(max);
SELECT @var31 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Rooms]') AND [c].[name] = N'CreatedAt');
IF @var31 IS NOT NULL EXEC(N'ALTER TABLE [Rooms] DROP CONSTRAINT ' + @var31 + ';');
ALTER TABLE [Rooms] ALTER COLUMN [CreatedAt] datetime2 NOT NULL;

DECLARE @var32 nvarchar(max);
SELECT @var32 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Rooms]') AND [c].[name] = N'Id');
IF @var32 IS NOT NULL EXEC(N'ALTER TABLE [Rooms] DROP CONSTRAINT ' + @var32 + ';');
ALTER TABLE [Rooms] ALTER COLUMN [Id] uniqueidentifier NOT NULL;

DECLARE @var33 nvarchar(max);
SELECT @var33 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MedicalHistories]') AND [c].[name] = N'UpdatedAt');
IF @var33 IS NOT NULL EXEC(N'ALTER TABLE [MedicalHistories] DROP CONSTRAINT ' + @var33 + ';');
ALTER TABLE [MedicalHistories] ALTER COLUMN [UpdatedAt] datetime2 NOT NULL;

DROP INDEX [IX_MedicalHistories_PatientId] ON [MedicalHistories];
DECLARE @var34 nvarchar(max);
SELECT @var34 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MedicalHistories]') AND [c].[name] = N'PatientId');
IF @var34 IS NOT NULL EXEC(N'ALTER TABLE [MedicalHistories] DROP CONSTRAINT ' + @var34 + ';');
ALTER TABLE [MedicalHistories] ALTER COLUMN [PatientId] uniqueidentifier NOT NULL;
CREATE INDEX [IX_MedicalHistories_PatientId] ON [MedicalHistories] ([PatientId]);

DECLARE @var35 nvarchar(max);
SELECT @var35 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MedicalHistories]') AND [c].[name] = N'IsDeleted');
IF @var35 IS NOT NULL EXEC(N'ALTER TABLE [MedicalHistories] DROP CONSTRAINT ' + @var35 + ';');
ALTER TABLE [MedicalHistories] ALTER COLUMN [IsDeleted] bit NOT NULL;

DECLARE @var36 nvarchar(max);
SELECT @var36 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MedicalHistories]') AND [c].[name] = N'Diagnostic');
IF @var36 IS NOT NULL EXEC(N'ALTER TABLE [MedicalHistories] DROP CONSTRAINT ' + @var36 + ';');
ALTER TABLE [MedicalHistories] ALTER COLUMN [Diagnostic] nvarchar(max) NOT NULL;

DECLARE @var37 nvarchar(max);
SELECT @var37 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MedicalHistories]') AND [c].[name] = N'DeletedAt');
IF @var37 IS NOT NULL EXEC(N'ALTER TABLE [MedicalHistories] DROP CONSTRAINT ' + @var37 + ';');
ALTER TABLE [MedicalHistories] ALTER COLUMN [DeletedAt] datetime2 NULL;

DECLARE @var38 nvarchar(max);
SELECT @var38 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MedicalHistories]') AND [c].[name] = N'DateTime');
IF @var38 IS NOT NULL EXEC(N'ALTER TABLE [MedicalHistories] DROP CONSTRAINT ' + @var38 + ';');
ALTER TABLE [MedicalHistories] ALTER COLUMN [DateTime] datetime2 NOT NULL;

DECLARE @var39 nvarchar(max);
SELECT @var39 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MedicalHistories]') AND [c].[name] = N'CreatedAt');
IF @var39 IS NOT NULL EXEC(N'ALTER TABLE [MedicalHistories] DROP CONSTRAINT ' + @var39 + ';');
ALTER TABLE [MedicalHistories] ALTER COLUMN [CreatedAt] datetime2 NOT NULL;

DROP INDEX [IX_MedicalHistories_AppointmentId] ON [MedicalHistories];
DECLARE @var40 nvarchar(max);
SELECT @var40 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MedicalHistories]') AND [c].[name] = N'AppointmentId');
IF @var40 IS NOT NULL EXEC(N'ALTER TABLE [MedicalHistories] DROP CONSTRAINT ' + @var40 + ';');
ALTER TABLE [MedicalHistories] ALTER COLUMN [AppointmentId] uniqueidentifier NOT NULL;
CREATE UNIQUE INDEX [IX_MedicalHistories_AppointmentId] ON [MedicalHistories] ([AppointmentId]);

DECLARE @var41 nvarchar(max);
SELECT @var41 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MedicalHistories]') AND [c].[name] = N'Id');
IF @var41 IS NOT NULL EXEC(N'ALTER TABLE [MedicalHistories] DROP CONSTRAINT ' + @var41 + ';');
ALTER TABLE [MedicalHistories] ALTER COLUMN [Id] uniqueidentifier NOT NULL;

DECLARE @var42 nvarchar(max);
SELECT @var42 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'UpdatedAt');
IF @var42 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT ' + @var42 + ';');
ALTER TABLE [Appointments] ALTER COLUMN [UpdatedAt] datetime2 NOT NULL;

DECLARE @var43 nvarchar(max);
SELECT @var43 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'State');
IF @var43 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT ' + @var43 + ';');
ALTER TABLE [Appointments] ALTER COLUMN [State] nvarchar(max) NOT NULL;

DROP INDEX [IX_Appointments_RoomId] ON [Appointments];
DECLARE @var44 nvarchar(max);
SELECT @var44 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'RoomId');
IF @var44 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT ' + @var44 + ';');
ALTER TABLE [Appointments] ALTER COLUMN [RoomId] uniqueidentifier NOT NULL;
CREATE INDEX [IX_Appointments_RoomId] ON [Appointments] ([RoomId]);

DROP INDEX [IX_Appointments_PatientId] ON [Appointments];
DECLARE @var45 nvarchar(max);
SELECT @var45 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'PatientId');
IF @var45 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT ' + @var45 + ';');
ALTER TABLE [Appointments] ALTER COLUMN [PatientId] uniqueidentifier NOT NULL;
CREATE INDEX [IX_Appointments_PatientId] ON [Appointments] ([PatientId]);

DECLARE @var46 nvarchar(max);
SELECT @var46 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'IsDeleted');
IF @var46 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT ' + @var46 + ';');
ALTER TABLE [Appointments] ALTER COLUMN [IsDeleted] bit NOT NULL;

DROP INDEX [IX_Appointments_DoctorId] ON [Appointments];
DECLARE @var47 nvarchar(max);
SELECT @var47 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'DoctorId');
IF @var47 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT ' + @var47 + ';');
ALTER TABLE [Appointments] ALTER COLUMN [DoctorId] uniqueidentifier NOT NULL;
CREATE INDEX [IX_Appointments_DoctorId] ON [Appointments] ([DoctorId]);

DECLARE @var48 nvarchar(max);
SELECT @var48 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'DeletedAt');
IF @var48 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT ' + @var48 + ';');
ALTER TABLE [Appointments] ALTER COLUMN [DeletedAt] datetime2 NULL;

DECLARE @var49 nvarchar(max);
SELECT @var49 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'DateTime');
IF @var49 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT ' + @var49 + ';');
ALTER TABLE [Appointments] ALTER COLUMN [DateTime] datetime2 NOT NULL;

DECLARE @var50 nvarchar(max);
SELECT @var50 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'CreatedAt');
IF @var50 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT ' + @var50 + ';');
ALTER TABLE [Appointments] ALTER COLUMN [CreatedAt] datetime2 NOT NULL;

DECLARE @var51 nvarchar(max);
SELECT @var51 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'Id');
IF @var51 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT ' + @var51 + ';');
ALTER TABLE [Appointments] ALTER COLUMN [Id] uniqueidentifier NOT NULL;

CREATE UNIQUE INDEX [IX_User_Credential] ON [User] ([Credential]) WHERE [Credential] IS NOT NULL;

CREATE UNIQUE INDEX [IX_User_Dni] ON [User] ([Dni]) WHERE [Dni] IS NOT NULL;

CREATE UNIQUE INDEX [IX_User_EmployeeNumber] ON [User] ([EmployeeNumber]) WHERE [EmployeeNumber] IS NOT NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260613033716_SqlEngineChangedToSQLServer', N'10.0.8');

COMMIT;
GO

BEGIN TRANSACTION;
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260613033956_InitialSqlServer', N'10.0.8');

COMMIT;
GO

BEGIN TRANSACTION;
CREATE TABLE [User] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [Email] nvarchar(450) NOT NULL,
    [Role] nvarchar(max) NOT NULL,
    [UserType] nvarchar(13) NOT NULL,
    [Credential] nvarchar(450) NULL,
    [Specialty] nvarchar(max) NULL,
    [IsAvailable] bit NULL,
    [Dni] nvarchar(450) NULL,
    [BirthDate] date NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [Address] nvarchar(max) NULL,
    [EmployeeNumber] nvarchar(450) NULL,
    [WorkingShift] nvarchar(max) NULL,
    [Area] nvarchar(max) NULL,
    [IsDeleted] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_User] PRIMARY KEY ([Id])
);

CREATE TABLE [Rooms] (
    [Id] uniqueidentifier NOT NULL,
    [Number] nvarchar(max) NOT NULL,
    [Floor] int NOT NULL,
    [Specialty] nvarchar(max) NOT NULL,
    [DoctorId] uniqueidentifier NULL,
    [IsDeleted] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_Rooms] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Rooms_User_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [User] ([Id]) ON DELETE SET NULL
);

CREATE TABLE [Appointments] (
    [Id] uniqueidentifier NOT NULL,
    [PatientId] uniqueidentifier NOT NULL,
    [DoctorId] uniqueidentifier NOT NULL,
    [RoomId] uniqueidentifier NOT NULL,
    [DateTime] datetime2 NOT NULL,
    [State] nvarchar(max) NOT NULL,
    [IsDeleted] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_Appointments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Appointments_Rooms_RoomId] FOREIGN KEY ([RoomId]) REFERENCES [Rooms] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Appointments_User_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [User] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Appointments_User_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [User] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [MedicalHistories] (
    [Id] uniqueidentifier NOT NULL,
    [AppointmentId] uniqueidentifier NOT NULL,
    [PatientId] uniqueidentifier NOT NULL,
    [Diagnostic] nvarchar(max) NOT NULL,
    [DateTime] datetime2 NOT NULL,
    [IsDeleted] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_MedicalHistories] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MedicalHistories_Appointments_AppointmentId] FOREIGN KEY ([AppointmentId]) REFERENCES [Appointments] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_MedicalHistories_User_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [User] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_Appointments_DoctorId] ON [Appointments] ([DoctorId]);

CREATE INDEX [IX_Appointments_PatientId] ON [Appointments] ([PatientId]);

CREATE INDEX [IX_Appointments_RoomId] ON [Appointments] ([RoomId]);

CREATE UNIQUE INDEX [IX_MedicalHistories_AppointmentId] ON [MedicalHistories] ([AppointmentId]);

CREATE INDEX [IX_MedicalHistories_PatientId] ON [MedicalHistories] ([PatientId]);

CREATE INDEX [IX_Rooms_DoctorId] ON [Rooms] ([DoctorId]);

CREATE UNIQUE INDEX [IX_User_Credential] ON [User] ([Credential]) WHERE [Credential] IS NOT NULL;

CREATE UNIQUE INDEX [IX_User_Dni] ON [User] ([Dni]) WHERE [Dni] IS NOT NULL;

CREATE UNIQUE INDEX [IX_User_Email] ON [User] ([Email]);

CREATE UNIQUE INDEX [IX_User_EmployeeNumber] ON [User] ([EmployeeNumber]) WHERE [EmployeeNumber] IS NOT NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260613034719_Initial', N'10.0.8');

COMMIT;
GO

