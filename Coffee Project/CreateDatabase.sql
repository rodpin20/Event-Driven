
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'CoffeeProjectDB')
BEGIN
    CREATE DATABASE CoffeeProjectDB;
END
GO

USE CoffeeProjectDB;
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Users] (
        [UserId] INT IDENTITY(1,1) PRIMARY KEY,
        [Username] NVARCHAR(50) NOT NULL UNIQUE,
        [Name] NVARCHAR(100) NOT NULL,
        [Password] NVARCHAR(100) NOT NULL,
        [Role] NVARCHAR(20) NOT NULL CHECK ([Role] IN ('Admin', 'Employee')),
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE()
    );
    
    PRINT 'Users table created successfully.';
END
ELSE
BEGIN
    PRINT 'Users table already exists.';
END
GO

-- Optional: Insert a test Admin user (password: admin123)
-- You can remove this after testing
IF NOT EXISTS (SELECT * FROM Users WHERE Username = 'admin')
BEGIN
    INSERT INTO Users (Username, Name, Password, Role, CreatedDate)
    VALUES ('admin', 'Administrator', 'admin123', 'Admin', GETDATE());
    PRINT 'Test admin user created (Username: admin, Password: admin123)';
END
GO

-- Optional: Insert a test Employee user (password: emp123)
-- You can remove this after testing
IF NOT EXISTS (SELECT * FROM Users WHERE Username = 'employee')
BEGIN
    INSERT INTO Users (Username, Name, Password, Role, CreatedDate)
    VALUES ('employee', 'Test Employee', 'emp123', 'Employee', GETDATE());
    PRINT 'Test employee user created (Username: employee, Password: emp123)';
END
GO

PRINT 'Database setup completed successfully!';
GO

