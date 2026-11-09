-- ============================================================
-- Stored Procedure : usp_GetUserById
-- ============================================================
CREATE OR ALTER PROCEDURE dbo.usp_GetUserById
    @userID INT
AS
BEGIN
    SET NOCOUNT ON;

    IF (@userID IS NULL OR @userID <= 0)
        THROW 50014, N'UserId must be greater than zero.', 1;

    SELECT UserId, Email, PasswordHash, [Role]
    FROM dbo.Users
    WHERE UserId = @userID;
END;
GO