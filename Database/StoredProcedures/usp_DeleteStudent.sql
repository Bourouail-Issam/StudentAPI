-- ============================================================
-- Stored Procedure : usp_DeleteStudent
-- ============================================================
CREATE PROCEDURE dbo.usp_DeleteStudent
    @StudentID  INT,
    @RowsAffected INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF (@StudentID IS NULL)
    BEGIN
        THROW 50008, 'StudentID cannot be Null', 1;
    END;

      IF (@StudentID <= 0)
    BEGIN
        DECLARE @ErrorMsg NVARCHAR(200);
        SET @ErrorMsg = 'Not accepted ID : ' + CAST(@StudentID AS NVARCHAR(10));
        THROW 50006, @ErrorMsg, 1;
    END;

 -- ==========================================
    -- 2) Execution inside a Transaction + Error Handling
    --    (existence check happens here, right before delete,
    --     to avoid a race condition with the earlier pre-check)
    -- ==========================================
    BEGIN TRY
        BEGIN TRANSACTION;

        DELETE FROM dbo.Students
        WHERE StudentId = @StudentID;

        SET @RowsAffected = @@ROWCOUNT;

        IF (@RowsAffected = 0)
        BEGIN
            DECLARE @ErrorMsg2 NVARCHAR(200);
            SET @ErrorMsg2 = 'Student with ID : ' + CAST(@StudentID AS NVARCHAR(10)) + ' is not found';
            THROW 50007, @ErrorMsg2, 1;
        END;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF (XACT_STATE()) <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
