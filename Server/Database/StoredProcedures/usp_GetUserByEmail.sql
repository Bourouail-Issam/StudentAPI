-- ============================================================
-- Stored Procedure : usp_GetUserByEmail
-- ============================================================
CREATE PROCEDURE dbo.usp_GetUserByEmail
    @Email NVARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;

    IF (@Email IS NULL OR LTRIM(RTRIM(@Email)) = '')
        THROW 50010, N'Email cannot be empty.', 1;

    SELECT UserId,
           Email,
           PasswordHash,
           Role
    FROM dbo.Users
    WHERE LOWER(Email) = LOWER(@Email);
END;
GO