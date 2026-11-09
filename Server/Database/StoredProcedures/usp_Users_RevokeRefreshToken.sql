-- ============================================================
-- Stored Procedure : usp_Users_RevokeRefreshToken
-- ============================================================
CREATE OR ALTER PROCEDURE dbo.usp_Users_RevokeRefreshToken
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Users
    SET RefreshTokenRevokedAt = SYSUTCDATETIME()
    WHERE UserId = @UserId
      AND RefreshTokenHash IS NOT NULL;
END;
GO