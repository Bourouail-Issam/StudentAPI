-- ============================================================
-- Stored Procedure : usp_DeleteStudent
-- ============================================================
CREATE PROCEDURE dbo.usp_DeleteStudent
    @StudentID    INT,
    @RowsAffected INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF (@StudentID IS NULL)
        THROW 50008, N'StudentID cannot be Null', 1;

    IF (@StudentID <= 0)
    BEGIN
        DECLARE @ErrorMsg NVARCHAR(200);
        SET @ErrorMsg = N'Not accepted ID : ' + CAST(@StudentID AS NVARCHAR(10));
        THROW 50006, @ErrorMsg, 1;
    END;

    DECLARE @DeletedUserIds TABLE (UserId INT);

    BEGIN TRY
        BEGIN TRANSACTION;

        DELETE FROM dbo.Students
        OUTPUT DELETED.UserId INTO @DeletedUserIds
        WHERE StudentId = @StudentID;

        SET @RowsAffected = @@ROWCOUNT;

        IF (@RowsAffected = 0)
        BEGIN
            DECLARE @ErrorMsg2 NVARCHAR(200);
            SET @ErrorMsg2 = N'Student with ID : ' + CAST(@StudentID AS NVARCHAR(10)) + N' is not found';
            THROW 50007, @ErrorMsg2, 1;
        END;

        DELETE FROM dbo.Users
        WHERE UserId IN (SELECT UserId FROM @DeletedUserIds WHERE UserId IS NOT NULL);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF (XACT_STATE()) <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO