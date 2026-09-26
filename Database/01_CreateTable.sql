
-- ============================================================
-- Table : Users
-- ============================================================
CREATE TABLE Users
(
    UserId       INT           NOT NULL IDENTITY(1,1),
    Email        NVARCHAR(150) NOT NULL,
    PasswordHash NVARCHAR(512) NOT NULL,
    [Role]       NVARCHAR(30)  NOT NULL CONSTRAINT DF_Users_Role DEFAULT ('Student'),

    CONSTRAINT PK_Users PRIMARY KEY (UserId),
    CONSTRAINT UQ_Users_Email UNIQUE (Email),
    CONSTRAINT CK_Users_Role CHECK ([Role] IN ('Owner', 'Manager', 'Admin', 'Student', 'Teacher'))
);
GO
CREATE INDEX IX_Users_Role
ON dbo.Users([Role]);
GO
CREATE INDEX IX_Users_Role_Email
ON dbo.Users([Role], Email);
GO

-- ============================================================
-- Table : Students
-- ============================================================
CREATE TABLE Students
(
    StudentId INT           NOT NULL IDENTITY(1,1),
    UserId    INT           NOT NULL,
    FullName  NVARCHAR(100) NOT NULL,
    Age       INT           NOT NULL,
    Grade     INT           NOT NULL,

    CONSTRAINT PK_StudentID PRIMARY KEY (StudentId),
    CONSTRAINT FK_Students_Users FOREIGN KEY (UserId) REFERENCES Users(UserId),
    CONSTRAINT UQ_Students_UserId UNIQUE (UserId),
    CONSTRAINT CK_Students_Age CHECK (Age >= 12 AND Age <= 60),
    CONSTRAINT CK_Students_Grade CHECK (Grade >= 0 AND Grade <= 100),
    CONSTRAINT CK_Students_FullName CHECK (LEN(LTRIM(RTRIM(FullName))) > 0)
);
GO
