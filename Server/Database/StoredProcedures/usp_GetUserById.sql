-- ============================================================
-- Stored Procedure : usp_GetUserById
-- ============================================================
CREATE PROCEDURE dbo.usp_GetUserById
    @userID NVARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;

    IF (@userID <= 0)
        THROW 50010, N'ID cannot be zero or less than zero.', 1;

    SELECT UserId,
           Email,
           PasswordHash,
           Role
    FROM dbo.Users
    WHERE UserId = @userID;
END;
GO