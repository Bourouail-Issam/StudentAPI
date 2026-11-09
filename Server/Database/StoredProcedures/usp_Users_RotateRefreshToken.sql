-- ============================================================
-- Stored Procedure : usp_Users_RotateRefreshToken
-- ============================================================
CREATE OR ALTER PROCEDURE dbo.usp_Users_RotateRefreshToken
    @OldRefreshTokenHash VARCHAR(44),
    @NewRefreshTokenHash VARCHAR(44),
    @NewExpiresAt        DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Users
    SET RefreshTokenHash      = @NewRefreshTokenHash,
        RefreshTokenExpiresAt = @NewExpiresAt,
        RefreshTokenRevokedAt = NULL
    OUTPUT INSERTED.UserId, INSERTED.Email, INSERTED.[Role]
    WHERE RefreshTokenHash      = @OldRefreshTokenHash
      AND RefreshTokenRevokedAt IS NULL
      AND RefreshTokenExpiresAt > SYSUTCDATETIME();
END;
GO