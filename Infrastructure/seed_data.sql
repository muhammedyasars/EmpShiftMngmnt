 
SET IDENTITY_INSERT Employees ON
INSERT INTO Employees (Id, FirstName, LastName, Email, IsActive, CreatedAt, UpdatedAt)
VALUES 
(1, 'John', 'Doe', 'john.doe@example.com', 1, GETUTCDATE(), GETUTCDATE()),
(2, 'Jane', 'Smith', 'jane.smith@example.com', 1, GETUTCDATE(), GETUTCDATE()),
(3, 'Bob', 'Johnson', 'bob.johnson@example.com', 1, GETUTCDATE(), GETUTCDATE());
SET IDENTITY_INSERT Employees OFF

 
SET IDENTITY_INSERT Shifts ON
INSERT INTO Shifts (Id, EmployeeId, StartTime, EndTime, IsDeleted, CreatedAt, UpdatedAt)
VALUES 
(1, 1, DATEADD(DAY, 1, GETUTCDATE()), DATEADD(DAY, 1, DATEADD(HOUR, 8, GETUTCDATE())), 0, GETUTCDATE(), GETUTCDATE()),
(2, 1, DATEADD(DAY, 2, GETUTCDATE()), DATEADD(DAY, 2, DATEADD(HOUR, 8, GETUTCDATE())), 0, GETUTCDATE(), GETUTCDATE());

 
INSERT INTO Shifts (Id, EmployeeId, StartTime, EndTime, IsDeleted, CreatedAt, UpdatedAt)
VALUES 
(3, 2, DATEADD(DAY, 1, GETUTCDATE()), DATEADD(DAY, 1, DATEADD(HOUR, 6, GETUTCDATE())), 0, GETUTCDATE(), GETUTCDATE()),
(4, 2, DATEADD(DAY, 3, GETUTCDATE()), DATEADD(DAY, 3, DATEADD(HOUR, 6, GETUTCDATE())), 0, GETUTCDATE(), GETUTCDATE());

 
INSERT INTO Shifts (Id, EmployeeId, StartTime, EndTime, IsDeleted, CreatedAt, UpdatedAt)
VALUES 
(5, 3, DATEADD(DAY, 2, GETUTCDATE()), DATEADD(DAY, 2, DATEADD(HOUR, 9, GETUTCDATE())), 0, GETUTCDATE(), GETUTCDATE()),
(6, 3, DATEADD(DAY, 4, GETUTCDATE()), DATEADD(DAY, 4, DATEADD(HOUR, 9, GETUTCDATE())), 0, GETUTCDATE(), GETUTCDATE());
SET IDENTITY_INSERT Shifts OFF