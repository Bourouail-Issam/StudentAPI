-- ============================================================
-- Stored Procedure : usp_RegisterStudent
-- ============================================================
CREATE PROCEDURE dbo.usp_RegisterStudent
    @FullName      NVARCHAR(100),
    @Age           INT,
    @Grade         INT,
    @Email         NVARCHAR(150),
    @PasswordHash  NVARCHAR(512),
    @Role          NVARCHAR(30),
    @NewStudentID  INT OUTPUT,
    @NewUserID     INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF (@FullName IS NULL OR LTRIM(RTRIM(@FullName)) = '')
        THROW 50001, N'FullName cannot be empty.', 1;

    IF (@Age IS NULL OR @Age < 12 OR @Age > 60)
        THROW 50002, N'Age must be between 12 and 60.', 1;

    IF (@Grade IS NULL OR @Grade NOT BETWEEN 0 AND 100)
        THROW 50003, N'Grade must be between 0 and 100.', 1;

    IF (@Email IS NULL OR LTRIM(RTRIM(@Email)) = '')
        THROW 50010, N'Email cannot be empty.', 1;

    IF EXISTS (SELECT 1 FROM dbo.Users WHERE Email = @Email)
        THROW 50009, N'Email is already registered.', 1;

    IF (@PasswordHash IS NULL OR LTRIM(RTRIM(@PasswordHash)) = '')
        THROW 50011, N'PasswordHash cannot be empty.', 1;
  
    IF (@Role NOT IN ('Owner', 'Manager', 'Admin', 'Student', 'Teacher'))
        THROW 50012, N'Invalid Role value.', 1;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO dbo.Users (Email, PasswordHash, [Role])
        VALUES (@Email, @PasswordHash, @Role);
        SET @NewUserID = SCOPE_IDENTITY();

        INSERT INTO dbo.Students (UserId, FullName, Age, Grade)
        VALUES (@NewUserID, @FullName, @Age, @Grade);
        SET @NewStudentID = SCOPE_IDENTITY();

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF (XACT_STATE()) <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO