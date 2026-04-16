-- Sử dụng database hiện có
USE SystemBaseVDI;
GO

-- Tạo lại bảng Users
CREATE TABLE [dbo].[Users] (
    [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [Email] NVARCHAR(255) NOT NULL UNIQUE,
    [Username] NVARCHAR(100) NOT NULL UNIQUE,
    [PasswordHash] NVARCHAR(255) NOT NULL,
    [FirstName] NVARCHAR(100) NULL,
    [LastName] NVARCHAR(100) NULL,
    [IsEmailConfirmed] BIT NOT NULL DEFAULT 0,
    [FailedLoginAttempts] INT NOT NULL DEFAULT 0,
    [LockoutEnd] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NULL,
    [LastLoginAt] DATETIME2 NULL,
    [Status] INT NOT NULL DEFAULT 1
);
GO

-- Tạo lại bảng RefreshTokens
CREATE TABLE [dbo].[RefreshTokens] (
    [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [Token] NVARCHAR(255) NOT NULL UNIQUE,
    [UserId] INT NOT NULL,
    [ExpiryDate] DATETIME2 NOT NULL,
    [IsRevoked] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [CreatedByIp] NVARCHAR(50) NULL,
    [RevokedAt] DATETIME2 NULL,
    [RevokedByIp] NVARCHAR(50) NULL,
    [ReplacedByToken] NVARCHAR(255) NULL,
    
    CONSTRAINT FK_RefreshTokens_Users FOREIGN KEY (UserId) REFERENCES [dbo].[Users](Id) ON DELETE CASCADE
);
GO

SELECT * FROM Users 