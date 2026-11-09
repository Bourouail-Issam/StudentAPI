
-- ============================================================
-- Stored Procedure : usp_Users_UpdateRefreshToken
-- ============================================================
CREATE OR ALTER PROCEDURE dbo.usp_Users_UpdateRefreshToken
    @UserId                INT,
    @RefreshTokenHash      VARCHAR(44),
    @RefreshTokenExpiresAt DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Users
    SET RefreshTokenHash      = @RefreshTokenHash,
        RefreshTokenExpiresAt = @RefreshTokenExpiresAt,
        RefreshTokenRevokedAt = NULL
    WHERE UserId = @UserId;

    IF @@ROWCOUNT = 0
        THROW 50013, N'User not found.', 1;
END;
GO
